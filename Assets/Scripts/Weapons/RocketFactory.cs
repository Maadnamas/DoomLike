using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketFactory : MonoBehaviour
{
    private RocketPool pool;

    void Awake()
    {
        pool = FindObjectOfType<RocketPool>();
        if (pool == null)
            Debug.LogError("no hay un RocketPool");
    }

    public GameObject CreateRocket(Vector3 position, Quaternion rotation)
    {
        return pool.GetRocket(position, rotation);
    }

    public void RecycleRocket(GameObject rocket)
    {
        pool.ReturnRocket(rocket);
    }
}

