using UnityEngine;

public class RogerAnime : MonoBehaviour
{
    private Animator rogerAnimator;

    private void Start() {
        this.rogerAnimator = this.gameObject.GetComponent<Animator>();
    }
    // 下壓動畫
    public void PlayDown()
    {
        if (rogerAnimator != null)
        {
            rogerAnimator.SetTrigger("downTrigger");
        }
    }

    // 上放動畫
    public void PlayUp()
    {
        if (rogerAnimator != null)
        {
            rogerAnimator.SetTrigger("upTrigger");
        }
    }
    public void switchIdle(bool p)
    {
        if (rogerAnimator != null)
        {
            rogerAnimator.SetBool("idleBool",p);
        }
    }
}
