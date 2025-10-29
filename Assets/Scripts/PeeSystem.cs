using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeeSystem : MonoBehaviour
{
    [Header("Ajustes Generales")]
    [SerializeField] private ParticleSystem peeParticles; // Asignar el sistema de part�culas desde el inspector
    [SerializeField] private KeyCode peeKey = KeyCode.X; // Tecla para hacer pis
    [SerializeField] private float maxPeeAmount = 100f; // Cantidad m�xima de pis
    [SerializeField] private float peeDrainRate = 20f; // Cu�nto baja por segundo al mear
    [SerializeField] private float peeRegenRate = 10f; // Cu�nto se regenera por segundo
    [SerializeField] private float minPeeToStart = 30f; // M�nimo para poder mear

    private float currentPeeAmount;
    private bool isPeeing = false;

    void Start()
    {
        currentPeeAmount = maxPeeAmount;
        if (peeParticles != null)
            peeParticles.Stop();
    }

    void Update()
    {
        HandleInput();
        RegeneratePee();
        UpdateParticleEmission();
    }

    void HandleInput()
    {
        if (peeParticles == null) return;

        // Si se mantiene presionada la tecla y hay pis suficiente
        if (Input.GetKey(peeKey) && currentPeeAmount > minPeeToStart)
        {
            if (!isPeeing)
            {
                isPeeing = true;
                peeParticles.Play();
            }

            currentPeeAmount -= peeDrainRate * Time.deltaTime;
            currentPeeAmount = Mathf.Clamp(currentPeeAmount, 0, maxPeeAmount);
        }
        else if (isPeeing)
        {
            // Si suelta la tecla o se queda sin pis, detener
            isPeeing = false;
            peeParticles.Stop();
        }
    }

    void RegeneratePee()
    {
        if (!isPeeing && currentPeeAmount < maxPeeAmount)
        {
            currentPeeAmount += peeRegenRate * Time.deltaTime;
            currentPeeAmount = Mathf.Clamp(currentPeeAmount, 0, maxPeeAmount);
        }
    }

    void UpdateParticleEmission()
    {
        if (peeParticles == null) return;

        // Ajustar la cantidad de part�culas seg�n cu�nto pis le quede
        var emission = peeParticles.emission;
        float emissionRate = Mathf.Lerp(0, 100, currentPeeAmount / maxPeeAmount);
        emission.rateOverTime = emissionRate;
    }
}