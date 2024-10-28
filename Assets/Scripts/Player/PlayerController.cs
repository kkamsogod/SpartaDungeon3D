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

    [Header("Effects")]
    public float speedMultiplier = 1f;
    public bool enableDoubleJump = false;
    public bool isInvincible = false;

    private bool canDoubleJump = false;

    public Action inventory;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
        float currentSpeed = (isRunning ? runSpeed : moveSpeed) * speedMultiplier;
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
            if (IsGrounded())
            {
                _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                canDoubleJump = true;
            }
            else if (enableDoubleJump && canDoubleJump)
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
            isRunning = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isRunning = false;
        }
    }

    public void StartSpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(ApplySpeedBoost(multiplier, duration));
    }

    public void StartDoubleJump(float duration)
    {
        StartCoroutine(ApplyDoubleJump(duration));
    }

    public void StartInvincibility(float duration)
    {
        StartCoroutine(ApplyInvincibility(duration));
    }

    private IEnumerator ApplySpeedBoost(float speedMultiplier, float duration)
    {
        Debug.Log("Speed Boost Activated");
        moveSpeed *= speedMultiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed /= speedMultiplier;
        Debug.Log("Speed Boost Deactivated");
    }

    private IEnumerator ApplyDoubleJump(float duration)
    {
        Debug.Log("Double Jump Activated");
        enableDoubleJump = true;
        yield return new WaitForSeconds(duration);
        enableDoubleJump = false;
        Debug.Log("Double Jump Deactivated");
    }

    private IEnumerator ApplyInvincibility(float duration)
    {
        Debug.Log("Invincibility Activated");
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("Invincibility Deactivated");
    }
}