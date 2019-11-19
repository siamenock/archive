using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// 이 클래스는 공통Attribute만 저장함. 특별한 기능 없음.
// Attack 클래스는 Attack Obj가 적에게 충돌시 실제 데미지, 경직, CC등을 먹이기 위해 사용
// Warning클래스는 Attack Obj 생성이 확정되었을 때 미리 생성됨. Attack Obj가 나타날 장소, 진행경로에 배치되며
//                 AI가 이를 참고해 동작하고 (공격 피하기, 슈아로 버티면서 받아치기 등)
//                 회피판정 (회피에 성공시 추가효과 받는 스킬 등)에도 사용함. Warning box에는 맞았는데 Attack은 안 맞은 경우 회피로 판단.

public class AttackOrWarning : MonoBehaviour
{
    [HideInInspector] public GameObject attacker = null;
    
    
}
