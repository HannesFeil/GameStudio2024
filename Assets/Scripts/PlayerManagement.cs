using System;
using UnityEngine;

public class PlayerManagement : MonoBehaviour
{
    [Header("Animals")]
    [SerializeField] 
    private GameObject[] animals;
    
    [SerializeField]
    private AnimalTyps animalTyps = AnimalTyps.SNAKE;

    [SerializeField]
    [Range(0, 90)]
    private float camVerticalClampAngle = 70;
    
    [SerializeField] 
    [Range(0, 1)] 
    private float camSphereRaduis = 0.3f;
    
    [SerializeField]
    [Range(0, 10)]
    private float camDistance = 5;
    
    [SerializeField]
    [Range(0, 10)]
    private float zoomCamDistance = 2;

    [SerializeField]
    [Range(0, 1.5f)]
    private float zoomYOffset = 0.5f;

    [SerializeField]
    [Range(0, 10)]
    private float mouseSensitivityX = 6;
    
    [SerializeField]
    [Range(0, 5)]
    private float mouseSensitivityY = 2.6f;

    [SerializeField]
    [Range(0f, 20f)]
    public float MovementForce = 10;
    
    [SerializeField]
    [Range(0f, 1f)]
    public float AirMovementFactor = 0.5f;

    [SerializeField]
    [Range(100f, 300f)]
    public float JumpForce = 200;

    [SerializeField]
    [Range(0, 20)]
    private int maxGrounded = 10;
    
    private AnimalMove[] animalMove;
    
    private Transform _camTransform;
    private Rigidbody _rigidbody;
    private Transform _transform;

    private int _groundCollisionID;
    private bool _grounded;
    private int _groundedTimer;

    private float _swapped;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _rigidbody = GetComponentInParent<Rigidbody>();
        _transform = GetComponentInParent<Transform>().transform;

        animalMove = new AnimalMove[4];
        for (int i = 0; i < animals.Length; i++)
        {
            animals[i].SetActive((int)animalTyps == i);
            print(animals[i]);
            animalMove[i] = animals[i].GetComponent<AnimalMove>();
            print(animalMove[i]);
            animalMove[i].SetPlayerManagement(this);
        }

        _camTransform = GameObject.Find("Main Camera").GetComponent<Transform>().transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_grounded) {
            _groundedTimer = Mathf.Max(_groundedTimer - 1, -1);
        }
    
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -1*Input.GetAxis("Mouse Y");
        bool specialActive = Input.GetButton("Fire1");
        
        float camXr = _camTransform.rotation.eulerAngles.x + mouseY * mouseSensitivityY;
        float camYr = _camTransform.rotation.eulerAngles.y + mouseX * mouseSensitivityX;

        camXr = Mathf.Clamp(
            (camXr + 90) % 360, 
            90 - camVerticalClampAngle, 
            90 + camVerticalClampAngle
        ) - 90;

        _camTransform.rotation = Quaternion.Euler(camXr, camYr, 0);                

        Vector3 inputVec3 = new Vector3(inputH, 0, inputV);
        inputVec3.Normalize();
        inputVec3 = Quaternion.Euler(0, camYr, 0) * inputVec3;
        
        Vector2 inputVec2 = new Vector2(inputVec3.x,inputVec3.z);
        
        animalMove[(int) animalTyps].Move(inputVec2,specialActive);
    }

    public void CheckSwap() 
    {
        float swap = Input.GetAxis("Switch");
        
        if (swap != 0) {
            if (_swapped != swap) {
                int current = (int) animalTyps;
                int next = (current + (int) swap + 4) % animals.Length;
            
                animals[current].SetActive(false);
                animals[next].SetActive(true);

                animalTyps = (AnimalTyps) next;
                _swapped = swap;
            }
        } else {
            _swapped = 0;
        }
    }

    public bool IsGrounded() 
    {
        return _groundedTimer >= 0;
    }

    public void CamLookAtPlayer() 
    {
        Vector3 offset; 
        float distance;
        
        if (Input.GetButton("Fire2")) {
            offset = Vector3.up * (camSphereRaduis + zoomYOffset);
            distance = zoomCamDistance;
        } else {
            offset= Vector3.up * camSphereRaduis; 
            distance = camDistance;
        }
        
        Vector3 viewOffset = -1 * (_camTransform.rotation * Vector3.forward);
        
        RaycastHit hit;
        bool camCastHit = Physics.SphereCast(_transform.position + offset, camSphereRaduis, viewOffset, out hit, distance);

        if (camCastHit) {
            _camTransform.position = _transform.position + offset + (hit.distance - camSphereRaduis) * viewOffset;
        } else {
            _camTransform.position = _transform.position + offset + distance * viewOffset;
        }
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (collision.GetContact(0).normal.y > 0.9) {
            _groundCollisionID = collision.gameObject.GetInstanceID();
            _grounded = true;            
            _groundedTimer = maxGrounded;
        }
    }
    
    void OnCollisionExit(Collision collision) 
    {
        if (collision.gameObject.GetInstanceID() == _groundCollisionID) {
            _grounded = false;
        }
    }

    internal void SetNotGrounded()
    {
        _groundedTimer = -1;
    }

    public Rigidbody GetRigidbody() {
        return _rigidbody;
    }
    
    public Transform GetTransform() {
        return _transform;
    }
}
