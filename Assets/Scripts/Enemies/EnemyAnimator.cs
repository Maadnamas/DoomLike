using UnityEngine;

public class EnemyAnimator : MonoBehaviour, IEnemyAnimator
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void SetWalking(bool walking)
    {
        animator.SetBool("IsWalking", walking);
    }

    public void SetIdle(bool idle)
    {
        animator.SetBool("IsIdle", idle);
    }

    public void PlayHit()
    {
        animator.SetTrigger("Hit");
    }

    public void PlayDeath()
    {
        animator.SetTrigger("Death");
    }
}