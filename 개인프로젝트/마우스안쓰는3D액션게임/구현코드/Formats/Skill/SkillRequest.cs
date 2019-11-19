using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// character use Skill::SkillProcess() to use skill. in function CharacterUpdater::SkillProcess() cooltime management, motion time managemet are done in there
// Skill::~~() returns this class to give data that can't be handle in Skill class
public class SkillRequest
{
    public static readonly SkillRequest END_SKILL = null;   // any better idea?
    
    //=============================================== 일반 Action Request ===============================================//
    // 캐릭터 이동&회전, 공격 모션 시작 명령, 공격 Obj 생성 등
    //기타 버프효과 등도 추가바람. 근데 그러면 오퍼레이터도 같이 변경해줘야됨
    public string       motionName  = null;             // animation code that character must perform
    public Vector3      movement    = Vector3.zero;       // character should move towaard...
    public float        rotateSpeed = 0f;              // 좌우 구분을 여기서 해줘야 함
    public List<GameObject> atkObjs;   // character should instantiate attack Objects
    //=============================================== 일반 Action Request ===============================================//

    //================================================== 특수 Request ===================================================//
    
    //public int     saveAimStack = 0;                            // 이거 안쓸려나...?
    public bool?   setRotateLock = null;             // false면 자유상태, true면 lock걸려서 rotateSpeed에 의해서만 회전가능
                                                     // 락걸리면 상대가 움직여도              

    //================================================== 특수 Request ===================================================//

    public SkillRequest() {
        atkObjs = new List<GameObject>();
    }

    public static SkillRequest Copy(SkillRequest a) {
        if (a == END_SKILL) { return END_SKILL; }
        SkillRequest ret = new SkillRequest();
        ret.motionName  = a.motionName;
        ret.movement    = a.movement;
        ret.rotateSpeed = a.rotateSpeed;
        ret.atkObjs     = new List<GameObject>(a.atkObjs);
        return ret;
    }


    public static SkillRequest operator +(SkillRequest a, SkillRequest b)
    {
        if (a == END_SKILL) return b;       // == null
        if (b == END_SKILL) return a;

        SkillRequest ret = SkillRequest.Copy(a);

        // 다른 attribue 추가되면 여기도 바꿔줘야 함
        if (a.movement == Vector3.zero) ret.movement = b.movement;
        if (a.rotateSpeed == 0f)        ret.rotateSpeed = b.rotateSpeed;

        if ((a.motionName == null || a.motionName == "") && b.motionName != null)
            ret.motionName = string.Copy(b.motionName);
        if (b.atkObjs != null)
            foreach (GameObject o in b.atkObjs)
                ret.atkObjs.Add(o);
        if (b.setRotateLock == null || a.setRotateLock == b.setRotateLock)   // 둘다 None이면 None 아닌걸로 처리하고, 같으면 그냥 똑같이 감
            ret.setRotateLock = a.setRotateLock;
        else if (a.setRotateLock == null)                                  // 서로 다른 경우 뒤에 나온게 더 우선시 됨
            ret.setRotateLock = b.setRotateLock;
        
            return ret;
    }
}
