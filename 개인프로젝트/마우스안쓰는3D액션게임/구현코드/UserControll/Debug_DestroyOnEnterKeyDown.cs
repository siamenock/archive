using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_DestroyOnEnterKeyDown : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
            Destroy(this.gameObject);
        }
    }
}
