using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ClickPlayAnimation : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        if (animator)
        {
            animator.SetTrigger("PlayMove"); // ��Ӧ���� Animator �ﴴ���� Trigger
        }
    }
}
