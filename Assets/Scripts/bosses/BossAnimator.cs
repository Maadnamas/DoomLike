using UnityEngine;

public class BossAnimator : MonoBehaviour, IEnemyAnimator
{
    private Animator animator;

    [Header("Nombres de Parámetros del Animator")]
    [SerializeField] private string walkParameter = "camina";
    [SerializeField] private string meleeAttackTrigger = "ataque1";
    [SerializeField] private string spikeAttackTrigger = "ataque2";
    [SerializeField] private string hitTrigger = "hit";
    [SerializeField] private string deathTrigger = "muerte";

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("No se encontró Animator en el Boss: " + gameObject.name);
        }
    }

    public void SetIdle(bool value)
    {
        // Idle se maneja desactivando el caminar y correr
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
        // Este es para el ataque melee
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

    // Método para resetear todos los triggers (útil para evitar bugs)
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