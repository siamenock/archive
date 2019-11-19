using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;


public class TargetUpdater : MonoBehaviour {
	List<GameObject> target_list = new List<GameObject>();	// saves in form of GameObject
    List<GameObject>  outer_list = new List<GameObject>();  // 캐릭터가 공격 불가능한 위치에 있는 타겟 (중간에 벽)
    CharacterInteractor mydata;
	GameObject	self;
	bool		im_player = false;

	float		range	= 70f;	
	float		target_list_lock_timer;
	int			main_target = -1;

	Camera		cam = null;
	CameraMover cam_mover = null;
	float		cam_w, cam_h;	    //TODO : find exact angles from camera.
	float		margin_w = 0.02f,   // 2% width margin
                margin_h = 0.00f;	// no hight margin



	// Use this for initialization
	void Start () {		
		self = gameObject;
		if (self.name == "Player") {	        // todo : CHANGE THIS hard coded...
			im_player = true;
			cam_mover = GameObject.Find ("CameraHolder").GetComponent<CameraMover> ();
			InitIsInCamera ();
		}
		mydata = GetComponent<CharacterInteractor> ();
		target_list_lock_timer = Time.time;
		StartCoroutine(UpdateTargetList ());	//0.2sec routine
	}

	IEnumerator UpdateTargetList(){
		while (true) {
			yield return new WaitForSeconds (.2f);
			while(Time.time < target_list_lock_timer){
				float wait = target_list_lock_timer - Time.time;
				yield return new WaitForSeconds (wait);
			}
			RefreshTargets ();
		}
	}

    // ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ public methods ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼

    public void ChangeTargetToNext(){
		target_list_lock_timer = Time.time + 0.25f;	//여러번 탭해서 변경하는경우를 위해 타겟리스트고정
		ChangeMainTargetTo( (main_target + 1) % target_list.Count);
		return;
	}
	public bool ChangeMainTargetTo(int target_num){
		if (0 <= target_num && target_num < target_list.Count) {
			main_target = target_num;
			return true;
		} else {
			return false;
		}
	}
	//가장 가까운각도의 좌/우측 sub타겟으로 메인타겟변경. 각도는 radian집어치우고 화면내xy좌표기준임
	public bool ChangeTargetToward(int left_right){		//l= -1, r= +1
        if (target_list.Count < 2 || left_right == 0) { return false; }
        float x_limit = ((cam.WorldToScreenPoint(target_list[main_target].transform.position)).x / Screen.width) *2 -1;
		return ChangeTargetToward(left_right, x_limit);

    }

	// 가장 가까운각도의 좌/우측 sub타겟으로 메인타겟변경. 각도는 radian집어치우고 화면내xy좌표기준임
	public bool ChangeTargetToward(int left_right, float x_limit){	//x_limit == [-1 ~ +1] 범위로 표현된 스크린 x좌표
		if (target_list.Count < 2 || left_right == 0) { return false; }

		float min_right = +1, min_left = -1;
		int i_right = main_target, i_left = main_target;	// index to min left/right
		for (int i = 0; i < target_list.Count; i++) {
			float x = ((cam.WorldToScreenPoint(target_list[i].transform.position)).x / Screen.width) *2 -1;
			// x == [-1 ~ +1] 범위로 표현된 스크린 x좌표
			// x_limit은 main_target의 좌표. 최적화가능
			if (x_limit < x && x < min_right)	{ min_right = x; i_right = i; }
			if (x_limit > x && x > min_left)	{ min_left  = x; i_left  = i; }
		}

		if (left_right < 0) {	// find left target
			if (i_left == main_target) {
				return false;	// nothing is more left than main_target
			} else {
				main_target = i_left;
				return true;
			}
		} else {				// find right target
			if(i_right == main_target){
				return false;	// nothing is more right than main_target
			} else {
				main_target = i_right;
				return true;
			}
		}
	}
    // 타겟 리스트 중 dir 방향에 가장 가까운 타겟 찾기
    public int GetCentralTargetNum(Quaternion dir){
		Vector3 pibot_pos = transform.position + dir * Vector3.forward;
		float x_max = 1.1f;	// -1 <= x <= +1
		int target_num = -1;
		for (int i = 0; i < target_list.Count; i++) {
			float x = ((cam.WorldToScreenPoint(target_list[i].transform.position)).x / Screen.width) *2 -1;
			if( 0 < (x_max-x)*(x_max+x) ){	// \x\ < \x_max\  절대값비교
				x_max = x;
				target_num = i;
			}
		}
		return target_num;
	}
	public GameObject GetTarget(int target_num){ return TargetNullCheck(target_num)? GetMainTarget() : target_list[target_num];}
    public List<GameObject> GetTargets()	{ return target_list;}
    public List<GameObject> GetOuters ()    { return  outer_list; }
    public GameObject GetMainTarget() {
		if (0 <= main_target && main_target < target_list.Count) {
            TargetNullCheck (main_target);
			return target_list [main_target];
		} else {
			return null;    // no target
		}
	}
    public bool TargetNullCheck(int target_num) {

        if (target_list[target_num] == null) {
            RefreshTargets();
            return true;
        }
        return false;
    }


    // ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ private methods ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼
    bool IsTerrain(GameObject o) { return o && o.tag == "Terrain"; }

    void RefreshTargets()
    {
        //save current main target. it will be main target again if in camera again
        GameObject last_main_target = (main_target < 0) ? null : target_list[main_target];


        target_list.Clear();
        outer_list.Clear();


        //================================================================//
        //                      Refresh Target List                       //
        //================================================================//
        Vector3 pos_me = transform.position;
        Collider[] in_range = Physics.OverlapSphere(pos_me, range);
        int col_count = 0;
        // target must not be my team, and must be in screen
        foreach (Collider col in in_range)
        {
            col_count++;
            CharacterInteractor enemy = col.GetComponent<CharacterInteractor>();
            if (enemy == null || enemy.team == mydata.team)
            { // not attack-able object or my team   
                continue;
            }
            else if (!im_player)
            {
                // for AI,	not supporting camera now. AI has 360 degree sight now
                if (!MyLibrary.IsBetween(this.gameObject, col.gameObject, IsTerrain))
                    target_list.Add(col.gameObject);
                else    // 직접공격 불가위치
                    outer_list.Add(col.gameObject);
            }
            else if ((im_player && !IsInCamera(col.transform.position)))
            {
                continue;
                // for user,only targets in screens is marked
            }
            else
            {
                if (!MyLibrary.IsBetween(this.gameObject, col.gameObject, IsTerrain))
                    target_list.Add(col.gameObject);
                else    // 직접공격 불가위치
                    outer_list.Add(col.gameObject);

            }

        }

        //================================================================//
        //               New Main Target =  Last Main target              //
        //================================================================//
        bool main_target_in_screen = false; // 메인타겟이 시야에 남아 있는가? 확인하
        if (main_target != -1)
        {			// there was Main target in last check
            int i;
            for (i = 0; i < target_list.Count; i++)
            {
                if (last_main_target == target_list[i])
                {   //move main target to target_list[0]
                    main_target_in_screen = true;

                    GameObject temp = target_list[0];
                    target_list[0] = target_list[i];
                    target_list[i] = temp;
                    main_target = 0;
                    break;
                }
            }
            if (i == target_list.Count && cam_mover != null)
            {   // main target 사라짐
                if (cam_mover.IsLockOnMode())
                    cam_mover.SetLockOnMode(false);
            }
        }
        //================================================================//
        //                  New Main Target == #0 target                  //
        //================================================================//
        if (target_list.Count == 0)
        {
            main_target = -1;
        }
        else
        {
            main_target = 0;
            if (main_target_in_screen)
            {
                // sort except 0 (main target already in target_list[0])
                target_list.Sort(1, target_list.Count - 1, new SortByDistanceFrom(self));
            }
            else
            {
                // sort 0 ~ all. now target_list[0] is new main target
                target_list.Sort(0, target_list.Count, new SortByDistanceFrom(self));
            }
        }
    }

    
	Quaternion YAngleToward(GameObject from, GameObject to){
		Vector3 dir = to.transform.position - from.transform.position;
		dir.y = 0;

		Quaternion ret = new Quaternion ();
		ret.SetLookRotation (dir);
		return ret;
	}

	// ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ Player인 경우에만 사용하는 함수 ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼ ▼
	void InitIsInCamera(){
		cam = GameObject.Find("MainCamera").GetComponent<Camera> ();
		if (cam == null) {	print ("CRITICAL ERR : TargetUpdater.InitIsInCamera() failed!");	}
		cam_w = Screen.width;
		cam_h = Screen.width;

	}

	bool IsInCamera(Vector3 pos_enemy){
		pos_enemy = cam.WorldToScreenPoint (pos_enemy);
		return (cam_w * margin_w <= pos_enemy.x && pos_enemy.x < cam_w * (1-margin_w)
			&& cam_h * margin_h <= pos_enemy.y && pos_enemy.y < cam_h * (1-margin_h));
	}

    // List Sorting 용 class
    private class SortByDistanceFrom : IComparer<GameObject>
    {
        public GameObject p;

        public SortByDistanceFrom(GameObject _self) { p = _self; }

        public int Compare(GameObject x, GameObject y)
        {
            Vector3 pos_self = p.transform.position;
            float dst1 = (pos_self - x.transform.position).magnitude;
            float dst2 = (pos_self - y.transform.position).magnitude;

            if (dst1 > dst2)
            {
                return +1;
            }
            else if (dst1 < dst2)
            {
                return -1;
            }
            else
            {
                return 00;
            }
        }
    }
}
