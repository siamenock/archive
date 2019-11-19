using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class Skill is not only skill data storage. It actually calculate motion time when this.UpdateSkill() called by character
public class Skill
{
    // attribute 추가시 DeeopClone()함수도 업데이트 해줘야 함

    // static DB
    // 2 dictionary are set by class [Skill List Global Initialize]
    [HideInInspector] public static Dictionary<string, Skill> preset = new Dictionary<string, Skill>();
    [HideInInspector] public static Dictionary<string, CommandFilter.Command> precmd = new Dictionary<string, CommandFilter.Command>();

    // about owner
    [HideInInspector] public GameObject    owner = null;              // owner == character who has this skill
    [HideInInspector] public Stat          ownerStat = null;         // Stat.atk_speed will affect speed of using skill
    [HideInInspector] public CommandFilter ownerCommandFilter = null; // owner data will be set by SetSkill class when character get skill data
    [HideInInspector] public CommandFilterWASD ownerCommandFilterWASD = null; // owner data will be set by SetSkill class when character get skill data


    // about skill
    [HideInInspector] public string name = "default";
    [HideInInspector] public float constCoolTime = 0;     //any way next use const proper?
    [HideInInspector] public float curCoolTime = 0;
    [HideInInspector] public List<Phase> phases = new List<Phase>();
    [HideInInspector] public List<PhaseLinker> phaseLinkers = new List<PhaseLinker>();
    //[HideInInspector] public float delay_pre, delay_post, dps, range;	// TODO: data for AI's decision, maybe better if abstract?

    // private, temporary values
    private List<PhaseLinker> curLinkers = new List<PhaseLinker>();
    private int curPhase = 0;
    private float curTimer = 0f;
    private CommandFilter.KeyInput curKeystate;

    public Skill DeepClone()
    {
        Skill ret = new Skill
        {
            name = this.name,
            //ret.is_rotateable	= is_rotateable;
            constCoolTime = constCoolTime,
            phases = new List<Phase>(phases.Count)
        };

        for (int i = 0; i < phases.Count; i++)
        {
            ret.phases.Add(new Phase()
            {
                motion_name = phases[i].motion_name,
                motion_time = phases[i].motion_time,
                rotate_speed= phases[i].rotate_speed,
                atk_obj     = phases[i].atk_obj,

                movement = phases[i].movement,
                movement_additional = phases[i].movement_additional
            });
        }
        ret.phaseLinkers = new List<PhaseLinker>();
        foreach (PhaseLinker l in this.phaseLinkers) {
            PhaseLinker clone = new PhaseLinker(l.trigger_querry, l.prev,l.next,l.recursion_max,l.additional);
            ret.phaseLinkers.Add(clone);
        }
        return ret;
    }

    public bool ActInit(bool ignore_cool_time = false)
    {
        //MonoBehaviour.print(this.name + " .ActInit() 호출됨. 링커갯수 : " + this.phaseLinkers.Count);
        if (this.curCoolTime > 0 && !ignore_cool_time)
            return false;
        this.curPhase = -1;    // phase 0 will start when first [this.Act()] called
        this.curTimer = 0f;    // if (timer <=0) -> start next phase
        this.curLinkers.Clear();
        this.curCoolTime = this.constCoolTime;
        foreach (PhaseLinker l in this.phaseLinkers)
            l.Init();

        return true;
    }
    public SkillRequest Act(CommandFilter.KeyInput input)
    {
        SkillRequest start_req = null; //return value
        this.curTimer -= Time.deltaTime;

        if (curTimer <= 0)
        {
            // current phase end. move to other phase
            const int default_val = -999;
            int next_phase = default_val;
            foreach (PhaseLinker l in this.curLinkers)
            {
                //MonoBehaviour.print("링커 탐색중");
                if (l.prev == this.curPhase)   // cur_LINKER 업데이트에 문제가 있어서 코드실행 안되는 듯
                {
                    //MonoBehaviour.print("해당되는 링커 찾음. 현재 페이즈 : " + this.curPhase);
                    if (l.IsTriggered() != 0)     // [PhaseLinker.additional] 때문에 ==문 필요
                    {
                        //MonoBehaviour.print("다음단계 변경됨 " + next_phase + " to " + l.next);
                        next_phase = l.next;
                    }
                    //MonoBehaviour.print("다음단계 변경 안됨 " + next_phase);

                }
            }
            
            if (next_phase != default_val)  this.curPhase = next_phase;    // PhaseLinker 작동시 해당 페이즈로
            else                            this.curPhase++;               // 미작동시 그냥 다음페이즈로 이동

            if (this.curPhase == PhaseLinker.END_SKILL || this.curPhase >= this.phases.Count)
            {
                //MonoBehaviour.print("스킬 종료요청 보냄. cur_phase == " + this.curPhase);
                this.ActInit();
                return SkillRequest.END_SKILL;                        // 존재하지 않는 페이즈로 이동시 end of skill
            }
            this.CurLinkerListInit();

            this.curTimer += this.phases[this.curPhase].motion_time;  // 타이머를 새 페이즈의 사용시간만큼 증가시킴.  TODO: 공속영향받도록 할 것
            start_req = this.phases[this.curPhase].ActInit();

            //MonoBehaviour.print("atk_obj in request. 1단계");
        }

        // work on current phase
        // check trigger of linkers with current key state
        this.curKeystate = input;
        foreach (PhaseLinker l in this.curLinkers)
            l.CheckTrigger(this.curKeystate);
        
        // Merge ActionRequests
        SkillRequest act_req = this.phases[this.curPhase].Act(curKeystate);
        SkillRequest return_ = act_req + start_req;
        if (return_.atkObjs.Count != 0) {
            MonoBehaviour.print("atk_obj in request. 2단계");
        }
        return return_;
    }
    
    public void ReduceCooltime() { this.curCoolTime -= Time.deltaTime; }

    // clear [this.cur_linker], refresh cur_linker list with []
    private void CurLinkerListInit()
    {
        this.curLinkers.Clear();
        //MonoBehaviour.print("curLinker 업데이트 시작. 총 링커 수 : " + this.phaseLinkers.Count);
        foreach (PhaseLinker l in this.phaseLinkers)
        {
            int gap = (l.prev - this.curPhase);
            //MonoBehaviour.print("갭 == " + gap);
            if (0 <= gap && gap <= l.additional)
            {  // if cur_phase in detection of PhaseLinker
                this.curLinkers.Add(l);
                //MonoBehaviour.print("curlinke에 링커 추가, 지금 시작되는 신규 페이즈 == " + this.curPhase);
                if (gap == 0)           // not in 'additional' range
                    l.ReadyNextPhase();      // it's new phase
            }
        }
    }

    public class Phase
    {
        public string motion_name           = null;                  // animation code that character must perform
        public float  motion_time           = 1f;                    // time needed for this phase
        public float  rotate_speed          = 1f;                    // whether this skill allow rotating ( 0f = not allowed, 0.5f = half speed, 1f = with original speed)
        public List<GameObject> atk_obj     = new List<GameObject>();// attack data  included in gameObject's settig
        public Vector3 movement             = Vector3.zero;    	     // forward, right direction speeds.
        public Vector3 movement_additional  = Vector3.zero;          // character's movement speed will be applied
        public float default_dir            = 360f;                  // movement_additional is affected by key input. default_dir used if there is no key pressed
        public bool? setRotateLock          = null;                  // null이면 현상유지, false면 자유상태, true면 lock걸려서 rotateSpeed에 의해서만 회전가능. 락걸리면 에임이 고정됨.

        //TODO:super amor??
        //TODO:Buff on self?

        public SkillRequest ActInit()       // phase 시작시 발생하는 Request
        {
            return new SkillRequest()
            {
                motionName = this.motion_name,
                atkObjs    = this.atk_obj
            };
        }
        public SkillRequest Act(CommandFilter.KeyInput cur_input)
        {
            return new SkillRequest()       // phase 중간에 발생하는 Request
            {
                rotateSpeed   = this.rotate_speed,
                movement      = this.TotalSpeedWitInput(cur_input),
                setRotateLock = this.setRotateLock,
                // 시스템 추가시 확장 필요
                // 어차피 복붙인데 자동화 할 방법은 없을까? C# 프로그래밍 기법중 있을것같은데
            };
        }


        Vector3 TotalSpeedWitInput(CommandFilter.KeyInput cur_input)
        {
            float dir_input = cur_input.To360Dir();
            Vector3 adi = this.AdditionalMove(dir_input);
            Vector3 sum = this.movement + adi;
            return sum;
        }

        private Vector3 AdditionalMove(float dir_input)
        {
            float ROOT2 = (Vector3.right + Vector3.forward).magnitude;
            switch ((int)dir_input)
            {
                default:
                case +360:  // no_dir
                    if (default_dir == 360)
                        return this.movement_additional * 0f;   //no dir -> can be changed by default options
                    else
                        return this.AdditionalMove(this.default_dir);
                    
                case +000: return new Vector3(0, 0, +this.movement_additional.z);
                case +180: return new Vector3(0, 0, -this.movement_additional.z);
                case +090: return new Vector3(+this.movement_additional.x, 0, 0);
                case -090: return new Vector3(-this.movement_additional.x, 0, 0);
                case +045: return new Vector3(+this.movement_additional.x, 0, +this.movement_additional.z) / ROOT2; //좀더고민해보고결정해야될듯
                case -045: return new Vector3(-this.movement_additional.x, 0, +this.movement_additional.z) / ROOT2; // ROOT2를 걍 냅둬도되려나
                case +135: return new Vector3(+this.movement_additional.x, 0, -this.movement_additional.z) / ROOT2;
                case -135: return new Vector3(-this.movement_additional.x, 0, -this.movement_additional.z) / ROOT2;
            }
        }
    }
    // Phase Linker can change order of skill phases or make loop of repeated phases
    // if no phase linker, all phase will move like 0->1->2->3->...->last_phase->end_skill
    public class PhaseLinker
    {
        [HideInInspector] public static readonly int END_SKILL = -1;

        // if activate trigger_querry while in phase[prev], as soon as phase[prev] ended, current_phase move to phase[next]
        // trigger example : ["1+" == (new press on skill key 1)] is trigger_querry, "2-" == new off on 2   (same format with CommandFilter.Command constructor)
        // only one key input allowed. but other situation(if attacked, if enemy die) can be added too.(TODO)
        [HideInInspector] public readonly int prev, next;
        [HideInInspector] public readonly string trigger_querry;

        // if there is 'additional' margin of trigger_querry
        // trigger_querry will work in range [prev -additional ~ prev] (breaking into this range with other linker is not allowed)
        // but able next move phase into [next] only if [cur_phase == prev]
        [HideInInspector] public readonly int additional;
        [HideInInspector] public readonly int recursion_max; // how many times this trigger_querry works.

        private int recursion_count = 0;
        private int triggered = 0;
        // normal  trigger_querry : "2+" , "2-", "U+"... get only one input
        // special trigger_querry :  "attacked", "hp 25%-", "true"(무조건 발동), "on_buff ~~~"
        // next = -1 == END_SKILL
        public PhaseLinker(string trigger_, int prev_, int next_ = -1, int recursion_max_ = 1, int additional_ = 0)
        {
            prev = prev_;
            next = next_;
            trigger_querry = string.Copy(trigger_);
            additional = additional_;
            recursion_max = recursion_max_;
        }

        // deactivate this liker for next phase
        public void ReadyNextPhase()
        {
            if (this.triggered > 0)
                this.recursion_count++;
            this.triggered = 0;
        }
        public void Init()
        {
            this.ReadyNextPhase();
            this.recursion_count = 0;
        }

        // 1. return whether trigger_querry is activated in [this phase]
        // 2. set trigger_querry off
        public int IsTriggered()
        {
            if (this.recursion_count == this.recursion_max)
            {
                //MonoBehaviour.print("can't activate phase_linker. 링커 최대반복횟수 도달했음");
                return 0;
            }
            if (0 < this.triggered)
            {
                //MonoBehaviour.print("IsTriggered() == " + this.triggered);
                return this.triggered;
            }
            return 0;
        }
        // 1. check whethere trigger_querry is activated at this frame
        // 2. if trigger_querry activated, this.triggered++
        // 3. return whether trigger_querry is activated [this frame]
        public bool CheckTrigger(CommandFilter.KeyInput cur_input)
        {
            bool is_triggered_now = false;
            // special trigger_querry like  "attacked", "hp 25%-", "true"(무조건 발동), "on_buff ~~~"
            // TODO
            
            if (trigger_querry == "true")
                is_triggered_now = true;
            else
            {
                //normal triger_querry check block (such as "U+" or "1-")
                CommandFilter.KeyType key    = CommandFilter.Command.ToKeyType(this.trigger_querry[0]);
                CommandFilter.KeyState state = CommandFilter.Command.ToKeyState(this.trigger_querry[1]);

                is_triggered_now = cur_input.Is(key, state);
            }
            //MonoBehaviour.print("링커 체크 결과 : " + is_triggered_now);

            if (is_triggered_now)
                this.triggered++;
            
            return is_triggered_now;
        }

    }

}
