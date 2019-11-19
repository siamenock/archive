using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trace : MonoBehaviour {

	//TODO : 시스템보강, 순간이동시 보완 필요
	//시스템 결점 : 연속적 이동만 가정하고 있음.
	//0.1초마다 위치 로그 남기고 0.22초 요구시 선형평균으로 return
	//save untill 4 sec (not exact actually)
	
	private static readonly float   TRACE_SAVE_MAX_TIME 	= 4f,		// save trace data of 4 sec maximum
					                PERIOD			        = 0.1f,     // save trace every 0.1 sec
                                    FORCAST_MODEL_MIN = 1f/3f,
                                    FORCAST_MODEL_MAX = 1.5f;             // forcast 시스템에 사용되는 변수. (value)초 이상의 gap을 이용한 모델은 사용하지 않는다는 뜻.

    private List<Vector3>           trace                   = new List<Vector3>();
    private float                   timer_stop              = -9999f,	// Tracer stop untill (timer_stop)
	                                time_last;				            // last saved time
	private int                     allowed_save            = (int) (TRACE_SAVE_MAX_TIME / PERIOD);
    private Stat                    pStat;
    private CharacterController     pCC;


    void Start(){
		StartCoroutine (Routine());
        pStat = this.GetComponent<Stat>();
        pCC   = this.GetComponent<CharacterController>();
	}

	public IEnumerator Routine(){
		time_last = Time.time;
		while (true) {
			yield return new WaitForSeconds (PERIOD);
			while(Time.time < timer_stop){
				yield return new WaitForSeconds (0.04f);
			}

            CharacterController.State s = pCC.GetState();

            if (s == CharacterController.State.down || s == CharacterController.State.hit)  // 공격받아서 경직/다운된 상태면 trace 초기화됨.
                trace.Clear();

			trace.Insert (0, transform.position );
			time_last = Time.time;
			if(allowed_save < trace.Count){
				trace.RemoveRange (allowed_save, trace.Count - allowed_save);
			}

		}
	}

    public float GetMaxRecordTime() {
        return trace.Count * PERIOD;
    }

    public Vector3 GetPosWith(float chase) {
        return PosB4(chase - pStat.Get(Stat.Name.EVADE));
    }

	public Vector3 PosB4(float time){
		if(time < 0){ time = 0; }

        int index     = (int) (time / PERIOD);
		float mid       = (time % PERIOD) / PERIOD;	// 0 ~ 1 val 가중치
        if (time == 0){         // evade system not need
            return transform.position;
        } else if (0 < index && index < trace.Count) {		// has enough tracelog
			return trace[index] * mid + trace[index + 1] * (1 - mid);
		} else {	            // not enough trace log -> return last history
            return trace[trace.Count - 1];
		}
	}

	public void RecordStop(float stopping_period){	//remove past trace if got hit. Under Combo-Attack, character can not evade.
		if(stopping_period < 0){ return; }
		timer_stop = Time.time + stopping_period;
	}
	public void RecordReset(){		    //not working now...
		timer_stop = Time.time;
	}

    // 즉시공격인 HitRay나 Hitbox가 아닌 Bullet형태의 공격을 할 때 예측샷을 지원해주는 함수
    // after초 이후의 위치를 return
    // evade_gap은 회피-명중 보정치를 의미.
    // accuracy는 얼마나 정밀하게 측정할건지 의미함. (예시: 대상을 오랬동안 타겟팅하고있거나 자신이 정지상태에서 오래 있을경우 accuracy상승)
    // accuracy는 일종의 시간해상도? 역할로 작용함. 등속운동시에는 상관없지만, 가속운동시 측정간격 레벨이 됨. 이게 높으면 연산을 더 많이 함

    // 일기예보랑 비슷한 개념으로 작동함. 여러가지 모델을 이용해서 과거 데이터를 기준으로 현재위치를 추측함. 이 중 가장 정확한 모델을 사용해 미래값을 return 
    // 모델이라 함은 어느정도의 time_gap으로 측정할지에 따라 달라짐
    // 1차 근사식으로 예측함.
    // TODO: 2차근사도 생각해보자.
    
    public Vector3 PredictAfter(float after, float evade_gap, float accuracy = 0) {
        if (after     < 0) { after      = 0; }
        if (evade_gap < 0) { evade_gap  = 0; }

        float e = evade_gap;
        float forcast_error_min = float.MaxValue;
        float best_model = -1f;
        float time_resolution = 1 / (accuracy + 1/FORCAST_MODEL_MIN);
        float limit = FORCAST_MODEL_MAX;

        if (e + 2 * time_resolution > this.GetMaxRecordTime()) {
            time_resolution = (GetMaxRecordTime() - e) / 2;
            limit = time_resolution * 2;
        }

        for (float model = time_resolution; model < limit; model += time_resolution) {
            Vector3 now_pos = this.PosB4(e);                                    // 회피율 격차로 인한 시간차를 뺀 만큼의 현재
            Vector3 predict = PredictVector(e, e + model, e + 2 * model);    // predicted now position 
            Vector3 gap     = (now_pos - predict);
            float forcast_error = gap.magnitude;          // 실제 위치와 예측값 사이 오차 측정

            if (forcast_error < forcast_error_min) {
                forcast_error_min = forcast_error;
                best_model = model;
            }
        }
        // best model 선정 완료

        return PredictVector(+e - after, e, e + best_model);
    }
    // b4 초전 위치와 now초전 위치를 기반으로 after초 후 위치를 예측함. (대과거 데이터 기반으로 과거를 예측 가능)
    // 파라미터 값은 +방향이 과거로 가는 방향, -가 미래로 가는 방향임.
    // after < (now = 0) < b4 여야 일반적인 미래예측 알고리즘.
    private Vector3 PredictVector(float after, float now, float b4) {
        //Instantiate(DB_temp.lib[DB_temp.NAME.pos_marker2], this.PosB4(now), new Quaternion());
        //Instantiate(DB_temp.lib[DB_temp.NAME.pos_marker2], this.PosB4(b4), new Quaternion());

        return PredictVector2(
                this.PosB4(now),
                this.PosB4(b4),
                after - now,
                now   - b4
            );
    }

    // 1차근사
    // after < (now = 0) < b4 여야 일반적인 미래예측 알고리즘.
    private Vector3 PredictVector2(Vector3 now, Vector3 last, float after, float b4) {
        Vector3 gap = now - last;
        gap *= (after / b4);
        Vector3 ret = (now + gap);

        //Instantiate(DB_temp.lib[DB_temp.NAME.pos_marker2], last, new Quaternion());
        //Instantiate(DB_temp.lib[DB_temp.NAME.pos_marker2], now , new Quaternion());
        //Instantiate(DB_temp.lib[DB_temp.NAME.pos_marker2], ret , new Quaternion());

        return (now + gap);
    }

    
}










