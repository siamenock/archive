using UnityEngine;
using System.Collections;

public class PrefabDeActivate : MonoBehaviour
{
    public string[] deactivate_list;
    // Use this for initialization
    void Awake()
    {
        foreach (string name in deactivate_list) {
            GameObject prefab = Resources.Load(name, typeof(GameObject)) as GameObject;
            prefab.SetActive(false);
        }

        //Destroy(this);
    }

    
}
