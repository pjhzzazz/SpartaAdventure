using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Walk,
    Sprint,
    Jump,
    Crouch,
    Roll,
    Death
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;
    public float runSpeed;
    public PlayerState currentState;

    [Header("Look")] public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;

     [HideInInspector]
    private Rigidbody _rigidbody;
    
    Animator animator;

    private bool isJumping;
    private bool isSprinting;
    private bool isCrouching;
    private bool isRolling;
    private float rollDuration = 1f;
    public Transform modelTransform;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Walk:
                break;
            case PlayerState.Sprint:
                break;
            case PlayerState.Jump:
                break;
            case PlayerState.Crouch:
                break;
            case PlayerState.Roll:
                break;
            case PlayerState.Death:
                break;
        }

        animator.SetBool("isGrounded", IsGrounded());
        if (IsGrounded())
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
        }
    }

    private void FixedUpdate()
    {
       Move();
       RotateModel();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };
        
        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    private void Move()
    {
        if (isRolling) return;
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
        float speed = isCrouching ? moveSpeed * 0.5f : currentState == PlayerState.Sprint ? runSpeed : moveSpeed;
        dir *= speed;
        dir.y = _rigidbody.velocity.y;
        
        _rigidbody.velocity = dir;
        
        if (isCrouching)
        {
            currentState = PlayerState.Crouch;
        }
        else
        {
            if (flatDir.sqrMagnitude < 0.01f)
            {
                currentState = PlayerState.Idle;
            }
            else
            {
                if(!isJumping)
                    currentState = isSprinting ? PlayerState.Sprint : PlayerState.Walk;
            }
        }
        animator.SetInteger("State", (int)currentState);
    }
    private void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }       
            
    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    } 
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (isRolling) return;
        if (context.phase == InputActionPhase.Performed )
        {
            curMovementInput = context.ReadValue<Vector2>();
            currentState = PlayerState.Walk;
            if(!isCrouching && !isSprinting && !isRolling && !isJumping)
                animator.SetInteger("State", (int)currentState);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
            currentState = PlayerState.Idle;
            animator.SetInteger("State", (int)currentState);
        }
    }

    public void OnSprintInput(InputAction.CallbackContext context)
    {
        if (isRolling) return;
        if (context.phase == InputActionPhase.Started && curMovementInput.sqrMagnitude > 0.1f)
        {
            isSprinting = true;
            currentState = PlayerState.Sprint;
            animator.SetInteger("State", (int)currentState);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isSprinting = false;
            currentState = PlayerState.Walk;
            animator.SetInteger("State", (int)currentState);
        }
    }
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            isJumping = true;
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
        
        animator.SetBool("isJumping", true);
        animator.SetBool("isGrounded", false);
    }
    
    public void OnRollInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !isRolling)
        {
            StartCoroutine("RollRoutine");
        }
    }

    public void OnCrouchInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isCrouching = !isCrouching;
            
            currentState = isCrouching ? PlayerState.Crouch : PlayerState.Walk;
            animator.SetInteger("State", (int)currentState);
        }
    }
    IEnumerator RollRoutine()
    {
        isRolling = true;
        currentState = PlayerState.Roll;
        animator.SetInteger("State", (int)currentState);;

        yield return new WaitForSeconds(rollDuration); 
        isRolling = false;
    }
 
    private void RotateModel()
    {
        Vector3 moveDir = _rigidbody.velocity;
        moveDir.y = 0;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }
}
