using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractor : MonoBehaviour {	//handle outer effect on character (attacked, get warned)
	public enum TEAM{DEFAULT, PLAYER, ENEMY}        // 이거 class로 바꿀까? pointer 쓰게


    [HideInInspector] public TEAM team = TEAM.DEFAULT;			    //set this variable on UNITY INSPECTOR
	CharacterController controller;
	Stat				myStat;
	AI					ai;
    Rigidbody           rigidBody;
    Trace               trace;


	void Start(){
		controller= GetComponent<CharacterController> ();
		myStat    = GetComponent<Stat> ();
        rigidBody = GetComponent<Rigidbody>();
        trace     = GetComponent<Trace>();
	}

	public void Hit(Attack attack){ //attack will call this function when collide
        HpBar.GetMyHpBarWork(this.gameObject);

        if (true)       // 슈퍼아머 상태나 무경직 판정 등이 아닐 때
        {
            this.rigidBody.velocity = attack.transform.rotation * attack.knock_back;
            if (attack.knock_down)
                controller.KnockDown();
            else if (0 < attack.knock_time)
                controller.KnockBack(attack.knock_time);
        }

        myStat.GetDamageBy(attack);

        if (myStat.Get(Stat.Name.HP) < 0) {         // 듀금!
            Destroy(this.gameObject, 0.1f);         // 천천히 모션 보여주고 죽으려면 Die() 라는거 만들어야될듯 더미 이미지로 모션을 만들면 되나
        }
	}

  

	

}
