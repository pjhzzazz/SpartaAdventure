using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EquipSlot
{
    RightHand,
    LeftHand,
    Back,
    Body
}
public class Equipment : MonoBehaviour
{
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public Transform backTransform;
    public Transform bodyTransform;

    private PlayerController controller;
    private PlayerCondition condition;
    private Animator animator;
    private GameObject currentEquippedObject;
    
    void Start()
    {
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        animator = GetComponentInChildren<Animator>();
    }

    public void EquipNew(ItemData data)
    {
        UnEquip();
        Transform equipPosition = GetEquipTransform(data.equipSlot);
        currentEquippedObject = Instantiate(data.equipPrefab, equipPosition);
    }

    public void UnEquip()
    {
        if (currentEquippedObject != null)
        {
            Destroy(currentEquippedObject);
            currentEquippedObject = null;
        }
    }

    private Transform GetEquipTransform(EquipSlot slot)
    {
        switch (slot)
        {
            case EquipSlot.RightHand:
                return rightHandTransform;
            case EquipSlot.LeftHand:
                return leftHandTransform;
            case EquipSlot.Back:
                return backTransform;
            case EquipSlot.Body:
                return bodyTransform;
            default:
                return null;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed &&currentEquippedObject != null && controller.canLook)
        {
            animator.SetBool("isAttacking", true);
            controller.OnAttackInput();
        }
    }
    
    
}
