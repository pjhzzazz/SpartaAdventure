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
    Attack,
    Death
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;
    private float runSpeed => moveSpeed * 2f;
    public PlayerState currentState;
    public float checkDistance = 1.0f;
    public float climbDelay = 0.4f;
    [SerializeField] private float maxClimbHeight = 2.0f;
    public LayerMask ledgeLayer;
    private bool isGrabbing = false;
    private float holdTime = 0f;
    private float useStamina;
    [SerializeField] private CapsuleCollider playerCollider;

    private float originalHeight;
    private Vector3 originalCenter;
    
    [Header("Look")] public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;
    
    public GameObject FirstPersonCamera;
    public GameObject ThirdPersonCamera;

    private bool isFirstPerson = true;

    public Action Inventory;
    
    [Header("Combat")]
    public float attackRate;
    private bool isattacking;
    public float attackDistance;
    public float damage;
    
    [HideInInspector]
    private Rigidbody _rigidbody;
    
    Animator animator;

    private bool isJumping;
    private bool isSprinting;
    private bool isCrouching;
    private bool isRolling;
    private float rollDuration = 1f;
    public Transform modelTransform;
    private float rollPower = 5f;
    private PlayerCondition condition;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        condition = CharacterManager.Instance.Player.condition;
        originalHeight = playerCollider.height;
        originalCenter = playerCollider.center;
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
                Stamina(15);
                break;
            case PlayerState.Jump:
                break;
            case PlayerState.Crouch:
                break;
            case PlayerState.Roll:
                Stamina(20);
                break;
            case PlayerState.Attack:
                break;
            case PlayerState.Death:
                break;
        }

        if (!isGrabbing && Input.GetKeyDown(KeyCode.Space)) 
        {
            Vector3 origin = transform.position + Vector3.up * 1.5f;

            if (CanClimb(origin, out RaycastHit hit))
            {
                StartCoroutine(ClimbUp());
            }
            else
            {
                CancelGrab(); 
            }
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
        if (isRolling || isGrabbing) return;
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
        if (context.phase == InputActionPhase.Performed)
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
        if (context.phase == InputActionPhase.Started && curMovementInput.sqrMagnitude > 0.1f && IsGrounded())
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
            if (isCrouching || isRolling)
                return;
            print("JUMP");
            StartCoroutine(JumpCoroutine());
        }
    }
    private IEnumerator JumpCoroutine()
    {
        isJumping = true;
        _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        animator.SetBool("isJumping", true);
        animator.SetBool("isGrounded", false);

        yield return new WaitForSeconds(0.3f); // 점프 직후 IsGrounded가 true가 되는것을 막기 위해 0.3초 대기

        yield return new WaitUntil(IsGrounded); // 땅에 착지할때까지 대기

        isJumping = false;
        animator.SetBool("isJumping", false);
        animator.SetBool("isGrounded", true);
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
            if (isCrouching)
            {
                playerCollider.height = originalHeight * 0.8f;
                playerCollider.center = new Vector3(originalCenter.x, originalCenter.y / 2f, originalCenter.z);
            }
            else
            {
                playerCollider.height = originalHeight;
                playerCollider.center = originalCenter;
            }
            currentState = isCrouching ? PlayerState.Crouch : PlayerState.Walk;
            animator.SetInteger("State", (int)currentState);
        }
    }
    
    public void OnInventoryInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Inventory?.Invoke();
            ToggleCursor();
        }
    }

    public void OnCameraSwitchInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isFirstPerson = !isFirstPerson;

            if (isFirstPerson)
            {
                FirstPersonCamera.SetActive(true);
                ThirdPersonCamera.SetActive(false);
            }
            else
            {
                FirstPersonCamera.SetActive(false);
                ThirdPersonCamera.SetActive(true);
            }
        }
    }
    IEnumerator RollRoutine()
    {
        isRolling = true;
        currentState = PlayerState.Roll;
        Vector3 moveDir = _rigidbody.velocity;
        moveDir.y = 0; 
        
        _rigidbody.velocity += moveDir.normalized * rollPower;
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

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
    void CheckLedge()
    {
        
    }
    
    void CancelGrab()
    { 
        isGrabbing = false;
        _rigidbody.isKinematic = false;
    }
    
    IEnumerator ClimbUp()
    {
        isGrabbing = true;
        _rigidbody.isKinematic = true;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * 2.0f + transform.forward * 0.7f;

        float time = 0f;
        float duration = 1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, time / duration);
            yield return null;
        }

        isGrabbing = false;
        _rigidbody.isKinematic = false;
    }

    public void SpeedUp(float value, float duration)
    {
        StartCoroutine(SpeedUpRoutine(value, duration));
    }

    private IEnumerator SpeedUpRoutine(float value, float duration)
    {
        moveSpeed += value;
        yield return new WaitForSeconds(duration);
        moveSpeed -= value;
    }

    private void Stamina(float value)
    {
        useStamina = value * Time.deltaTime;
        condition.UseStamina(useStamina);
        if (condition.StaminaIsEmpty())
        {
            currentState = PlayerState.Walk;
            animator.SetInteger("State", (int)currentState);
        }
    }

    public void OnAttackInput()
    {
        if (!isattacking)
        {
            isattacking = true;
            Invoke("OnCanAttack", attackRate);
        }
    }

    void OnCanAttack()
    {
        isattacking = false;
        animator.SetBool("isAttacking", false);
    }

    public void AddDamage(float value)
    {
        damage += value;
    }
    
    public void SpeedUp(float value)
    {
        moveSpeed += value;
    }
    
    private bool CanClimb(Vector3 origin, out RaycastHit ledgeHit)
    {
        if (Physics.Raycast(origin, transform.forward, out ledgeHit, checkDistance, ledgeLayer))
        {
            Vector3 topCheckOrigin = ledgeHit.point + Vector3.up * 1.5f;
            if (Physics.Raycast(topCheckOrigin, Vector3.up, 1f, ledgeLayer))
            {
                float height = topCheckOrigin.y - transform.position.y;
                if (height > maxClimbHeight)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
}
