using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// 기본AI는 아래와 같은 기능을 포함함. 
// [기능 1] 쿨타임 되는대로 가장 가까운 적에게 스킬난사
// [기능 2] 스킬사용 중이 아니라면 기본 무빙으로 공격을 회피 (도주용 스킬사용은 미구현)
// [기능 3] 가까운 적에게 계속 접근.
public class AI : MonoBehaviour
{
    public enum Mode { ATTACK, MOVE, EVADE};
    protected WarningSensorSystem warning_sensor        = null;
    protected CharacterInteractor character_interactor  = null;
    protected CharacterController character_controller  = null;
    protected TargetUpdater       target_updater	    = null;
    protected List<Skill>         skills                = null;   // character_controller.skills와 같은 포인터를 가리킴
    protected GameObject          main_target           = null;
    
    protected readonly int        evade_range       = 2;     // 캐릭터사이즈 * n 범위 내에 공격이 들어와야 반응함.
    
    protected Mode mode = Mode.MOVE;

    // MOVE     state
    protected int leftRightDir = 1;    
    // EVADE    state
    protected Vector3 evade_pos;
    // ATTACK   state


    //=============================================================================//
    //            상속받은 class들이 공통적으로 사용할 기본재료들                  //
    //=============================================================================//
    static protected readonly int NO_COMMAND = -1;
    protected static readonly CommandFilter.KeyInput INPUT_NOTHING = new CommandFilter.KeyInput();
    protected static readonly CommandFilter.KeyInput INPUT_UP = CommandFilter.KeyInput.WithOn(CommandFilter.KeyType.UP);
    protected static readonly CommandFilter.KeyInput INPUT_DOWN = CommandFilter.KeyInput.WithOn(CommandFilter.KeyType.DOWN);
    protected static readonly CommandFilter.KeyInput INPUT_RIGHT = CommandFilter.KeyInput.WithOn(CommandFilter.KeyType.RIGHT);
    protected static readonly CommandFilter.KeyInput INPUT_LEFT = CommandFilter.KeyInput.WithOn(CommandFilter.KeyType.LEFT);
    protected static readonly CommandFilter.KeyInput INPUT_SK1 = CommandFilter.KeyInput.WithOn(CommandFilter.KeyType.SK1);
    protected static readonly CommandFilter.KeyInput INPUT_SK2 = CommandFilter.KeyInput.WithOn(CommandFilter.KeyType.SK2);
    protected static readonly CommandFilter.KeyInput INPUT_SK3 = CommandFilter.KeyInput.WithOn(CommandFilter.KeyType.SK3);



    protected virtual void Start()
    {
        target_updater      = this.GetComponent<TargetUpdater>();
        character_interactor = this.GetComponent<CharacterInteractor>();
        character_controller = this.GetComponent<CharacterController>();
        skills = this.character_controller.skills;
        warning_sensor = this.gameObject.AddComponent<WarningSensorSystem>();
        warning_sensor.Set1stRadius(evade_range);
    }
    void Update()
    {
        
        SetTarget();
        SetState();
        SetDirection();                                              // 타겟전환, 위험감지 등으로 캐릭터의 방향전환

        int command_code = this.ChooseSkill2Start();
        if (command_code != NO_COMMAND)
            character_controller.ActivateCommand(command_code);

        CommandFilter.KeyInput input = this.ChooseKeyInput();
        character_controller.UpdateKeystate(input);
    }

    protected virtual void SetTarget()
    {
        this.main_target = target_updater.GetMainTarget();
    }

    protected virtual void SetState()
    {
        //======================================================================//
        //                              evade -> move                           //
        // 조건 : evade_pos에 도착 또는 이미 맞아서 피격상태
        if (this.mode == Mode.EVADE)
        {
            if (character_controller.GetState() == CharacterController.State.hit ||
                character_controller.GetState() == CharacterController.State.down)
            {
                this.mode = Mode.MOVE;
            }
            else
            {
                Vector3 gap = (this.transform.position - this.evade_pos);
                gap.y = 0;
                float distance = gap.magnitude;

                if (distance < 0.1)
                    this.mode = Mode.MOVE;
            }
        }
        //======================================================================//
        //                             attack -> move                           //
        // 조건 : 스킬사용 종료되거나 스킬 끊김
        if (this.mode == Mode.ATTACK)
            if (this.character_controller.GetState() < 0)   // 스킬 사용중이 아님
                this.mode = Mode.MOVE;

        //======================================================================//
        //                              all -> evade                            //
        // 조건 : 현재 내 위치가 위험한 경우.
        //        또한 이미 evade인데 evade_pos가 위험하면 새 evade_pos할당
        if (this.mode == Mode.EVADE)
        {
            bool evade_pos_is_dangerous = !warning_sensor.IsSafe(evade_pos);
            if (evade_pos_is_dangerous)
                evade_pos = warning_sensor.GetNearSafePos();
        }
        else if (warning_sensor.IsNearbyWarning()) {
            if (warning_sensor.AmIDangerous())
            {
                this.mode = Mode.EVADE;
                evade_pos = warning_sensor.GetNearSafePos();
            }
        }

        //======================================================================//
        //                              move -> attack                          //
        // 조건 : main_target에게 사용가능한 스킬 존재 (사거리, 쿨타임 제한됨) -> 사거리 미구현
        if (this.mode == Mode.MOVE) {
            foreach (Skill s in skills) {
                if (s.curCoolTime <= 0) {
                    this.mode = Mode.ATTACK;
                    break;
                }
            }
        }
    }

    private void SetDirection()
    {
        switch (this.mode) {
            default:
            case Mode.MOVE:
                SetDirectionStateMove();
                return;
            case Mode.EVADE:
                SetDirectionStateEvade();
                return;
            case Mode.ATTACK:
                SetDirectionStateAttack();
                return;
        }
    }
    protected virtual int ChooseSkill2Start()
    {
        if (this.mode != Mode.ATTACK)
            return NO_COMMAND;
        
        for (int i = 0; i < skills.Count; i++) {
            if(skills[i].curCoolTime <= 0)
                return i;
        }
        return NO_COMMAND;
    }
    private CommandFilter.KeyInput ChooseKeyInput()
    {
        switch (this.mode)
        {
            default:
            case Mode.MOVE:
                return ChooseKeyInputStateMove();
            case Mode.EVADE:
                return ChooseKeyInputStateEvade();
            case Mode.ATTACK:
                return ChooseKeyInputStateAttack();
        }
    }

    protected virtual void SetDirectionStateMove  () { LookMainTarget();        }
    protected virtual void SetDirectionStateEvade () { LookEvadePos();          }
    protected virtual void SetDirectionStateAttack() { SetDirectionStateMove(); }


    protected virtual CommandFilter.KeyInput ChooseKeyInputStateMove()
    {
        float distance = (main_target.transform.position - this.transform.position).magnitude;
        if (distance > 10)
            return INPUT_UP;
        else
        {
            if (leftRightDir < 0)
                return INPUT_LEFT;
            else
                return INPUT_RIGHT;
        }
    }
    protected virtual CommandFilter.KeyInput ChooseKeyInputStateEvade()  { return INPUT_UP; }
    protected virtual CommandFilter.KeyInput ChooseKeyInputStateAttack() { return ChooseKeyInputStateMove(); }


    protected void LookMainTarget()
    {
        character_controller.TurnSmoothToward(main_target);
    }
    protected void LookEvadePos()
    {
        character_controller.TurnSmoothToward(evade_pos);
    }
}
