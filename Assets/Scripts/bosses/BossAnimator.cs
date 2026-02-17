using UnityEngine;

public class BossAnimator : MonoBehaviour, IEnemyAnimator
{
    private Animator animator;

    [Header("Animator Parameter Names")]
    [SerializeField] private string walkParameter = "walk";
    [SerializeField] private string meleeAttackTrigger = "attack1";
    [SerializeField] private string spikeAttackTrigger = "attack2";
    [SerializeField] private string hitTrigger = "hit";
    [SerializeField] private string deathTrigger = "death";

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("No Animator found on Boss: " + gameObject.name);
        }
    }

    public void SetIdle(bool value)
    {
        // Idle is handled by disabling walking
        if (animator != null)
        {
            animator.SetBool(walkParameter, !value);
        }
    }

    public void SetWalking(bool value)
    {
        if (animator != null)
        {
            animator.SetBool(walkParameter, value);
        }
    }

    public void SetRunning(bool value)
    {
    }

    public void PlayAttack()
    {
        // This is for the melee attack
        if (animator != null)
        {
            animator.SetTrigger(meleeAttackTrigger);
        }
    }

    public void PlaySpikeAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger(spikeAttackTrigger);
        }
    }

    public void PlayHit()
    {
        if (animator != null)
        {
            animator.SetTrigger(hitTrigger);
        }
    }

    public void PlayDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger(deathTrigger);
        }
    }

    // Method to reset all triggers (useful to avoid bugs)
    public void ResetAllTriggers()
    {
        if (animator != null)
        {
            animator.ResetTrigger(meleeAttackTrigger);
            animator.ResetTrigger(spikeAttackTrigger);
            animator.ResetTrigger(hitTrigger);
            animator.ResetTrigger(deathTrigger);
        }
    }
}