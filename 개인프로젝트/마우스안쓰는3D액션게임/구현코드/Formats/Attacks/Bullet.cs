using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Attack {
    public float range = 10f, speed = 0.1f;
    
    protected override void Start () {
        base.Start();

        // 자체 Obj 관련
        this.GetComponent<Rigidbody>().velocity = transform.forward * this.speed;
        Destroy(this.gameObject, this.range / this.speed);  // 사거리 밖으로 나가면 제거됨

        
    }

    protected override void BuildWarningDisabled(GameObject attacker, float time_predelay)
    {
        base.BuildWarningDisabled(attacker, time_predelay);
        
        // Bullet Warning 관련 특이사항들
        Transform t = this.warning.transform;
        Vector3 scale = t.localScale;
        scale.z += this.range;
        t.localScale = scale;
    }

    // Update is called once per frame
    // void Update () {}
}
