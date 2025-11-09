using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPool : MonoBehaviour
{
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private int initialSize = 10;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject rocket = Instantiate(rocketPrefab);
            rocket.SetActive(false);
            pool.Enqueue(rocket);
        }
    }

    public GameObject GetRocket(Vector3 position, Quaternion rotation)
    {
        GameObject rocket;
        if (pool.Count > 0 && !pool.Peek().activeInHierarchy)
        {
            rocket = pool.Dequeue();
        }
        else
        {
            rocket = Instantiate(rocketPrefab);
        }

        rocket.transform.position = position;
        rocket.transform.rotation = rotation;
        rocket.SetActive(true);
        return rocket;
    }

    public void ReturnRocket(GameObject rocket)
    {
        rocket.SetActive(false);
        pool.Enqueue(rocket);
    }
}