using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageShow : MonoBehaviour {
    private static GameObject maincam = null;
    public static GameObject GetMainCam() { return maincam? maincam : maincam = GameObject.Find("MainCamera"); }

    private float speed =1f;

    public static void MakeUI(GameObject target, int damage) {
        GameObject display = Resources.Load("DamageShowUI",typeof(GameObject)) as GameObject;
        if (display == null) {
            print("can not load");
            return;
        }
        Vector3 pos = target.transform.position;
        float height = target.transform.lossyScale.y;
        pos.y += height;
        
        float distance = (pos - GetMainCam().transform.position).magnitude;


        display = Instantiate(display);
        display.GetComponent<TextMesh>().text = "" + damage;
        display.GetComponent<DamageShow>().speed = distance / 100;
        display.transform.localScale = new Vector3(1, 1, 1) * distance / 50;
        display.transform.position = pos;

        Transform t = display.gameObject.transform;
        Vector3 dir = t.position - maincam.transform.position;
        Quaternion q = new Quaternion();
        q.SetLookRotation(dir);
        t.rotation = q;

        Destroy(display, 1f);
    }

    private void Update()
    {
        this.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
    }


}
