using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class LizardWallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    [SerializeField]
    private LayerMask whatIsWall;
    [SerializeField]
    private float wallrunSpeed = 10;
    [SerializeField]
    private float staminaDrain = 0.25f;

    [Header("Input")]
    private float _horizontalInput;
    private float _verticalInput;
    private bool _jumpInput;
    private bool _activateKeyInput;

    [Header("Detection")]
    [SerializeField]
    private float wallCheckDistance = 1f;

    [Header("Keybinds")]
    [SerializeField]
    private KeyCode activateKey = KeyCode.Mouse0;
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    private RaycastHit _wallHitPoint;
    private bool _wallHit;
    private Vector3 _moveDirection;

    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform wallDirection;
    private PlayerMovement _pm;
    private Rigidbody _rb;

    private void Start()
    {
        _pm = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        MyInput();
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (_pm.GetWallrunning())
        {
            WallRunningMovment();
        }
    }

    private void CheckForWall()
    {
        if(_pm.GetWallrunning())
        {
            _wallHit = Physics.Raycast(transform.position, -orientation.up, out _wallHitPoint, wallCheckDistance, whatIsWall);
            
        }
        else
        {
            _wallHit = Physics.Raycast(transform.position, orientation.forward, out _wallHitPoint, wallCheckDistance, whatIsWall);
            if (_wallHit)
            {
                wallDirection.up = _wallHitPoint.normal;
            }
        }
        
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _jumpInput = Input.GetKey(jumpKey);
        _activateKeyInput = Input.GetKey(activateKey);
    }

    private void StateMachine()
    {
        if(_wallHit && _activateKeyInput && _pm.GetStamina(2) > 0)
        {
            if (!_pm.GetWallrunning())
            {
                StartWallRun();
            }
        } else
        {
            if (_pm.GetWallrunning())
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        _pm.SetWallrunning(true);
        _rb.useGravity = false;
        _rb.velocity = new Vector3(_rb.velocity.x,0f,_rb.velocity.z);
    }

    private void WallRunningMovment()
    {
        _pm.SetStamina(2,_pm.GetStamina(2) - staminaDrain);
        _moveDirection = orientation.forward * _verticalInput  + -orientation.right * _horizontalInput;
        _rb.AddForce(_moveDirection.normalized * wallrunSpeed * 10f , ForceMode.Force);
    }

    private void StopWallRun()
    {
        _pm.SetWallrunning(false);
    }
}
