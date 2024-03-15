using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SnakeSwing))]
[RequireComponent (typeof(MouseDash))]
[RequireComponent(typeof(LizzardClimbing))]
[RequireComponent(typeof (BirdGlide))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameManagementRefactored gameManagement;
    [SerializeField]
    private Transform orientation;

    private Rigidbody _rb;
    private SnakeSwing _snakeSwing;
    private MouseDash _mouseDash;
    private LizzardClimbing _lizzardClimbing;
    private BirdGlide _birdGlide;


    [Header("Movement")]
    [SerializeField]
    private float walkSpeed = 7;
    [SerializeField]
    private float walkSpeedChangeFactor = 50f;
    [SerializeField]
    private float airSpeed = 10;
    [SerializeField]
    private float airSpeedChangeFactor = 5f;
    [SerializeField]
    private float climbingSpeed = 10;
    [SerializeField]
    private float climbingSpeedChangeFactor = 5f;
    [SerializeField]
    private float dashSpeed = 20;
    [SerializeField]
    private float dashSpeedChangeFactor = 20f;
    [SerializeField]
    private float swingSpeed = 14f;
    [SerializeField]
    private float swingSpeedChangeFactor = 5f;
    [SerializeField]
    private float grapplingSpeedChangeFactor = 5f;
    [SerializeField]
    private float glideSpeed = 20f;
    [SerializeField]
    private float glideDamp = 1f;
    [SerializeField]
    private float glideSpeedChangeFactor = 5f;
    [SerializeField]
    private float groundDrag = 5;
    [SerializeField]
    private float climbingDrag = 5;
    [SerializeField]
    private float jumpForce = 6;
    [SerializeField]
    private float jumpCooldown = 0.25f;
    [SerializeField]
    private float airMultiplier = 0.2f;

    private float _maxMoveSpeed;

    [Header("Stamina")]
    [SerializeField]
    private float staminaRecoveryRate = 10f;
    [SerializeField]
    private bool debugStamina = false;

    private float _maxStamina = 100;
    private float[] _stamina;

    [Header("Keybinds")]
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;
    [SerializeField]
    private KeyCode swichLeft = KeyCode.Q;
    [SerializeField]
    private KeyCode swichRight = KeyCode.E;

    [Header("Ground Check")]
    [SerializeField]
    private float playerHeight = 0.7f;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private float groundTolerance = 0.2f;

    [Header("Slope Handling")]
    [SerializeField]
    private float maxSlopeAngle = 40f;

    private RaycastHit _slopeHit;
    private bool _exitingSlope;

    
    [Header("Input")]
    private float _horizontalInput;
    private float _verticalInput;
    private bool _jumpInput;
    private bool _switchLeftInput;
    private bool _switchRightInput;
    private float _lastSwitch;
    private Vector3 _moveDirection;


    [Header("States")]
    private AnimalType _animalType = AnimalType.SNAKE;
    private MovementStat _moveState;

    [Header("Paricle Systems")]
    [SerializeField]
    private ParticleSystem switchParticles;
    [SerializeField]
    private ParticleSystem moveParticles;
    [SerializeField]
    private ParticleSystem jumpParticles;
    [SerializeField]
    private ParticleSystem dashParticles;

    [Header("Sounds")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip jumpClip;
    [SerializeField]
    private AudioClip dashClip;
    [SerializeField]
    private AudioClip swingClip;
    [SerializeField]
    private AudioClip grappleClip;

    public enum MovementStat
    {
        walking,
        climbing,
        dashing,
        swinging,
        grappling,
        gliding,
        air
    }

    private bool _readyToJump = true;
    private bool _isJumping;
    private bool _isClimbing;
    private bool _isDashing;
    private bool _isSwinging;
    private bool _isGrounded;
    private bool _isGrappling;
    private bool _isGliding;

    private float _desiredMoveSpeed;
    private float _lastDesiredMoveSpeed;
    private MovementStat _lastState;
    private float _speedChangeFactor = 1f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _desiredMoveSpeed = walkSpeed;
        _stamina = new float[4];
        for (int i = 0; i < _stamina.Length; i++)
        {
            _stamina[i] = _maxStamina;
        }

        _snakeSwing = GetComponent<SnakeSwing>();
        _mouseDash = GetComponent<MouseDash>();
        _lizzardClimbing = GetComponent<LizzardClimbing>();
        _birdGlide = GetComponent<BirdGlide>();

        _snakeSwing.Setup();
        _mouseDash.Setup();
        _lizzardClimbing.Setup();
        _birdGlide.Setup();

        ToggleAniamMoves();
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
        Swich();
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _jumpInput = Input.GetKey(jumpKey);
        _switchLeftInput = Input.GetKey(swichLeft);
        _switchRightInput = Input.GetKey(swichRight);
    }

    private void groundCheck() 
    {
        _isGrounded = Physics.Raycast(transform.position,Vector3.down,playerHeight * 0.5f + groundTolerance, whatIsGround); 
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
        if(_isDashing)
        {
            _rb.drag = 0f;
        }
        else if (_isGrappling)
        {
            _rb.drag = 0f;
        }
        else if (_isGrounded)
        {
            _rb.drag = groundDrag;
        }
        else if (_isClimbing)
        {
            _rb.drag = climbingDrag;
        } 
        else
        {
            _rb.drag = 0f;
        }
    }

    private void StateHandler()
    {
        moveParticles.enableEmission = false;
        if (_isDashing)
        {
            dashParticles.Play();
            _moveState = MovementStat.dashing;
            _desiredMoveSpeed = dashSpeed;
            _speedChangeFactor = dashSpeedChangeFactor;
        } 
        else if (_isGliding)
        {
            _moveState = MovementStat.gliding;
            _desiredMoveSpeed = glideSpeed;
            _speedChangeFactor = glideSpeedChangeFactor;
        }
        else if (_isGrappling)
        {
            _moveState = MovementStat.grappling;
            _speedChangeFactor = grapplingSpeedChangeFactor;
        }
        else if (_isSwinging)
        {
            _moveState = MovementStat.swinging;
            _desiredMoveSpeed = swingSpeed;
            _speedChangeFactor = swingSpeedChangeFactor;
        }
        else if (_isClimbing)
        {
            _moveState = MovementStat.climbing;
            if (getVelocity() > 0.5) {
                moveParticles.enableEmission = true;
            }
            _desiredMoveSpeed = climbingSpeed;
        } 
        else if (_isGrounded)
        {
            _moveState = MovementStat.walking;
            if (getVelocity() > 0.5) {
                moveParticles.enableEmission = true;
            }
            _desiredMoveSpeed = walkSpeed;
            _speedChangeFactor = walkSpeedChangeFactor;
        } 
        else if (!_isGrounded)
        {
            _moveState = MovementStat.air;
            _desiredMoveSpeed = airSpeed;
            _speedChangeFactor = airSpeedChangeFactor;
        }

        bool desiredMoveSpeedHasChanged = _desiredMoveSpeed != _lastDesiredMoveSpeed;
        if(desiredMoveSpeedHasChanged)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }

        _lastDesiredMoveSpeed = _desiredMoveSpeed;
        _lastState = _moveState;
    }

    private void Swich()
    {
        if(_switchLeftInput && _lastSwitch != -1)
        {
            _animalType = (AnimalType) (((int) _animalType + 3) % 4);
            _lastSwitch = -1;
            ToggleAniamMoves();
            switchParticles.Play();
        } else if(_switchRightInput && _lastSwitch != 1)
        {
            _animalType = (AnimalType)(((int)_animalType + 1) % 4);
            _lastSwitch = 1;
            ToggleAniamMoves();
            switchParticles.Play();
        }
        if(!_switchLeftInput && !_switchRightInput)
        {
            _lastSwitch = 0;
        }
    }

    private void ToggleAniamMoves()
    {
        if(_animalType == AnimalType.SNAKE)
        {
            _snakeSwing.enabled = true;
            _mouseDash.SwitchOf();
            _lizzardClimbing.SwitchOf();
            _birdGlide.SwitchOf();
        } 
        else if(_animalType == AnimalType.MOUSE)
        {
            _snakeSwing.SwitchOf();
            _mouseDash.enabled = true;
            _lizzardClimbing.SwitchOf();
            _birdGlide.SwitchOf();
        }
        else if (_animalType == AnimalType.LIZARD)
        {
            _snakeSwing.SwitchOf();
            _mouseDash.SwitchOf();
            _lizzardClimbing.enabled = true;
            _birdGlide.SwitchOf();
        }
        else if (_animalType == AnimalType.BIRD)
        {
            _snakeSwing.SwitchOf();
            _mouseDash.SwitchOf();
            _lizzardClimbing.SwitchOf();
            _birdGlide.enabled = true;
        }
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(_desiredMoveSpeed - _maxMoveSpeed);
        float startValue = _maxMoveSpeed;

        float boostFactor = _speedChangeFactor;

        while (time < difference)
        {
            _maxMoveSpeed = Mathf.Lerp(startValue,_desiredMoveSpeed,time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        _maxMoveSpeed = _desiredMoveSpeed;
        _speedChangeFactor = 1f;
    }

    private void RecoverStamina()
    {
        for (int i = 0; i < _stamina.Length; i++)
        {
            if ((int) _animalType != i)
            {
                _stamina[i] = Mathf.Min(_stamina[i] + staminaRecoveryRate * Time.deltaTime, _maxStamina);
            }
            if (debugStamina)
            {
                _stamina[i] = _maxStamina;
            }
        }
    }

    private void MovePlayer()
    {
        if (_isClimbing) return;
        if (_isDashing) return;
        if (_isSwinging) return;
        if(_isGrappling) return;
        if (_isGliding) return;

        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (_jumpInput && _readyToJump && _isGrounded)
        {
            _readyToJump = false;

            Jump();
            jumpParticles.Play();
            audioSource.PlayOneShot(jumpClip);

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (OnSlope() && !_exitingSlope)
        {
            _rb.AddForce(GetSlopeMoveDirection() * walkSpeed * 20f, ForceMode.Force);
            if(_rb.velocity.y > 0f)
            {
                _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        } else if (_isGrounded )
        {
            _rb.AddForce(_moveDirection.normalized * walkSpeed * 6, ForceMode.Force);
        } else if(!_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * walkSpeed * 6 * airMultiplier, ForceMode.Force);
        }

        _rb.useGravity = !OnSlope();

    }

    private void Jump()
    {
        if(OnSlope())
        {
            _exitingSlope = true;
        }
        _isJumping = true;
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
        if(_isGrappling) return;

        if(_isClimbing)
        {
            if (_rb.velocity.magnitude > _maxMoveSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _maxMoveSpeed;
            }
        }
        else if (_isGliding)
        {
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            Vector3 limiteVel = _rb.velocity;
            if (flatVel.magnitude > _maxMoveSpeed)
            {
                limiteVel = flatVel.normalized * _maxMoveSpeed;
            }
            _rb.velocity = new Vector3(limiteVel.x, Mathf.Max(-glideDamp, _rb.velocity.y), limiteVel.z);
        }
        else if (_isDashing)
        {
            if (_rb.velocity.magnitude > _maxMoveSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _maxMoveSpeed;
            }
        }
        else if (OnSlope() && !_exitingSlope && _isGrounded)
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

    public MovementStat getMoveState()
    {
        return _moveState;
    }

    public void SetClimbing(bool climbing)
    {
        _isClimbing = climbing;
    }

    public bool IsClimbing()
    {
        return _isClimbing;
    }

    public Vector3 GetLastWallNormal() {
        return _lizzardClimbing.GetLastWallNormal();
    }

    public void SetDashing(bool dashing)
    {
        _isDashing = dashing;
    }

    public bool IsDashing()
    {
        return _isDashing;
    }

    public void SetSwinging(bool swinging)
    {
        _isSwinging = swinging;
    }

    public bool IsSwinging()
    {
        return _isSwinging;
    }

    public void SetGrappling(bool grappling)
    {
        _isGrappling = grappling;
    }

    public bool IsGrappling()
    {
        return _isGrappling;
    }

    public void SetGliding(bool gliding)
    {
        _isGliding = gliding;
    }

    public bool IsGliding()
    {
        return _isGliding;
    }
    
    public bool IsGroundet()
    {
        return _isGrounded;
    }

    public bool isJumping() {
        return _isJumping;
    }

    public void SetNotJumping() {
        _isJumping = false;
    }

    public float GetStamina(int index)
    {
        return _stamina[index];
    }

    public void SetStamina(int index,float stamina)
    {
        _stamina[index] = Mathf.Max(stamina,0);
    }

    public AnimalType GetAnimalTyp()
    {
        return _animalType;
    }

    public void PlayGrappleClip() {
        audioSource.PlayOneShot(grappleClip);
    }

    public void PlaySwingClip() {
        audioSource.PlayOneShot(swingClip);
    }

    public void PlayDashClip() {
        audioSource.PlayOneShot(dashClip);
    }
}
