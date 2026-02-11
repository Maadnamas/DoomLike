using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomAnimationOffset : MonoBehaviour
{
    void Awake()
    {
        Animator animator = GetComponent<Animator>();
        animator.Play(0, 0, Random.value);
    }
}
