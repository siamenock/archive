using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour { // Character Stat
    public enum Name { HP_MAX, HP, EVADE, CHASE, SPEED_MOVE, SPEED_ATK, DEF_PHYSICAL, DEF_MAGICAL, ATK_MAGICAL }
    
    // 유니티 인스펙터에서 조작가능
    [SerializeField] string[] skill_list = null;   // 
    [SerializeField]
    private float	level       = 1f,
                    hpMax		= 1000f,
					evade		= 0f,		// unit : +sec
                	chase		= 0f,		// unit : -sec
				    speed_move	= 4f,		// unit : m/sec
					speed_atk 	= 1f,		// unit : *n
                	def_phy		= 0f,		// unit : -n		physical defence
				    def_mag 	= 100f,		// unit : /n		magical defence
					atk_mag 	= 100f;     // unit : *n		magical attack
    private float   hp, // hide in inspector, 시작시 hp_max에 일치시켜줌.
                    timer_hit = 0,
                    timer_down = 0;
    private List<Effect> effects = new List<Effect>();	//상태 이상 목록

	private static readonly float EFFECTS_CHECK_INTERVAL = 0.2f;			//update period
    private static float DOWN_TIME = 1f;


    private void Awake()
    {
        if (this.GetComponent<Trace>() == null)
            this.gameObject.AddComponent<Trace>();

        if (this.GetComponent<TargetUpdater>() == null && this.GetComponent<CommandFilterWASD>() == null) { // wasd모드면 안됨
            this.gameObject.AddComponent<TargetUpdater>();
        }

        if (this.GetComponent<CharacterController>() == null)
            this.gameObject.AddComponent<CharacterController>();

        if (this.GetComponent<CharacterInteractor>() == null){
            this.gameObject.AddComponent<CharacterInteractor>();
            CharacterInteractor.TEAM team;
            switch (this.tag) {
                case "Enemy":   team = CharacterInteractor.TEAM.ENEMY;     break;
                case "Player":  team = CharacterInteractor.TEAM.PLAYER;    break;
                default:        team = CharacterInteractor.TEAM.DEFAULT;   break;
            }
            this.GetComponent<CharacterInteractor>().team = team;
        }
        
    }


    // Use this for initialization
    void Start () {
        hp = hpMax;
        InitSkillListOfCharacter.Set(this.gameObject, skill_list);  // 너무 길어서 다른 1회용 static class로 아웃소싱했어
        StartCoroutine (UpdateEffect());

	}

	public float Get(Name code){
		switch(code){
		case Name.HP_MAX:   			return hpMax;
		case Name.HP:		    	    return hp;
		case Name.EVADE:			    return evade;
		case Name.CHASE:			    return chase;
        case Name.SPEED_ATK:		    return speed_atk;
		case Name.SPEED_MOVE:			return speed_move;
		case Name.DEF_PHYSICAL:			return def_phy;
		case Name.DEF_MAGICAL:		    return def_mag;
		case Name.ATK_MAGICAL:		    return atk_mag;

		default:
			print("Stat.Get(Stat.LIST.XXXX) : not registered list called");
			return 0;
		}
	}

    public void Set(Name code, float value) {
        switch (code)
        {
            case Name.HP_MAX:           hpMax       = value;    return;
            case Name.HP:               hp          = value;    return;
            case Name.EVADE:            evade       = value;    return;
            case Name.CHASE:            chase       = value;    return;
            case Name.SPEED_ATK:        speed_atk   = value;    return; 
            case Name.SPEED_MOVE:       speed_move  = value;    return;
            case Name.DEF_PHYSICAL:     def_phy     = value;    return;
            case Name.DEF_MAGICAL:      def_mag     = value;    return;
            case Name.ATK_MAGICAL:      atk_mag     = value;    return;

            default:
                break;
        }
        print("Stat.Set(Stat.LIST.XXXX, value) : not registered name called");
    }

    public void GetDamageBy(Attack attack) {
        float damage_physical = attack.damage_physical - this.Get(Stat.Name.DEF_PHYSICAL);

        if (damage_physical < 0)   // 이런 경우 가끔 있음. 데미지 없이 경직만 있는 걸로 처리함.
            return;

        // [(공격자 마법공격력)/(내 마법방어력)] 만큼 데미지 증감
        float damage_amplify = attack.attacker.GetComponent<Stat>().Get(Stat.Name.ATK_MAGICAL) / this.Get(Stat.Name.DEF_MAGICAL);
        float damage_total = damage_physical * damage_amplify;

        this.hp -= damage_total;
        DamageShow.MakeUI(this.gameObject, (int)damage_total);
    }

	//======================================================================//
	//								EFFECTS                                 //
	//======================================================================//
	IEnumerator UpdateEffect () {
		while(true){
			yield return new WaitForSeconds (EFFECTS_CHECK_INTERVAL);
			for(int i = 0; i < effects.Count; i++){
				effects[i].ReduceTime (EFFECTS_CHECK_INTERVAL);
				if (effects [i].GetTime () < 0) {
					// TODO : stat 정상화(def down 등)
					print("on effect");
					effects.RemoveAt (i--); 
				} else {
					// TODO : activate effects applies each sec( blooding, poison...  )
				}
			}
		}
	}


	public void AddEffect(Effect e){
		EffectInitApply (e);		//Apply On Stat
		effects.Add (e);	//add in list
	}


	public void EffectInitApply(Effect e){
		switch(e.GetTypeOfEffect()){
		case Effect.TYPE.DOWN_EVADE:
			evade -= e.GetAmount ();
			break;

			//and other cases to add
		}
	}
	public void RemoveEffect(Effect e){
		int i = effects.IndexOf (e);
		RemoveEffect (i);
	}
	public void RemoveEffect(int index){	//private??
		Effect e = effects [index];
		effects.RemoveAt(index);
		e.SetAmount ( - e.GetAmount());
		EffectInitApply (e);
	}

    public float CalcEvadeDelay2Enemy(GameObject target)
    {
        Stat targetStat = target.GetComponent<Stat>();
        float delay = 0;
        if (targetStat)
        {
            delay = targetStat.Get(Stat.Name.EVADE) - this.Get(Stat.Name.CHASE);
            if (delay < 0)
                return 0;
        }
        return delay;
    }
    public Vector3 CalcEvadeDelayPos2Enemy(GameObject target) {
        Trace enemy_trace = target.GetComponent<Trace>();
        if (enemy_trace)
            return enemy_trace.PosB4(this.CalcEvadeDelay2Enemy(target));
        return target.transform.position;
    }
}
