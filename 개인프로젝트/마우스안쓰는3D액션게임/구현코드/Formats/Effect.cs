using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect {//상태 이상
	public enum TYPE{	DOWN_EVADE}
	TYPE type;
	float time;
	float amount;

	public Effect (float time_, TYPE type_, float amount_){
		time = time_;
		type = type_;
		amount = amount_;
	}


	public void ReduceTime(float delta_time){
		time -= delta_time;
	}

	public TYPE	 GetTypeOfEffect(){ return type;}//GetType funtion already exist. Object.GetType()
	public float GetTime(){ return time;}
	public float GetAmount(){return amount;}

	public TYPE	 SetType(TYPE t){ return type = t;}
	public float SetTime(float t){ return time= t;}
	public float SetAmount(float a){return amount = a;}

}
