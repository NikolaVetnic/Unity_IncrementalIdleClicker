using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PopUpText : MonoBehaviour
{
    public Text popUpText;
    Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        AnimatorClipInfo[] info = anim.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, info[0].clip.length);
    }

    public void ShowInfo(int amount)
    {
        popUpText.text = "+" + amount;
    }
}
