using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : use XML
// TODO : let Skill.Phase, Skill.PhaseLinker attribute 'readonly'
public class SkillListGlobalInitialize : MonoBehaviour
{
    GameObject  NA_movingshot_ATK0 = null,
                NA_movingshot_AI_ATK0 = null,
                SK_spin_kick_ATK0 = null,
                SK_MissleSpread_ATK0 = null;
        
    
    // Use this for initialization. 

    void Awake()
    {
        NA_movingshot_ATK0      = Resources.Load("AtkObj/NA_movingshot_ATK0"   ,    typeof(GameObject)) as GameObject;
        NA_movingshot_AI_ATK0   = Resources.Load("AtkObj/NA_movingshot_AI_ATK0",    typeof(GameObject)) as GameObject;
        SK_spin_kick_ATK0       = Resources.Load("AtkObj/SK_SpinKick_ATK0",         typeof(GameObject)) as GameObject;
        SK_MissleSpread_ATK0    = Resources.Load("AtkObj/SK_MissleSpread_ATK0",    typeof(GameObject)) as GameObject;

        bool success = PresetSetting();
        if (!success)
        {
            print("FATAL ERROR\nSkillListGlobalInitialize.presetSetting() initialization failed!");
            return;
        }

        return;

    }

    private bool PresetSetting()
    {
        Skill.preset.Clear();
        Skill.precmd.Clear();
        try
        {
            { // normal attack : movingshot.
                Skill skill = new Skill
                {
                    //이거도 좀 바꾸자.
                    name = "NA_movingshot",
                    constCoolTime = 0.3f
                };

                for (int i = 0; i < 6; i++) { skill.phases.Add(new Skill.Phase()); }

                skill.phases[0].motion_time = 0.50f;    // pre  delay	//bullet fired as phase1 start
                skill.phases[1].motion_time = 0.30f;    // mid  delay
                skill.phases[2].motion_time = 0.30f;    // mid  delay
                skill.phases[3].motion_time = 0.1f;    // post delay
                skill.phases[4].motion_time = 0.1f;    // 1,2 페이즈에서 끊을 경우 추가 딜레이 넣는 또 다른 post delay phase 필요
                skill.phases[5].motion_time = 0.3f;    // 1,2 페이즈에서 끊을 경우 추가 딜레이 넣는 또 다른 post delay phase 필요

                Vector3 speed = (Vector3.right + Vector3.forward) * 0.7f;   // 70% speed moving speed while attacking. Same speed for all phase
                skill.phases[0].movement_additional = speed;
                skill.phases[1].movement_additional = speed;
                skill.phases[2].movement_additional = speed;
                skill.phases[3].movement_additional = speed;
                skill.phases[4].movement_additional = speed;
                skill.phases[5].movement_additional = speed * 0;

                skill.phases[0].atk_obj = new List<GameObject>();
                skill.phases[1].atk_obj = new List<GameObject>();
                skill.phases[2].atk_obj = new List<GameObject>();
                skill.phases[3].atk_obj = new List<GameObject>();
                skill.phases[4].atk_obj = new List<GameObject>();
                skill.phases[1].atk_obj.Add(NA_movingshot_ATK0);            // hard coding each gameobj in unity inspector is bad idea
                skill.phases[2].atk_obj.Add(NA_movingshot_ATK0);            // hard coding each gameobj in unity inspector is bad idea
                skill.phases[3].atk_obj.Add(NA_movingshot_ATK0);            // 막타공격은 다른걸로 바꿔야 함
                skill.phases[4].atk_obj.Add(NA_movingshot_ATK0);            // 막타공격은 다른걸로 바꿔야 함

                skill.phaseLinkers = new List<Skill.PhaseLinker>();
                //skill.phaseLinkers.Add(new Skill.PhaseLinker("true", 2, Skill.PhaseLinker.END_SKILL, 99));   // 다른 트리거가 발동하지 않았다면 페이즈 2 종료시 스킬 종료

                CommandFilter.Command cmd = new CommandFilter.Command("2+ dont_care_off");

                Skill.preset.Add(skill.name, skill);
                Skill.precmd.Add(skill.name, cmd);

            }

            { // normal attack : movingshot, 테스트를 위해 딱 1발만 나가는 공격으로 만들었음
                Skill skill = new Skill
                {
                    name = "NA_movingshot_attack_test",
                    constCoolTime = 1f
                };

                for (int i = 0; i < 1; i++) { skill.phases.Add(new Skill.Phase()); }

                skill.phases[0].motion_time = 0.10f;    // pre  delay	//bullet fired as phase1 start

                Vector3 speed = (Vector3.right + Vector3.forward) * 0.7f;   // 70% speed moving speed while attacking. Same speed for all phase
                skill.phases[0].movement_additional = speed;


                skill.phases[0].atk_obj = new List<GameObject>();
                skill.phases[0].atk_obj.Add(NA_movingshot_ATK0);            // 막타공격은 다른걸로 바꿔야 함
                skill.phaseLinkers = new List<Skill.PhaseLinker>();

                CommandFilter.Command cmd = new CommandFilter.Command("2+ dont_care_off");

                Skill.preset.Add(skill.name, skill);
                Skill.precmd.Add(skill.name, cmd);

            }

            { // normal attack :  AI용은 쿨타임이 좀더 김
                Skill skill = new Skill
                {
                    name = "NA_movingshot_AI",
                    constCoolTime = 0.4f
                };

                for (int i = 0; i < 1; i++) { skill.phases.Add(new Skill.Phase()); }

                skill.phases[0].motion_time = 0.50f;    // pre  delay	//bullet fired as phase1 start

                Vector3 speed = (Vector3.right + Vector3.forward) * 0.7f;   // 70% speed moving speed while attacking. Same speed for all phase
                skill.phases[0].movement_additional = speed;


                skill.phases[0].atk_obj = new List<GameObject>();
                skill.phases[0].atk_obj.Add(NA_movingshot_AI_ATK0);
                skill.phaseLinkers = new List<Skill.PhaseLinker>();

                CommandFilter.Command cmd = new CommandFilter.Command("2+ dont_care_off");

                Skill.preset.Add(skill.name, skill);
                Skill.precmd.Add(skill.name, cmd);

            }

            {
                Skill skill = Skill.preset["NA_movingshot_attack_test"].DeepClone();
                skill.name = "SK_gettling";
                skill.constCoolTime = .1f;

                CommandFilter.Command cmd = new CommandFilter.Command("2+ dont_care_off");

                Skill.preset.Add(skill.name, skill);
                Skill.precmd.Add(skill.name, cmd);
            }

            {
                Skill skill = new Skill
                {
                    name = "SK_SpreadMissle",           //TODO
                    constCoolTime = 0.4f
                };

                for (int i = 0; i < 4; i++) { skill.phases.Add(new Skill.Phase()); }

                skill.phases[0].motion_time = 0.50f;    // pre  delay
                skill.phases[1].motion_time = 0.10f;    // mid  delay
                skill.phases[2].motion_time = 0.10f;    // mid  delay
                skill.phases[3].motion_time = 0.50f;    // post delay

                
                skill.phases[1].atk_obj = new List<GameObject>();
                skill.phases[2].atk_obj = new List<GameObject>();
                
                skill.phases[1].atk_obj.Add(SK_MissleSpread_ATK0);
                skill.phases[2].atk_obj.Add(SK_MissleSpread_ATK0);
                skill.phases[3].atk_obj.Add(SK_MissleSpread_ATK0);

                skill.phaseLinkers = new List<Skill.PhaseLinker>();

                CommandFilter.Command cmd = new CommandFilter.Command("3+ dont_care_off");

                Skill.preset.Add(skill.name, skill);
                Skill.precmd.Add(skill.name, cmd);

            }

            { // Skill : evade
                Skill skill = new Skill
                {
                    name = "SK_evade_forward",
                    constCoolTime = 0.2f
                };
                for (int i = 0; i < 3; i++) { skill.phases.Add(new Skill.Phase()); }
                // no pre-delay
                skill.phases[0].motion_time = 0.5f; // phase0 (fastest move)
                skill.phases[1].motion_time = 0.2f; // phase1 (slower move)
                skill.phases[2].motion_time = 0.45f; // post delay

                Vector3 speed = (Vector3.forward);              // 50% slowed moving speed while attacking. Same speed for all phase
                skill.phases[0].movement = speed * 1.8f;
                skill.phases[1].movement = speed * 1f;
                skill.phases[2].movement = speed * 0.2f;

                CommandFilter.Command cmd = new CommandFilter.Command("U+|U-|U+|U- check_key_off no_key_allowed");

                Skill.preset.Add(skill.name, skill);
                Skill.precmd.Add(skill.name, cmd);
            }
            {
                Skill skill = Skill.preset["SK_evade_forward"].DeepClone();
                skill.name = "SK_evade_back";

                Vector3 speed = (-Vector3.forward);             // 50% slowed moving speed while attacking. Same speed for all phase
                skill.phases[0].movement = speed * 1.8f;
                skill.phases[1].movement = speed * 1f;
                skill.phases[2].movement = speed * 0.2f;

                CommandFilter.Command cmd = new CommandFilter.Command("D+|D-|D+|D- check_key_off no_key_allowed");

                Skill.preset.Add(skill.name, skill);
                Skill.precmd.Add(skill.name, cmd);
            }
            {
                Skill skill = Skill.preset["SK_evade_forward"].DeepClone();
                skill.name = "SK_evade_right";

                Vector3 speed = (Vector3.right);                // 50% slowed moving speed while attacking. Same speed for all phase
                skill.phases[0].movement = speed * 1.8f;
                skill.phases[1].movement = speed * 1f;
                skill.phases[2].movement = speed * 0.2f;

                CommandFilter.Command cmd = new CommandFilter.Command("R+|R-|R+|R- check_key_off no_key_allowed");

                Skill.preset.Add(skill.name, skill);
                Skill.precmd.Add(skill.name, cmd);
            }
            {
                Skill skill = Skill.preset["SK_evade_forward"].DeepClone();
                skill.name = "SK_evade_left";

                Vector3 speed = (-Vector3.right);               // 50% slowed moving speed while attacking. Same speed for all phase
                skill.phases[0].movement = speed * 1.8f;
                skill.phases[1].movement = speed * 1f;
                skill.phases[2].movement = speed * 0.2f;

                CommandFilter.Command cmd = new CommandFilter.Command("L+|L-|L+|L- check_key_off no_key_allowed");

                Skill.preset.Add(skill.name, skill);
                Skill.precmd.Add(skill.name, cmd);
            }
            {

            }
            { // normal attack : movingshot.
                Skill skill = new Skill
                {
                    name = "SK_spin_kick",
                    constCoolTime = .2f
                };


                for (int i = 0; i < 5; i++) { skill.phases.Add(new Skill.Phase()); }

                skill.phases[0].motion_time = 0.2f; //pre delay	
                skill.phases[1].motion_time = 0.2f;
                skill.phases[2].motion_time = 0.2f;
                skill.phases[3].motion_time = 0.2f; //mid delay
                skill.phases[4].motion_time = 0.7f; //postdelay

                skill.phases[0].setRotateLock = true;
                skill.phases[4].setRotateLock = false;

                Vector3 dir = (Vector3.forward);
                skill.phases[1].movement = dir * 4f;
                skill.phases[2].movement = dir * 4f;
                skill.phases[3].movement = dir * 0.7f;
                skill.phases[4].movement = dir * 0.2f;

                dir = Vector3.forward + Vector3.right;

                skill.phases[1].movement_additional = dir * skill.phases[1].movement.magnitude * +1f;
                skill.phases[2].movement_additional = dir * skill.phases[2].movement.magnitude * +1f;
                skill.phases[2].movement_additional.x *= -1;
                skill.phases[3].movement_additional = skill.phases[3].movement * -1f;
                skill.phases[4].movement_additional = skill.phases[4].movement * -1f;


                skill.phases[3].atk_obj = new List<GameObject>();
                skill.phases[4].atk_obj = new List<GameObject>();
                skill.phases[3].atk_obj.Add(SK_spin_kick_ATK0);
                skill.phases[4].atk_obj.Add(SK_spin_kick_ATK0);



                CommandFilter.Command cmd = new CommandFilter.Command("U+|U+|1+ dont_care_off");

                Skill.preset.Add(skill.name, skill);
                Skill.precmd.Add(skill.name, cmd);
            }

            // phase[0] 's request include skill_name (motion name)
            foreach (KeyValuePair<string, Skill> p in Skill.preset)
            {
                p.Value.phases[0].motion_name = p.Value.name;
            }
        }
        catch (System.Exception e)
        {
            print(e.ToString());
            return false;
        }
        return true;
    }
    static void PrintSkillDictionary(Dictionary<string, Skill> d)
    {
        string str = "";
        foreach (var pair in d)
        {
            str += pair.Value.name + "\t\twith " + pair.Value.phases.Count + "phases" + "\n";
        }
        print("Skill.presetSetting() initialization -> total " + Skill.preset.Count + ", " + Skill.precmd.Count + " Skill.preset, Skill.precmd saved\n" + str);
    }
}
