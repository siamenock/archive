using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInNsec : MonoBehaviour
{
    public float destroy_time = 0.1f;
    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, destroy_time);
    }

}
