using UnityEngine;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }
}