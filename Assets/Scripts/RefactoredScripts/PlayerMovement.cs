using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float walkSpeed = 7;
    [SerializeField]
    private float wallrunSpeed = 10;
    [SerializeField]
    private float groundDrag = 5;
    [SerializeField]
    private float wallDrag = 5;
    [SerializeField]
    private float jumpForce = 6;
    [SerializeField]
    private float jumpCooldown = 0.25f;
    [SerializeField]
    private float airMultiplier = 0.4f;


    [Header("Keybinds")]
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    [SerializeField]
    private float playerHeight = 0.7f;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private float groundTolerance = 0.3f;

    [Header("Slope Handling")]
    [SerializeField]
    private float maxSlopeAngle = 40f;
    
    
    [SerializeField]
    private Transform orientation;

    private RaycastHit _slopeHit;
    private bool _grounded;
    private bool _exitingSlope;
    private float _maxMoveSpeed;
    private Vector3 _moveDirection;
    private Rigidbody _rb;
    private bool _readyToJump = true;

    [Header("Input")]
    private float _horizontalInput;
    private float _verticalInput;
    private bool _jumpInput;

    private MovementStat _moveState;


    private AnimalType _animalType = AnimalType.LIZARD;
    [SerializeField]
    private float staminaRecoveryRate = 0.1f;
    [SerializeField]
    private bool debugStamina = false;
    private float _maxStamina = 100;
    private float[] _stamina;

    public enum MovementStat
    {
        walking,
        wallrunning,
        air
    }

    private bool _wallrunning;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _stamina = new float[4];
        for (int i = 0; i < _stamina.Length; i++)
        {
            _stamina[i] = _maxStamina;
        }
    }

    private void Update()
    {
        groundCheck();
        MyInput();
        SpeedControl();
        StateHandler();
        DragOn();
        RecoverStamina();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _jumpInput = Input.GetKey(jumpKey);
    }

    private void groundCheck() 
    {
        _grounded = Physics.Raycast(transform.position,Vector3.down,playerHeight * 0.5f + groundTolerance, whatIsGround); 
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down,out _slopeHit, playerHeight * 0.5f + groundTolerance))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
    }

    private void DragOn()
    {
        if (_grounded)
        {
            _rb.drag = groundDrag;
        }
        else if (_wallrunning)
        {
            _rb.drag = wallDrag;
        } 
        else
        {
            _rb.drag = 0f;
        }
    }

    private void StateHandler()
    {
        if (_wallrunning)
        {
            _moveState = MovementStat.wallrunning;
            _maxMoveSpeed = wallrunSpeed;
        } else if (_grounded)
        {
            _moveState = MovementStat.walking;
            _maxMoveSpeed = walkSpeed;
        } else if (!_grounded)
        {
            _moveState = MovementStat.air;
        }
    }

    private void RecoverStamina()
    {
        for (int i = 0; i < _stamina.Length; i++)
        {
            if ((int) _animalType != i)
            {
                _stamina[i] = Mathf.Min(_stamina[i] + staminaRecoveryRate, _maxStamina);
            }
            if (debugStamina)
            {
                _stamina[i] += _maxStamina;
            }
        }
    }

    private void MovePlayer()
    {
        if (_wallrunning) return;

        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (_jumpInput && _readyToJump && _grounded)
        {
            _readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (OnSlope() && !_exitingSlope)
        {
            _rb.AddForce(GetSlopeMoveDirection() * walkSpeed * 20f, ForceMode.Force);
            if(_rb.velocity.y > 0f)
            {
                _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        } else if (_grounded )
        {
            _rb.AddForce(_moveDirection.normalized * walkSpeed * 10f, ForceMode.Force);
        } else if(!_grounded)
        {
            _rb.AddForce(_moveDirection.normalized * walkSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        _rb.useGravity = !OnSlope();

    }

    private void Jump()
    {
        _exitingSlope = true;

        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        _rb.AddForce(transform.up * jumpForce,ForceMode.Impulse);
    }


    private void ResetJump()
    {
        
        _readyToJump = true;

        _exitingSlope = false;
    }

    private void SpeedControl()
    {
        if(OnSlope() && !_exitingSlope)
        {
            if(_rb.velocity.magnitude > _maxMoveSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _maxMoveSpeed;
            }
        } else if(_wallrunning)
        {
            if (_rb.velocity.magnitude > _maxMoveSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _maxMoveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            if (flatVel.magnitude > _maxMoveSpeed)
            {
                Vector3 limiteVel = flatVel.normalized * _maxMoveSpeed;
                _rb.velocity = new Vector3(limiteVel.x, _rb.velocity.y, limiteVel.z);
            }
        }
    }

    public float getVelocity()
    {
        return (float) System.Math.Round(new Vector3(_rb.velocity.x, 0f, _rb.velocity.z).magnitude,2);

    }

    public string getMoveStateString()
    {
        return _moveState.ToString();
    }

    public void SetWallrunning(bool wallrunning)
    {
        _wallrunning = wallrunning;
    }

    public bool GetWallrunning()
    {
        return _wallrunning;
    }

    public float GetStamina(int index)
    {
        return _stamina[index];
    }

    public void SetStamina(int index,float stamina)
    {
        _stamina[index] = Mathf.Max(stamina,0);
    }
}
