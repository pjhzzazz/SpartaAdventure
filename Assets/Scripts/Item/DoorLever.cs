using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorLever : MonoBehaviour, IInteractable
{
    public DoorAnimation targetDoor;
    private Animator leverAnimator;
    public TextMeshProUGUI text;
    private bool isActivated = false;

    private void Awake()
    {
        leverAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        targetDoor = GetComponentInParent<DoorAnimation>();
    }

    public string GetInteractPrompt()
    {
        return text.text;
    }

    public void OnInteract()
    {
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        isActivated = !isActivated;

        if (leverAnimator != null)
        {
            if (isActivated)
            {
                isActivated = true;
                leverAnimator.SetTrigger("Activate");
                targetDoor.PlayOpenAnimation();
                
            }

            else
            {
                isActivated = false;
                leverAnimator.SetTrigger("Deactivate");
                targetDoor.PlayCloseAnimation();
            }
                
        }
    }
}