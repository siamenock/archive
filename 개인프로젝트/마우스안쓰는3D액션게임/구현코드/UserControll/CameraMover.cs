using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
	// for command filtering
	float		time_r = 0f,	// right, left를 누른 지 얼마나 오래됐는지 기록(더블클릭 확인용)
				time_l = 0f;
	float 		TIME_GAP_MAX	= 0.4f;	//더블클릭기준시간
	//r ,l		: right, left key is PRESSED state!
	//rr, ll	: 연속 2번 입력 (double click)
	//rn, ln	: new입력(최초1회 누름)
	const int	rn = +3,	rr	= +2,	r = +1, nothing = 00,
				ln = -3,	ll	= -2,	l = -1;

	//for camera rotating
	const float SPEED_ROTATE 	        = 90f,  // 일반회전 속도
				SPEED_ROTATE_EMERGENCY	= 500f,	// 급속회전 속도
	            EMERGENCY_TURN	        = 90f;  // 급속회전 각도
	Quaternion	angle_start, angle_toward;		// 급속회전 시 정해진 각도 까지만 회전하기 위해 사용
	bool		emergency_mode		= false;	// 급속회전 중인지 여부

	//for lock on mode on enemy
	bool lock_on_mode = false;		//락온 모드 여부
	
    TargetUpdater target_updater;
    
    public void SetLockOnMode(bool is_on) { lock_on_mode = is_on; }
    public bool IsLockOnMode() { return lock_on_mode; }
    private bool IsEmregencyMode() { return emergency_mode; }

    // Use this for initialization
    void Start () {
		//target_manager = transform.parent.gameObject.GetComponent<PlayerTargetManager>();
        target_updater = transform.parent.gameObject.GetComponent<TargetUpdater>();
        angle_start = this.transform.rotation;
    }

	// Update is called once per frame
	void Update () {					//filtering out INPUT COMMAND
		ProcessRotateInput();
        ProcessCapsLockAndTabInput();
	}
    //get input for camera moving (numpad 7, 9)
    void ProcessRotateInput()
    {
        // dl 인풋 타입 확인해서 다른애한테 넘기는 함수임
        int cmd_code = nothing;
        if (Input.GetButtonDown("Keypad9"))
        {
            float gap = Time.time - time_r;
            time_r = Time.time;
            if (gap < TIME_GAP_MAX)
                cmd_code = rr;
            else
                cmd_code = rn;
        }
        else if (Input.GetButtonDown("Keypad7"))
        {
            float gap = Time.time - time_l;
            time_l = Time.time;
            if (gap < TIME_GAP_MAX)
                cmd_code = ll;  // double clcik
            else
                cmd_code = ln;  // single

        }
        else if (Input.GetButton("Keypad9"))
            cmd_code = r;
        else if (Input.GetButton("Keypad7"))
            cmd_code = l;

        ExcuteCommand(cmd_code);

    }

	void ExcuteCommand(int cmd_code){
		if (IsLockOnMode ()) ExcuteCommand_LockOnMode (cmd_code);
		else				 ExcuteCommand_NormalMode (cmd_code);
	}

    void ProcessCapsLockAndTabInput() {
        if (Input.GetKeyDown("caps lock"))
        {
            bool current_mode = IsLockOnMode();
            GetComponent<CameraMover>().SetLockOnMode(!current_mode);
        }

        if (Input.GetKeyDown("tab"))
        {
            target_updater.ChangeTargetToNext();
        }
    }	

	void ExcuteCommand_NormalMode(int cmd_code){		//this function called every frame
		if (emergency_mode)
        {
            if (cmd_code == rr || cmd_code == ll)
                SetEmergencyRotate(cmd_code, angle_start);
            ExcuteEmergencyRotate();
            return;
        }
        else
            switch (cmd_code)
            {
                case rn:
                    angle_start = transform.rotation;       //set starting point of emergency rotate
                    goto case r;
                case r:
                    transform.Rotate(Vector3.up, +SPEED_ROTATE * Time.deltaTime);
                    break;

                case ln:
                    angle_start = transform.rotation;       //set starting point of emergency rotate
                    goto case l;
                case l:
                    transform.Rotate(Vector3.up, -SPEED_ROTATE * Time.deltaTime);
                    break;

                case ll: goto case rr;
                case rr:
                    SetEmergencyRotate(cmd_code, angle_start);
                    ExcuteEmergencyRotate();
                    break;

                default:
                case nothing:
                    break;
            }
    }

	void ExcuteCommand_LockOnMode(int cmd_code){
		if (emergency_mode && cmd_code != rr && cmd_code != ll) {
			ExcuteEmergencyRotate ();
			return;
		}
        if (cmd_code == nothing) { 
			KeepLookingTarget ();
			return;	// don't use [r] or [l] or [nothing]
		}

		bool success  = ChangeTargetToward (cmd_code);	// change target toward  left or right
		if (success == false) {
			if (cmd_code == rr || cmd_code == ll) {
				//print ("no more target on the side U want! emergency rotation start!");
				// TODO : 화면밖의 적에게 연결 구현?
				lock_on_mode = false;
				SetEmergencyRotate (cmd_code, transform.rotation);
				ExcuteEmergencyRotate ();
				return;
			}
		} else {	// if success, refresh pressed log. next input will not be rr || ll
			time_l = 0;
			time_r = 0;
		}
		KeepLookingTarget ();
		return;
	}
	void KeepLookingTarget(){
		Vector3 targetpos = target_updater.GetMainTarget().transform.position;
		Vector3 direction = targetpos - transform.position;
		direction.y = 0;	//세로 방향 고정
		direction.Normalize();
		Vector3 cur_dir = transform.forward;
		float gap = (1 - Vector3.Dot(direction, cur_dir)) / 2;	// (0`)0 <= gap <= 1 (180`)

		angle_toward = Quaternion.LookRotation(direction); //해당 방향으로 목표 각 설정

		transform.rotation = Quaternion.RotateTowards(transform.rotation, angle_toward, (SPEED_ROTATE + SPEED_ROTATE_EMERGENCY * gap) * Time.deltaTime);
	}

	bool ChangeTargetToward(int left_right){	//left < 0 < right
		return target_updater.ChangeTargetToward(left_right);
	}

	void SetEmergencyRotate(int left_right, Quaternion from) {
        if (emergency_mode == false)
        {
            if (left_right < 0)
                angle_toward = Quaternion.Euler(0f, -EMERGENCY_TURN, 0f) * from;
            else
                angle_toward = Quaternion.Euler(0f, +EMERGENCY_TURN, 0f) * from;
            emergency_mode = true;
            
            
        }
        else
        {
            print("double emergency rotate");
            if (left_right < 0)
                angle_toward = Quaternion.Euler(0f, -EMERGENCY_TURN, 0f) * angle_toward;
            else
                angle_toward = Quaternion.Euler(0f, +EMERGENCY_TURN, 0f) * angle_toward;
        }
        angle_start = angle_toward; // 꼬이는 경우 이거 달면 ㄱㅊ더라 ...
    }
	void ExcuteEmergencyRotate(){
		transform.rotation = Quaternion.RotateTowards(transform.rotation, angle_toward, SPEED_ROTATE_EMERGENCY * Time.deltaTime);
		if (transform.rotation == angle_toward){
			emergency_mode = false;
		}
	}

}


