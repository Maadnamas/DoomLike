using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeeSystem : AbilityBase
{
    [Header("General Settings")]
    [SerializeField] private ParticleSystem peeParticles;
    [SerializeField] private KeyCode peeKey = KeyCode.X;
    [SerializeField] private float maxPeeAmount = 100f;
    [SerializeField] private float peeDrainRate = 20f;
    [SerializeField] private float peeRegenRate = 10f;
    [SerializeField] private float minPeeToStart = 30f;

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

        // If key is held and there is enough pee
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
            // If key is released or out of pee, stop
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

        // Adjust particle amount based on remaining pee
        var emission = peeParticles.emission;
        float emissionRate = Mathf.Lerp(0, 100, currentPeeAmount / maxPeeAmount);
        emission.rateOverTime = emissionRate;
    }
}