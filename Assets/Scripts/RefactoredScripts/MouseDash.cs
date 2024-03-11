using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class MouseDash : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform playerCam;
    [SerializeField]
    private AnimalType aniaml = AnimalType.MOUSE;

    private Rigidbody _rb;
    private PlayerMovement _pm;

    [Header("Dashing")]
    [SerializeField]
    private float dashForce = 20;
    [SerializeField]
    private float dashUpwardsForce = 5;
    [SerializeField]
    private float dashDuration = 0.25f;
    [SerializeField]
    private float staminaDrain = 25f;

    [Header("Settings")]
    [SerializeField]
    private bool useCameraForward = true;
    [SerializeField]
    private bool allowAllDirections = true;
    [SerializeField]
    private bool disableGravity = false;
    [SerializeField]
    private bool resetVel = true;

    [Header("Cooldown")]
    [SerializeField]
    private float dashCd = 1.5f;
    private float _dashCdTimer;

    [Header("Keybinds")]
    [SerializeField]
    private KeyCode SpecialKey = KeyCode.Mouse0;

    [Header("Input")]
    private bool _specialInput;
    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _forceToApply;

    // Start is called before the first frame update
    public void Setup()
    {
        _rb = GetComponent<Rigidbody>();
        _pm = GetComponent<PlayerMovement>();
    }

    public void SwitchOf()
    {
        EndDash();
        _dashCdTimer = 0f;
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        _dashCdTimer = Mathf.Max(0f, _dashCdTimer - Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if(_specialInput && _pm.GetStamina((int)aniaml) > staminaDrain)
        {
            Dash();
        }
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _specialInput = Input.GetKey(SpecialKey);
    }

    private void Dash()
    {
        if (_dashCdTimer > 0f) return;
        else _dashCdTimer = dashCd;

        _pm.SetDashing(true);
        _pm.SetStamina((int)aniaml, _pm.GetStamina((int) aniaml) - staminaDrain);
        _dashCdTimer = dashCd;

        Transform forwardT;
        if (useCameraForward)
        {
            forwardT = playerCam;
        }
        else
        {
            forwardT= orientation;
        }

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + direction * dashUpwardsForce;

        _rb.useGravity = !disableGravity;

        _forceToApply = forceToApply;
        Invoke(nameof(DelayedForce), 0.1f);
        Invoke(nameof(EndDash), dashDuration);
    }

    private void DelayedForce()
    {
        if (resetVel)
        {
            _rb.velocity = Vector3.zero;
        }
        _rb.AddForce(_forceToApply, ForceMode.Impulse);
    }

    private void EndDash()
    {
        _rb.useGravity = true;
        _pm.SetDashing(false);
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        Vector3 direction = new Vector3();

        if (!allowAllDirections || (_verticalInput == 0 && _horizontalInput == 0))
        {
            direction = forwardT.forward;
        } 
        else
        {
            direction = forwardT.forward * _verticalInput + forwardT.right * _horizontalInput;
        }

        return direction.normalized;
    }
}
