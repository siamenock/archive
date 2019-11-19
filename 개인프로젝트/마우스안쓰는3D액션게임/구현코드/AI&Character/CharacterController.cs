using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CharacterUpdater is used for motion, status update, activating skill
// interaction with outer variable is prohibited.
// do it with CharacterInteractor, not this.
public class CharacterController : MonoBehaviour {
	public  static readonly float   NO_DIR      = 360f, //can't i use const on C#?
                                    DOWN_TIME   = 2f,
                                    ROTATE_SPEED= 720f;
    public enum State
    {
        hit = -10, down = -11,
        idle = -1, move = -2, jump = -3,

        sk00 = 00, sk01 = 01, sk02 = 02, sk03 = 03, sk04 = 04, sk05 = 05, sk06 = 06, sk07 = 07, sk08 = 08, sk09 = 09,
        sk10 = 10, sk11 = 11, sk12 = 12, sk13 = 13, sk14 = 14, sk15 = 15, sk16 = 16, sk17 = 17, sk18 = 18, sk19 = 19,
        sk20 = 20, sk21 = 21, sk22 = 22, sk23 = 23, sk24 = 24, sk25 = 25, sk26 = 26, sk27 = 27, sk28 = 28, sk29 = 29
    };

    // linking pointers
    GameObject      model        = null,
                    camHolder    = null;
    Rigidbody       rigidbody    = null;
    TargetUpdater   targeter     = null;
    Stat            stat         = null;
    Animator        modelAnimator= null;
    BoxCollider     boxCollider  = null;

    // 실제 instance
    [HideInInspector] public List<Skill> skills = new List<Skill>();
    
    State             state;
    float 	          motionTime   = 0f,
	                  lastDir	   = 0f;
    bool              rotateLock;
    Quaternion        lockedToward;
    Quaternion        modelToward;            //target direction to turn model toward. not currunt rotation.   

    // for WASD mode only. Not used in Auto mode
    bool wasdMode = false;
    Camera cam = null;      

                            // Use this for initialization
    void Start () {
        stat        = GetComponent<Stat> ();
        rigidbody   = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        model       = MyLibrary.FindChildGameObjectWithName(this.gameObject, "PlayerModel");
        
        if (model) {
            modelAnimator = model.GetComponent<Animator>();
            if (modelAnimator == null)
            {
                print("FATAL ERROR : model animator init   ON  " + this.name);
            }
        } else {
            //print ("init err. " + this.name + " fail to find model");
        }


        if (name == "Player") {	camHolder = MyLibrary.FindChildGameObjectWithName(this.gameObject, "CameraHolder");
		} else {  /* AI */		camHolder = this.gameObject;    // cam holder는 방향지시용으로 사용됨
		}
		targeter	= GetComponent<TargetUpdater>();
		state       = State.idle;

        if (this.GetComponent<CommandFilterWASD>() != null)
            this.wasdMode = true;

        if(camHolder != null && camHolder != this.gameObject)
            cam = MyLibrary.FindChildGameObjectWithName(camHolder, "MainCamera").GetComponent<Camera>();

    }


	public void TurnSmoothToward(GameObject target){	// for AI, not for user
        if (target == null) return;
        Vector3 look_pos    = stat.CalcEvadeDelayPos2Enemy(target);
        TurnSmoothToward(look_pos);
	}
    public void TurnSmoothToward(Vector3 look_pos) {
        if (rotateLock)
        {
            return;
        }
        Vector3 dir = look_pos - this.transform.position;
        dir.y = 0;                                                  // 수평
        Quaternion angle = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle, ROTATE_SPEED * Time.deltaTime);
    }

	// ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ Update Key states ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼
	//			REPLACE Update(), MoveToward() in this class													
	// 			UpdateKeystate is called once per frame by CommandFilter Before UpdateCommand called.			
	public void UpdateKeystate(CommandFilter.KeyInput input){
        //----------------------------------------------//
        //         모션 타임 감소, 전체 쿨타임 감소
        //----------------------------------------------//
        float dir = input.To360Dir();	// dir == clockwise input direction. 0~360, 0 = forward, 360 = no input
		if(0 < motionTime){			// can not do other action now.
			motionTime -= Time.deltaTime;
		} else if(state != State.idle || motionTime != 0){						// hit or down
            motionTime = 0;
			state = State.idle;
			modelAnimator.SetInteger ("ani_code", (int)State.idle);
		}
        this.UpdateSkillCoolTime();
        
        // 현재 이동속도 스텟 연동
        float speed_move = this.stat.Get(Stat.Name.SPEED_MOVE);

        //----------------------------------------------//
        //         스킬 진행 또는 기본동작 진행
        //----------------------------------------------//
        if (0 <= state)	    // if using skill. all basic motions are in minus state
        {
            SkillProcess(skills[(int)state], input);
        }
        else if (state == State.idle)
        {
            if (dir == NO_DIR)
            {
                modelAnimator.SetInteger("ani_code", (int)State.idle);
            }
            else		    // simple moving is also "state.basic"
            {
                modelAnimator.SetInteger("ani_code", (int)State.move);      //don't change character state for basic moving...
                modelToward = Convert2WorldDir(lastDir = dir);						//pattern of is_rotatable
                Vector3 euler = modelToward.eulerAngles;
                modelToward = Quaternion.Euler(0, euler.y, 0);                  // 캐릭터는 좌우회전만 함

                TurnModelToward(modelToward);
                SetSpeed(modelToward, speed_move);
            }
        }
        else if (state == State.down){          // if not grounded, timer not moves.
            if (! this.IsOnGround())
                motionTime += Time.deltaTime;   // netralize the - at the start of this function

        } else { 
            // if state < -1
			// hit-motion or down-motion. nothing to control.
			// just let motion_time decrease
            // do nothing
		}
	}

    // called if Command found on CommandFilter
    public void ActivateCommand(int skill_num){
		if(0 < motionTime){	//can not start other action now.
            print("다른 모션 진행중. 무시됨-새로운 커멘드("+ skills[skill_num].name +")");
			return;				//TODO : 모션 중 캔슬스킬/조작 추가 필요
		}

		if (0 <= skill_num && skill_num < skills.Count) {
            state = (CharacterController.State)skill_num;
			modelAnimator.SetInteger ("ani_code", skill_num);   // actually, code != State.idle, != State.move
            print("START SKILL :: " + skills[skill_num].name);
            if (StartSkill(skill_num))
                return;
            else
                print("현재 쿨타임임. 무시됨-새로운 커멘드(" + skills[skill_num].name);
		} else {
			print ("Command Code does not match!");
			return;
		}
	}

    // if (cooltime) return false;
    bool StartSkill(int code) {
        Skill skill = skills[code];
        if (skill.ActInit()) {
            state = (State)code;
            motionTime = 999f;     // todo
            return true;
        };

        return false;
    }
    void SkillProcess(Skill skill, CommandFilter.KeyInput input) {
        SkillRequest req = skill.Act(input);    
        if (req == SkillRequest.END_SKILL) {    // == null
            rotateLock = false;
            print("end skill " + (int)this.state);
            this.state = State.idle;
            this.motionTime = 0;
            return;
        }
        if (req.setRotateLock.HasValue)
            SetRotateLock(req.setRotateLock.Value);
        if (req.motionName != null)                 // 스킬에서 새로운 페이즈 시작시 새 모션 가능
            modelAnimator.Play(req.motionName);

        //debug_log = req.movement.ToString() + " == movement\n";
        //print(debug_log);
        float speed = stat.Get(Stat.Name.SPEED_MOVE);
        Vector3 move_character = req.movement * speed;                      // character 기준 이동
        Vector3 move_world     = Convert2WorldDir(NO_DIR) * move_character; // world 기준 이동
        TurnModelToward(modelToward);
        SetSpeed(move_world);


        if (req.atkObjs.Count != 0)
            foreach (GameObject o in req.atkObjs)          // TODO : warning box도 받게 만들어야?
                Attack2MainTarget(o);
        
        //print("Skill Process of " + skill.name + "\n" + debug_log);
    }	

	public State GetState(){ return state;	}

	public void KnockBack(float time){          // 넉백 (경직 + 밀어내기)
		if(state == State.down){	return; }   // 이미 다운인 경우 새로운 넉백공격을 맞아도 아무 일 없음 (일단 지금은...)
        rotateLock = false;

        state = State.hit;
		motionTime = time;
        if (modelAnimator == null)
            return;

		modelAnimator.Play ("HIT#-10");
		modelAnimator.SetInteger ("ani_code", -10);
	}
	public void KnockDown(){                    // 넉다운 (쓰러뜨리기)
	    if(state == State.down) { return;	}	// 이미 다운인 경우 새로운 다운공격을 맞아도 아무 일 없음 (일단 지금은...)
        rotateLock = false;                     // 공중에 떠있는동안은 넉다운 카운터가 감소안하도록 (계속 공중에 띄우면 못 일어남) 했는데
                                                // 애니메이션이 쓰러지고 일어나는것까지 한 세트라서 눈에 보이는 건 그냥 서 있음.
        state = State.down;
		motionTime = DOWN_TIME;
		modelAnimator.Play ("DOWN#-11");
		modelAnimator.SetInteger ("ani_code", -11);
	}

   
    void Attack2MainTarget(GameObject atk_obj)
    {
        Attack atk = atk_obj.GetComponent<Attack>();
        

        if (atk == null) return;                        // something wrong
        
        GameObject target = GetMainTarget();
        Vector3 fire_from = transform.position + Quaternion.LookRotation(transform.forward, Vector3.up) * atk.pos_instance;
        
        Quaternion  fire_dir = new Quaternion();
        if (!wasdMode) { // auto mode
            
            if (target)
            {
                Bullet bullet = atk_obj.GetComponent<Bullet>();
                if (bullet == null)
                {                                           // 시전즉시 맞는 공격들은 미래예측 알고리즘이 필요없음. 대신 딜레이는 만들어야 함
                    fire_dir = DirTo(fire_from, target);    // 회피, 명중 계산까지 이 안에서 처리함       

                }
                else {
                    fire_dir = PredictTo(fire_from, target, bullet);// 회피, 명중 계산까지 이 안에서 처리함       
                }
            }
            else fire_dir = Convert2WorldDir(this.lastDir);        // forward
        }
        else
        {          // WASD mode
            Vector3 fire_to = MyLibrary.RayPoint(cam.transform.position, cam.transform.forward, MyLibrary.CriteriaTerrainOrEnemy);
            Vector3 distance = (fire_to - this.transform.position);         // fire from == 0 일때만 동작함

            fire_dir.SetLookRotation(distance.normalized);
        }
            
        InstantiateAttack(atk_obj, fire_from, fire_dir, target);
    }
    Quaternion Convert2WorldDir(float dir){	    //convert looking_dir based dir 2 world dir. 
		if (dir == NO_DIR)
			dir = 0;

		return Quaternion.Euler (0f, dir, 0f) * GetCharacterDirection(); 
	}


    void TurnModelToward                  (Quaternion word_dir) { model.transform.rotation = Quaternion.RotateTowards(model.transform.rotation, word_dir, ROTATE_SPEED * Time.deltaTime); }
    void TurnModelTowardIgnoreRotationLock(Quaternion word_dir) { model.transform.rotation = Quaternion.RotateTowards(model.transform.rotation, word_dir, ROTATE_SPEED * Time.deltaTime); }
    void SetSpeed (Quaternion dir, float speed){ SetSpeed(dir * Vector3.forward * speed); }
    void SetSpeed               (Vector3 speed){ rigidbody.velocity = speed;}


    GameObject GetMainTarget(){
		return targeter.GetMainTarget ();
	}


    // posInstattiate : atk obj 생성하는 상대좌표. 현재 미구현
	Quaternion DirTo(Vector3 posInstantiate, GameObject target){	//evade system included	

        //▼▼▼▼▼▼▼▼ set attacking direction ▼▼▼▼▼▼▼▼
        Vector3 posAtk = target.transform.position;
        Quaternion qtrAtk = new Quaternion();
        if (target)
        {
            posAtk = stat.CalcEvadeDelayPos2Enemy(target);          // 회피시스템ㅁ 적용 후의 위치

            Vector3 dirAtk = posAtk - this.transform.position;
            qtrAtk.SetLookRotation(dirAtk);
        }
        else {
            qtrAtk = this.modelToward;
            //print("회피/명중 시스템 작동불가 : 타겟 없음");
        }
        
        return qtrAtk;
    }

    Quaternion PredictTo(Vector3 posInstantiate, GameObject target, Bullet bullet)      // 현재 pos 사용 안하고 있음!
    {	//evade system included	
        float evade = 0, chase = 0;
        float delay = 0;    // evade - chase

        float distance = (target.transform.position - this.transform.position).magnitude;
        float time = distance / bullet.speed;

        //▼▼▼▼▼▼▼▼ set attacking direction ▼▼▼▼▼▼▼▼
        Vector3 posAtk = this.transform.position + this.transform.forward * 2;
        Quaternion qtrAtk = new Quaternion();
        if (target)
        {
            Stat targetStat   = target.GetComponent<Stat>();
            Trace targetTrace = target.GetComponent<Trace>();

            if (this.stat) chase = this.stat.Get(Stat.Name.CHASE);
            if (targetStat) evade = targetStat.Get(Stat.Name.EVADE);
            delay = evade - chase;
            posAtk = targetTrace ? targetTrace.PredictAfter(time, delay) : target.transform.position;       // TODO : use accuracy
            //print("회피/명중 딜레이 == " + delay + "\ntrace시스템 적용여부 : " + (targetTrace != null));

            Vector3 dirAtk = posAtk - this.transform.position;
            qtrAtk.SetLookRotation(dirAtk);
        }
        else
        {
            qtrAtk = this.modelToward;
            
        }
        //Instantiate(DB_temp.lib[DB_temp.NAME.pos_marker1], posAtk, new Quaternion());

        return qtrAtk;
    }




    GameObject InstantiateAttack(GameObject atk_obj, Quaternion rotation, GameObject target){
		return InstantiateAttack (atk_obj, this.transform.position, rotation, target);
	}
	GameObject InstantiateAttack(GameObject atk_obj, Vector3 position, Quaternion rotation, GameObject target){
		GameObject atk		= Instantiate (atk_obj, position, rotation);
		Attack atk_data		= atk.GetComponent<Attack> ();
        
		if (atk_data != null) {
			atk_data.attacker	= gameObject;
            atk_data.target     = target;

        } else {
			print ("Instatiate fail");
		}
		return atk;
	}

	// reduce cooltime of all skill except currently used
	void UpdateSkillCoolTime(){
        for (int i = 0; i < this.skills.Count; i++) {
            if (i == (int)this.state) {
                //현재 사용중인 스킬임
                continue;
            }
            else
                this.skills[i].ReduceCooltime();
        }
	}

    void SetRotateLock(bool val) {
        rotateLock = val;
        if(val)
            lockedToward = this.modelToward;
    }

    Quaternion GetCharacterDirection() {
        if(rotateLock)  return this.lockedToward;
        else            return camHolder.transform.rotation;
    }

    public bool IsOnGround()
    {
        float character_len = boxCollider.size.y /2 - boxCollider.center.y;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, -Vector3.up, character_len + 0.05f);
        foreach (RaycastHit h in hits) {
            if(h.collider.tag == "Terrain")
                return true;
        }

        return false;
    }
}