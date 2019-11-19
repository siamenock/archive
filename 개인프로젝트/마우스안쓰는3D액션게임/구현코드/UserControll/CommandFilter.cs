using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CommandFilter : MonoBehaviour {
    //Key Code
    //todo : extract these codes into header file
    public enum KeyType  { UP = 1, DOWN, RIGHT, LEFT, SK1, SK2, SK3, KEYTYPE_COUNT };
    public enum KeyState { ON = 1, NEW_ON = 2, OFF = -1, NEW_OFF = -2, ERROR = 0 };
    static readonly string[] KEY_NAMES = { "not_used", "Keypad8", "Keypad5", "Keypad6", "Keypad4", "Skill1", "Skill2", "Skill3" };      //total length == signal (0 not used)
    [HideInInspector] public const int  UP	            = (int) KeyType.UP,
                                        DOWN	        = (int) KeyType.DOWN,
	                                    RIGHT           = (int) KeyType.RIGHT,
	                                    LEFT	        = (int) KeyType.LEFT,
	                                    SK1 	        = (int) KeyType.SK1,
	                                    SK2	            = (int) KeyType.SK2,
	                                    SK3	            = (int) KeyType.SK3,
	                                    KEYTYPE_COUNT   = (int) KeyType.KEYTYPE_COUNT; //not a key input. used as counter & new_state signal

    //Keystate Code
    [HideInInspector] public const int  IDLE     = 0,
                                        ON       = (int) KeyState.ON,
                                        OFF      = (int) KeyState.OFF,
                                        NEW_ON   = (int) KeyState.NEW_ON,
                                        NEW_OFF  = (int) KeyState.NEW_OFF;

    [HideInInspector] public const float NO_DIR = 360f;
    [HideInInspector] public KeyInput keyinput { get; private set; }
    [HideInInspector] public Command[] cmd_list = new Command[0];       //이건 setSkill에서 초기화 해줌
    
    // 형제 Component 포인터
    private CharacterController characterController;

    //if no new input for (maxgap), all command history initialized
    private const float TIME_MAX_GAP = 0.3f;
    private float       time_deadline = 0f;

    //history of input summery(only 1 KEYSTATE CODE(int) saved)
	private LinkedList<int> history = new LinkedList<int>();

	
    void Start () {
		characterController = GetComponent<CharacterController> ();
        keyinput = new KeyInput();
    }
		
	void Update(){
		keyinput.keystates [UP]		= Input.GetButton ("Keypad8")? ON:OFF;
		keyinput.keystates [DOWN]	= Input.GetButton ("Keypad5")? ON:OFF;
		keyinput.keystates [RIGHT]	= Input.GetButton ("Keypad6")? ON:OFF;
		keyinput.keystates [LEFT]	= Input.GetButton ("Keypad4")? ON:OFF; //unexpected error : can't get 4 dirs at once. but not important now
		keyinput.keystates [SK1]	= Input.GetButton ("Skill1" )? ON:OFF;
		keyinput.keystates [SK2]	= Input.GetButton ("Skill2" )? ON:OFF;
		keyinput.keystates [SK3]	= Input.GetButton ("Skill3" )? ON:OFF;


		//save new changes in history
		bool no_change = true;
		for(int i = UP; i < KEYTYPE_COUNT; i++)
        {
            if (Input.GetButtonDown(KEY_NAMES[i]))
            { // if button state changed (0 < on * on, same for off too)
                HistoryAdd(ON * i);                         //save change
                keyinput.keystates[i] = NEW_ON;
            }
            else if (Input.GetButtonUp(KEY_NAMES[i]))
            { // if button state changed (0 < on * on, same for off too)
                HistoryAdd(OFF * i);                        //save change
                keyinput.keystates[i] = NEW_OFF;
            }
            else
            { // if nothing changed
                continue;
            }
              // if any change detected
            no_change = false;                          //report change
            int pattern = CommandSearch();
            if (pattern >= 0)    // else pattern not found
                characterController.ActivateCommand(pattern);
        }

		if (no_change && time_deadline < Time.time) { //if input not change for more than command time gap(ex 0.3 sec)
			HistoryInit ();								//remove all history
		}
		characterController.UpdateKeystate (keyinput);	

	}

	void HistoryInit(){
		time_deadline = Time.time + TIME_MAX_GAP;
		history.Clear ();
	}
	void HistoryAdd(int summery_code){
		time_deadline = Time.time + TIME_MAX_GAP;
		//if (history.First != null && history.First.Value == idle) {history.RemoveFirst (); } //outdated code
		history.AddFirst(summery_code);
	}

	int CommandSearch(){
		for(int i = 0; i <cmd_list.Length; i++){
			if (CommandMatch (cmd_list [i])) {
				//if pattern ends with skill keys, inititalize (prevent duplicated check)
				if(    history.First.Value == SK1 * ON
					|| history.First.Value == SK2 * ON
					|| history.First.Value == SK3 * ON) {
						HistoryInit ();
				}
				return cmd_list[i].index;
			}
        }
		return -1;
	}

	//match history with pattern.  if same, return true
	bool CommandMatch(Command cmd){
		bool checkdown = cmd.check_off;
		LinkedListNode<int> it;
		int i;
		for (i = cmd.cmd.Length - 1, it = history.First; it != null; i--, it = it.Next) {
			while(!checkdown && it.Value < 0){
				it = it.Next;
				if (it == null) {break;}
			}                                       // down key ignore
			if (it == null) {break;}                // down key ignore
			if (cmd.cmd[i] != it.Value) {break;}    // cmd check
			if (i == 0) {                           // ??
				if (cmd.no_key_allowed) {
					foreach (int is_pressed in keyinput.keystates) {
						if (0 < is_pressed) {
							print ("COMMAND OPTION : NO KEY ALLOWED 에 의 커맨드 무효");
							return false;
						}
					}
				}
				return true;//pattern fully match
			}
		}
		return false;		//pattern not match
	}

	float ConvertTo360dir(int[] input){
		int move_v	= 0, move_h	= 0;
		//(v,h)  = (Back-Forward, Left-Right) = (vertical, horizental)
		if (0 < input[UP	]) { move_v++;}
		if (0 < input[DOWN	]) { move_v--;}
		if (0 < input[RIGHT	]) { move_h++;}
		if (0 < input[LEFT	]) { move_h--;}

		switch (move_v * 10 + move_h) {
		case +10:	return    0f;	//↑
		case -10:	return +180f;	//↓
		case +01:	return + 90f;	//→
		case -01:	return - 90f;	//←
		case +11:	return + 45f;	//↗
		case +09:	return - 45f;	//↖
		case -11:	return -135f;	//↙
		case -09:	return +135f;	//↘
		default:
		case 000:	return NO_DIR;	//no input. return 360
		}

	}
        
	public class Command{
        public int index = 0;

        public bool		check_off;
		public int[]	cmd;
		public bool     no_key_allowed;

        // 맨 앞에 커멘드 적고, 뒤에 옵션 붙임
        // 예시 : "2+|2+ dont_care_off" -> 2번키 2번 누르는거 감지, 나머지 키는 신경쓰지 않음.
        public Command(string str_cmd){
			string[] tokens = str_cmd.Split(' ');
            string[] cmd_str = tokens[0].Split('|');
            
            cmd = new int[cmd_str.Length];

            // 커멘드 입력
            for (int i = 0; i < cmd_str.Length; i++)
            {                                                       // for example tokens[i] = "U+" becomes
                int key   = (int)ToKeyType(cmd_str[i][0]);          // UP       (up key)
                int state = (int)ToKeyState(cmd_str[i][1]) / NEW_ON;// NEW_ON   (pressed now)

                cmd[i] = key * state;
            }

            // 옵션부분
			foreach(string t in tokens){
                switch (t)
                {
                    case "no_key_allowed":
                        no_key_allowed = true;
                        break;

                    case "check_key_off":
                        check_off = true;
                        break;
                    case "dont_care_off":
                        check_off = false;
                        break;

                    default:
                        break;
                }
            }

			
		}

        public override string ToString(){	// for debug
			string str = "";
            str += "\ncheck off\t= " + check_off;
            str += "\nno_key_allowed\t= " + no_key_allowed;
            str += "\ncmd\t= ";
			foreach (int c in cmd) {
				str += c + " ";
			}
            str += "\n    with index = " + index;
			return str;
		}
        public static KeyType ToKeyType(char c)
        {
            switch (c)
            {
                case 'U': return KeyType.UP;
                case 'D': return KeyType.DOWN;
                case 'R': return KeyType.RIGHT;
                case 'L': return KeyType.LEFT;
                case '1': return KeyType.SK1;
                case '2': return KeyType.SK2;
                case '3': return KeyType.SK3;
                default: return KeyType.KEYTYPE_COUNT;
            }
        }
        public static KeyState ToKeyState(char c)
        {
            switch (c)
            {
                case '+': return KeyState.NEW_ON;
                case '-': return KeyState.NEW_OFF;
                case 'o': return KeyState.ON;
                case 'x': return KeyState.OFF;
                default: return KeyState.ERROR;
            }
        }

    };

    public class KeyInput {
        public int[] keystates;     // 파라미터 이거 하나로 끝
        public KeyInput() { keystates = new int[KEYTYPE_COUNT] { OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF }; }
        public static KeyInput WithOn(KeyType on_key) {
            KeyInput ret = new KeyInput();
            ret.keystates[(int)on_key] = ON;
            return ret;
        }

        public static KeyInput operator + (KeyInput a, KeyInput b) {
            if (a == null) return b;
            if (b == null) return a;
            KeyInput ret = new KeyInput();
            for (int i = 0; i < KEYTYPE_COUNT; i++)
            {
                int asig = a.keystates[i];
                int bsig = b.keystates[i];

                if (asig == OFF)        ret.keystates[i] = bsig;
                else if (bsig == OFF)   ret.keystates[i] = asig;
                else {
                    if (asig == NEW_ON || bsig == NEW_ON)           ret.keystates[i] = NEW_ON;
                    else if (asig == NEW_OFF || bsig == NEW_OFF)    ret.keystates[i] = NEW_OFF;
                    else                                            ret.keystates[i] = ON;
                }
            }
            return ret;
        }

        public bool Is(KeyType type, KeyState state) {
            switch ((int) state) {
                case ON:      return keystates[(int)type] > 0;
                case OFF:     return keystates[(int)type] < 0;
                case NEW_ON:  return keystates[(int)type] == NEW_ON;
                case NEW_OFF: return keystates[(int)type] == NEW_OFF;
                default:      return false;
            }
        }

		public float To360Dir(){
			int move_v	= 0, move_h	= 0;
			//(v,h)  = (Back-Forward, Left-Right) = (vertical, horizental)
			if (0 < keystates[UP	]) { move_v++;}
			if (0 < keystates[DOWN	]) { move_v--;}
			if (0 < keystates[RIGHT	]) { move_h++;}
			if (0 < keystates[LEFT	]) { move_h--;}

			switch (move_v * 10 + move_h) {
			case +10:	return    0f;	//↑
			case -10:	return +180f;	//↓
			case +01:	return + 90f;	//→
			case -01:	return - 90f;	//←
			case +11:	return + 45f;	//↗
			case +09:	return - 45f;	//↖
			case -11:	return -135f;	//↙
			case -09:	return +135f;	//↘
			default:
			case 000:	return NO_DIR;	//no input. return 360
			}

		}
		public Quaternion ToQuaternion(){ return Quaternion.Euler(0, To360Dir(), 0); }

      

        
    }
}
