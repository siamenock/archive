using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// homing missle Class : trace enemy until specific time limit or in angle limit
public class Missle : Bullet
{ 
    public float    rotSpeed    = 10f,
                    limitTime   = 1f,
                    limitAngle  = 90f;   // 2가지 limit중에 뭘 할까?

    private bool        chaseMode       = true;
    private float       START_TIME      = 0;
    private Rigidbody   pThisRigidbody;               // pointer of RigidBody
    private Trace       pTargetTrace;
    private Stat        pTargetStat,
                        pAttackerStat;
    


    protected override void Start()
    {
        base.Start();

        START_TIME          = Time.time;

        pThisRigidbody      = this          .GetComponent<Rigidbody>();
        pTargetTrace        = this.target   .GetComponent<Trace>();
        pTargetStat         = this.target   .GetComponent<Stat>();
        pAttackerStat       = this.attacker .GetComponent<Stat>();
    }


    protected override void Update()
    {
        base.Update(); // 이것도

        if (this.target == null)
        {
            FixDirectionAndWarning();
            return;
        }

        if (chaseMode)
        {  
            Vector3     posTarget  = pTargetTrace.GetPosWith( pAttackerStat.Get(Stat.Name.CHASE) + this.additional_chase);  // target pos
            Vector3     gapTarget  = (posTarget - this.transform.position);        // distance between me and target

            Quaternion rotCurrent = this.transform.rotation;
            Quaternion rotTarget = Quaternion.LookRotation(gapTarget, Vector3.up);        // rotation toward target

            float angleGap = Quaternion.Angle(rotCurrent, rotTarget);  // unsigned 양수값 return

            // 최소거리 내로 접근했거나, 최대각을 벗어나는 경우 추격 중지
            if (gapTarget.magnitude < limitTime * this.speed || limitAngle < angleGap)
            {
                FixDirectionAndWarning();
                return;
            }

            Quaternion rotNewThis = Quaternion.RotateTowards(this.transform.rotation, rotTarget, this.rotSpeed * Time.deltaTime);
            float      curSpeed   = this.pThisRigidbody.velocity.magnitude;

            // 실제 적용은 두줄밖에 안대여
            this.pThisRigidbody.velocity = rotNewThis * Vector3.forward * curSpeed;
            this.transform.rotation      = rotNewThis;

        }

       
        
    }
 
    void FixDirectionAndWarning()
    {
        chaseMode = false;                      // fix dir

        this.warning.transform.position = this.transform.position;
        this.warning.transform.rotation = this.transform.rotation;
        


        float len       = this.range - (Time.time - START_TIME) * this.speed;   // total range - moved range. todo: 이거 슬로우모션 연동하면면 버그나겠다.
        Transform t     = this.warning.transform;
        Vector3 scale   = t.localScale;
        scale.z         += len;
        t.localScale    = scale;   

        this.warning.gameObject.SetActive(true);// enable warning


    }
}