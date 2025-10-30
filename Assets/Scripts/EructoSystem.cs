using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EructoSystemm : MonoBehaviour
{
    [Header("eructo setins")]
    public GameObject burpSmokePrefab;  

    public AudioClip burpSound;  
    
    public float burpCooldown = 3f;

    public float spawnDistance = 1f;

    private float burpTimer = 0f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        burpTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E) && burpTimer <= 0f)
        {
            Burp();
            burpTimer = burpCooldown;
        }
    }

    void Burp()
    {

        if (burpSmokePrefab)
        {
            Vector3 spawnPos = transform.position + transform.forward * spawnDistance;
            GameObject smoke = Instantiate(burpSmokePrefab, spawnPos, Quaternion.identity);
            Destroy(smoke, 2f);
        }

        if (burpSound)
        {
            audioSource.pitch = Random.Range(0.8f, 1.3f);
            audioSource.PlayOneShot(burpSound);
        }

        Debug.Log("SE COMUNICA MEDIANTE ESTE DEBUG.LOG QUE MASON HA ERUCTADO");
    }
}
