using System.Collections;
using UnityEngine;

public class MenuRabbitSequence : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator gunAnimator;

    private void Start()
    {
        StartCoroutine(LoopSequence());
    }

    private IEnumerator LoopSequence()
    {
        while (true)
        {
            yield return PlayAndWait(playerAnimator, "MainMenuFlip");
            yield return PlayAndWait(gunAnimator, "Reload");
        }
    }

    private IEnumerator PlayAndWait(Animator animator, string stateName)
    {
        animator.Play(stateName);

        // Wait until state is entered
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // Wait until animation finishes
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }
}
