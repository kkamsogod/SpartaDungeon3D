using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float runSpeed;
    public float jumpPower;
    private Vector2 _curMovementInput;
    private bool isRunning;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float _camCurXRot;
    public float lookSensitivity;
    private Vector2 _mouseDelta;
    public bool canLook = true;

    private float originalSpeed;
    private bool canDoubleJump = false;
    public bool enableDoubleJump = false;

    [Header("Cost")]
    public float runStaminaCost;
    public float jumpStaminaCost;

    public Action inventory;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        originalSpeed = moveSpeed;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (isRunning && _curMovementInput != Vector2.zero && IsGrounded())
        {
            bool hasEnoughStamina = CharacterManager.Instance.Player.condition.UseStamina(runStaminaCost * Time.deltaTime);
            if (!hasEnoughStamina)
            {
                isRunning = false;
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    private void Move()
    {
        float currentSpeed = (isRunning ? runSpeed : moveSpeed);
        Vector3 dir = transform.forward * _curMovementInput.y + transform.right * _curMovementInput.x;
        dir *= currentSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    private void CameraLook()
    {
        _camCurXRot += _mouseDelta.y * lookSensitivity;
        _camCurXRot = Mathf.Clamp(_camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-_camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, _mouseDelta.x * lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (IsGrounded() && CharacterManager.Instance.Player.condition.UseStamina(jumpStaminaCost))
            {
                _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                canDoubleJump = true;
            }
            else if (enableDoubleJump && canDoubleJump && CharacterManager.Instance.Player.condition.UseStamina(jumpStaminaCost))
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
                _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                canDoubleJump = false;
            }
        }
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    private void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (IsGrounded())
            {
                isRunning = true;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isRunning = false;
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        moveSpeed = originalSpeed * multiplier;
    }

    public void ResetSpeedMultiplier()
    {
        moveSpeed = originalSpeed;
    }

    public void EnableDoubleJump(bool enable)
    {
        enableDoubleJump = enable;
        canDoubleJump = enable;
    }
}
