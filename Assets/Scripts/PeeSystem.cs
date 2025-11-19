using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeeSystem : AbilityBase
{
    [Header("Ajustes Generales")]
    [SerializeField] private ParticleSystem peeParticles; // Asignar el sistema de partículas desde el inspector
    [SerializeField] private KeyCode peeKey = KeyCode.X; // Tecla para hacer pis
    [SerializeField] private float maxPeeAmount = 100f; // Cantidad máxima de pis
    [SerializeField] private float peeDrainRate = 20f; // Cuánto baja por segundo al mear
    [SerializeField] private float peeRegenRate = 10f; // Cuánto se regenera por segundo
    [SerializeField] private float minPeeToStart = 30f; // Mínimo para poder mear

    private float currentPeeAmount;
    private bool isPeeing = false;

    void Start()
    {
        currentPeeAmount = maxPeeAmount;
        if (peeParticles != null)
            peeParticles.Stop();
    }

    public override void ActionExecution()
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

        // Ajustar la cantidad de partículas según cuánto pis le quede
        var emission = peeParticles.emission;
        float emissionRate = Mathf.Lerp(0, 100, currentPeeAmount / maxPeeAmount);
        emission.rateOverTime = emissionRate;
    }
}