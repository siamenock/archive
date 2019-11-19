using UnityEngine;
using System.Collections;

public class OnDestroyCounterA : MonoBehaviour
{
    public static int count = 0;
    private void OnDestroy()
    {
        count++;
    }
}
