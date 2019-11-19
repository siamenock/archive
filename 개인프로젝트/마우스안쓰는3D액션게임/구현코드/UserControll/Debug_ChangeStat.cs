using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Debug_ChangeStat : MonoBehaviour
{
    static readonly float GAP = 0.2f;
    Stat targetStat;
    // Start is called before the first frame update
    void Start()
    {
        targetStat = this.gameObject.GetComponent<Stat>();
    }

    // Update is called once per frame
    void Update()
    {
        int change = 0;
        if (Input.GetKeyDown(KeyCode.LeftBracket )) { change = -1; }
        if (Input.GetKeyDown(KeyCode.RightBracket)) { change = +1; }

        if (change == 0) return;


        float cur;
        cur = targetStat.Get(Stat.Name.EVADE);
        cur += GAP * change;
        targetStat.Set(Stat.Name.EVADE, cur);

        MonoBehaviour.print("change evade stat to " + cur);
        //GUI.Label(new Rect(10, 10, 100, 30), Convert.ToString(cur));      // 이거 딴데서 해야된다는데... 아몰랑 걍 프린트로그로 때워
        

    }
}
