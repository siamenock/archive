using UnityEngine;
using System.Collections;
using System;

public class MyLibrary
{


    // from과 to 사이에 충돌판정이 있는 gameobjcet 중 Criteria에 해당되는 것이 있는지 return
    public static bool IsBetween(GameObject from, GameObject to, Func<GameObject,bool> Criteria) {
        Ray ray = new Ray(from.transform.position, (to.transform.position - from.transform.position).normalized);

        Vector3 posStart    = from.transform.position;
        Vector3 moveVector  = (to.transform.position - from.transform.position);

        RaycastHit[] hits =  Physics.RaycastAll(posStart, moveVector.normalized, moveVector.magnitude);
        foreach (RaycastHit hit in hits) {
            if (Criteria(hit.collider.gameObject)) {
                return true;
            }
        }
        return false;
    }

	

	public static bool ReturnTrue(GameObject o) { return true;}

    public static bool CriteriaTerrainOrEnemy(GameObject o)
    {
        if (o.tag == "Terrain" || o.tag == "Enemy") return true;

        MonoBehaviour.print("tag " + o.tag + " is not accepted");
        return false;
    }

    // from부터 dir 방향으로 raycast해서 부딪힌 결과물 중 criteria를 만족하고, 가장 가까이서 부딪힌 물체와의 충돌위치를 return
    // 타겟이 없는경우 from + dir return
    public static Vector3 RayPoint(Vector3 from, Vector3 dir, Func<GameObject, bool> Criteria = null) {
        Ray ray = new Ray(from, dir * 999f);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            return hit.point;
        }
        return from + dir * 10000;
    }

    // that 게임오브젝트의 child 중 이름으로 검색. 없으면 return null.
    public static GameObject FindChildGameObjectWithName(GameObject that, string name)
    {
        GameObject ret = null;
        for (int i = 0; i < that.transform.childCount; i++)
        {
            Transform t = that.transform.GetChild(i);
            if (t.name == name)
            {
                ret = t.gameObject;
                break;
            }
        }
        
        return ret;
    }
}
