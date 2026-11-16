using System;
using UnityEngine;
using UnityEngine.UI;
public class Good : MonoBehaviour
{
    private Animator goodAnimator;
    public event Action<GameObject> used;
    private void Start(){
        goodAnimator = gameObject.GetComponent<Animator>();
    }
    private void OnEnable()
    {
        gameObject.transform.position = new Vector3(0,0,0);
        // 播放動畫
        goodAnimator.Play("GoodAnimation", 0, 0);
    }
    
    private void Update()
    {
        AnimatorStateInfo stateInfo = goodAnimator.GetCurrentAnimatorStateInfo(0);

        // 如果動畫已經播完且不在過渡中
        if (stateInfo.normalizedTime >= 1f && !goodAnimator.IsInTransition(0))
        {
            used?.Invoke(gameObject);
        }
    }
}
