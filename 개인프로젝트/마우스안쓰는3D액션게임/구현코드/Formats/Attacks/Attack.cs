using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : AttackOrWarning {
    // Attack 클래스는 다음 클래스들로 상속!
    // HitRay, HitBox, Bullet
    // Bullet은 Missle로 상속된다.

    //----------------------------------------------------------//
    //   [Attack Or Warning] class 에게서 상속받은 attribute
    //----------------------------------------------------------//
    //[HideInInspector] public GameObject attacker = null; 
    //------------------------- 상속 끝-------------------------

    public GameObject   target;                                     // 지금은 Missle만 사용하는 변수지만, Instantiate의 범용성을 위해 Attack에서 받음.
    public Warning      warning;
    public List<Effect> effects         = new List<Effect>();       // 미구현
    public float    damage_physical     = 0;
    public float    additional_chase    = 0;                        // 추적 스텟 보조 (캐릭터 스텟에 이거 추가됨)
    public bool     knock_down          = false;                    // false -> knock_back, true -> knock_down  // neither -> push=0, knocktime=0
    public float    knock_time          = 0;                        // for knock back motion. not needed for knock down
    public Vector3  knock_back          = new Vector3(0, 0, 0);     // will be combined with attack direction. to make it push  normal,  [knockback_speed * Vector3(1, 0, 1).normalize]   recoomanded
    public Vector3  pos_instance        = new Vector3(0f, 0f, 0f);  // 생성시 위치
    public Vector3  rot_instance_eulier = new Vector3(0,0,0);       // 생성시 방향. rotation을 그대로 복붙해올 것
    


    void OnTriggerEnter(Collider other){
        if (this.attacker == null) {	return;	} // attack not set yet

		CharacterInteractor enemy = other.GetComponent<CharacterInteractor>();
		CharacterInteractor atker = attacker.GetComponent<CharacterInteractor> ();
        bool hit_enemy = false;
        if (enemy != null && atker != null) {
			if (enemy.team != atker.team){
				//print ("맞은 놈이 적군임. enemy.Hit(this attack object)");
				enemy.Hit(this);
                hit_enemy = true;
			} else if (enemy.team == atker.team) {
				//print ("맞은 놈이 아군임. 아무 것도 안함");
				return;
			}
		}


        if (this is HitRay || this is Bullet) { 
            // TODO : EXPLOSION?? maybe put it on Attack Ondestroy
            // 적에게 맞았거나, 벽에 부딪혔거나
            if (other.tag == "Terrain" || hit_enemy)
			    Destroy (this.gameObject);
		}
	}


    
    protected virtual void Start()    
    {
        // 공통사항이 없어서 여기선 pass (시작지점 세팅하는 건 최초 Instantiate할 때 CharacterUpdater에서 처리)
        // 폭발처리나 링크드 처리 등 나중에 추가
        // Attack의 정보를 실제 GameObject에 적용하는 파트. 상속받은 클래스에서 처리할 것


        //------------------
        // 워닝박스를 여기서 생성했음.
        // todo1 : 스킬시작시로 변경
        // todo2 : 관련 페이즈 시작시로 변경
        EnableWarning(this.attacker, 0f);
        

        //이건 남겨야됨
        

    }

    protected virtual void Update()
    {
        //Missle 쪽은 처리할거 많을 듯. 매턴 유도해줘야 하잖아.
    }

    // WarningBox를 비활성화 된 상태로 instantiate
    // 이거 하려면 모든 Attack Obj의 충돌박스는 Cube형태여야 하고 (특이한 모양으로 하려는건 여러개 묶은 Linked attack으로 나중에 개발)
    // Attack Obj의 회전은 this.rot_instance에 적혀있어야지 기본 rotation은 0이어야 함
    // Attack type별 추가작동사항은 override해서 조절함.
    protected virtual void BuildWarningDisabled(GameObject attacker, float time_predelay) {
        Vector3 v = this.rot_instance_eulier;
        Quaternion rot_instance = Quaternion.Euler(v.x, v.y, v.z);
        Vector3     sum_pos = this.transform.position + this.transform.rotation * this.pos_instance;
        
        Quaternion sum_rot = rot_instance * this.transform.rotation;
        GameObject  ret;
        if (this is Bullet)
            ret = Resources.Load("WarningBoxBullet(UnActive)", typeof(GameObject)) as GameObject;
        else
            ret = Resources.Load("WarningBox(UnActive)", typeof(GameObject)) as GameObject;
        ret = Instantiate(ret, sum_pos, sum_rot);

        ret.transform.localScale = this.transform.localScale;       // 이거 충분한건가?

        this.warning                    = ret.GetComponent<Warning>();
        this.warning.time_attack_start  = Time.time + time_predelay;
        this.warning.attack             = this;
        this.warning.attacker           = this.attacker;
        this.warning.is_attack_start    = true;
    }
    
    
    protected virtual void EnableWarning(GameObject attacker, float time_predelay) {
        BuildWarningDisabled(attacker, time_predelay);

        // Missle은 최초생성시에 WarningBox 생성안됨(궤도 확정된 뒤에 warningbox 활성화)
        // Todo : warning 타이머기능은 달아줘야 함. 그리고 다음 2줄 좀 정리하자.
        if (this is Missle)
            return;

        this.warning.gameObject.SetActive(true);
        return;
    }


}
