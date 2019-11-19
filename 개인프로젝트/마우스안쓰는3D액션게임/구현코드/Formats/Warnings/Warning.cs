using UnityEngine;
using System.Collections;

public class Warning : AttackOrWarning
{

	[HideInInspector] public Attack   attack;
	[HideInInspector] public Vector3  origin;             // 시작지점
	[HideInInspector] public float    time_attack_start;  // Attack Obj 생성되는 시점
	[HideInInspector] public bool     is_attack_start = false;

    private void Update()
    {
        if (is_attack_start == false)
        {
            if (Time.time - 0.1f > time_attack_start)   // TODO : attacker 가 해당 스킬 중지시로 변경할것. 지금 대충 적용함
                Destroy(this.gameObject);
				
        }
        else {
            if (attack == null)
                Destroy(this.gameObject);
        }


    }


	// 적과의 충돌까지 남은 시간을 예측
	// Attack 타입별로 여러가지 필요.
	public float TimeLeft2Attack(CharacterInteractor enemy) {
        float time_left_to_attack = (this.time_attack_start) - Time.time;   // 공격 시작까지 남은 시간
        float time_to_move = 0f;                                            // Attack obj 생성후 해당지점으로 이동까지 남은 시간

        if (this.attack is Bullet) {    // 정확하게 충돌지점으로 체크하는게 아니라, 시작점과 other의 위치로 계산함. 실제 시간보다 큼!
            Bullet bullet = (Bullet) this.attack;
            time_to_move = (this.gameObject.transform.position - this.attack.gameObject.transform.position).magnitude / bullet.speed;
        }

        return time_to_move + time_left_to_attack;
    }
}
