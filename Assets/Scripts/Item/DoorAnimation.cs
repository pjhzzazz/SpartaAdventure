using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    private bool isOpened = false;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayOpenAnimation()
    {
        if (isOpened) return;
        isOpened = true;
        animator.SetTrigger("Open");
    }

    public void PlayCloseAnimation()
    {
        if (!isOpened) return;
        isOpened = false;
        animator.SetTrigger("Close");
    }
}
