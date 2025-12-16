using UnityEngine;

public class EnemyAnimator : MonoBehaviour, IEnemyAnimator
{
    [SerializeField] private Animator animator;
    private AudioSource audioSource;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;

        audioSource = GetComponent<AudioSource>();
    }

    private void StopWalkSound()
    {
        if (audioSource != null && audioSource.isPlaying && audioSource.loop)
        {
            audioSource.Stop();
        }
    }

    public void SetWalking(bool walking)
    {
        animator.SetBool("IsWalking", walking);
        if (!walking) StopWalkSound();
    }

    public void SetIdle(bool idle)
    {
        animator.SetBool("IsIdle", idle);
        if (idle) StopWalkSound();
    }

    public void SetRunning(bool running)
    {
        animator.SetBool("IsRunning", running);
        if (!running) StopWalkSound();
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void PlayHit()
    {
        animator.SetTrigger("Hit");
    }

    public void PlayDeath()
    {
        animator.SetTrigger("Death");
        StopWalkSound();
    }
}