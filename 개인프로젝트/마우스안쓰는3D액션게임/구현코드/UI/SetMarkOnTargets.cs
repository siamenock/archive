using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMarkOnTargets : MonoBehaviour {
    public GameObject ui_main_target;
	public GameObject ui_text_target;
	TargetUpdater target_updater;
	Camera cam;

	Vector3 out_of_screen;
	List<GameObject> next_markers  = new List<GameObject>();
    List<GameObject> outer_markers = new List<GameObject>();
    

	// Use this for initialization
	void Start () {
		target_updater	= GameObject.Find ("Player").GetComponent<TargetUpdater>();
		cam				= GameObject.Find ("MainCamera").GetComponent<Camera> ();
        
        //ui_main_target  = GameObject.Instantiate(ui_main_target, this.GetComponent<Transform>());
        //MonoBehaviour.print("타겟마커 업데이터 main_target == " + ui_main_target + ",   text_marker == " + ui_text_target);

        //this ui must be out of screen before game starts.
        out_of_screen = GetComponent<RectTransform> ().position;		
		for (int i = 0; i < 10; i++) {
			GameObject added = GameObject.Instantiate (ui_text_target, GetComponent<RectTransform>().parent); 
			added.name = ("subtarget marker" + i);
			added.GetComponent<Text> ().text = "Target" + i;
			next_markers.Add (added);
		}

        for (int i = 0; i < 10; i++)    // 벽에 가로막힌 object들은 이런식으로 표시함. (카메라 시야에도 막혔으면 표시하지 말도록 수정 필요)
        {
            GameObject added = GameObject.Instantiate(ui_text_target, GetComponent<RectTransform>().parent);
            added.name = "outer marker" + (i);
            added.GetComponent<Text>().text = "공격불가";
            outer_markers.Add(added);
        }
    }
	
	// Update is called once per frame
	void Update () {
		Vector3 move_ui_to;
		GameObject main_target = target_updater.GetMainTarget();

		// for main target
		if (main_target != null) {	move_ui_to = cam.WorldToScreenPoint (main_target.transform.position);
		} else {					move_ui_to = out_of_screen;	}

		GetComponent<RectTransform> ().position = move_ui_to;

		// for sub-targets
		// set target markers for targets (except main target)
		List<GameObject> targets = target_updater.GetTargets ();
        List<GameObject> outers  = target_updater.GetOuters();
		int i;
		for (i = 0; i < targets.Count && i < 10; i++) {
			if(targets[i] == null){ break; }
			if(targets[i] == main_target){
				next_markers [i].transform.position = out_of_screen;
				continue;
			}
			move_ui_to = cam.WorldToScreenPoint(targets[i].transform.position);
			next_markers [i].transform.position = move_ui_to;
		}

		// move away not-using sub-target markers
		for (   ;	i < 10; i++) {
			next_markers[i].transform.position = out_of_screen;
		}

        for (i = 0; i < outers.Count; i++) {
            if (outers[i] == null) { break; }
            move_ui_to = cam.WorldToScreenPoint(outers[i].transform.position);
            outer_markers[i].transform.position = move_ui_to;
        }
        for (; i < 10; i++)
        {
            outer_markers[i].transform.position = out_of_screen;
        }
    }
}
