using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EructoSystemm : MonoBehaviour
{
    [Header("eructo setins")]
    public GameObject burpSmokePrefab;      

    public AudioClip[] burpSounds;          

    public float burpCooldown = 3f;      
    
    public float spawnDistance = 1f;        

    private float burpTimer = 0f;
    private AudioSource audioSource;

    [Header("Pitch stenigns")]
    public float minPitch = 0.95f;         

    public float maxPitch = 1.05f;           


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        burpTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.C) && burpTimer <= 0f)
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


        if (burpSounds != null && burpSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, burpSounds.Length);
            AudioClip chosenClip = burpSounds[randomIndex];

            audioSource.pitch = Random.Range(minPitch, maxPitch); 
            audioSource.volume = Random.Range(0.8f, 1f);
            audioSource.PlayOneShot(chosenClip);
        }

        Debug.Log("SE COMUNICA MEDIANTE ESTE DEBUG.LOG QUE MASON HA ERUCTADO");
    }
}
