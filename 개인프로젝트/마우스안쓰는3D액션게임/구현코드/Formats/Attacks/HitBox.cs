using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HitBox : Attack
{       // hitbox style (can be sphere, sylinder also)
    public float duration = 0f; // time to remain
    public bool independent_from_atker = false; // if false, attack cancled when attacker hit, move together with attacker

    private void Start()
    {
        base.Start();
        Destroy(this.gameObject, duration);
    }
}
