using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EructoSystemm : AbilityBase
{
    [Header("Eructo Settings")]
    public GameObject burpSmokePrefab;
    public AudioClip[] burpSounds;
    public float burpCooldown = 3f;

    public float spawnDistance = 1f;
    private float burpTimer = 0f;

    [Header("Pitch Settings")]
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    [Header("Volume Settings")]
    public float minVolume = 0.8f;
    public float maxVolume = 1f;

    public override void ActionExecution()
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

            // Usar AudioManager para reproducir el sonido en 3D en la posición del jugador
            AudioManager.PlaySFX3D(chosenClip, transform.position);
        }
    }
}
