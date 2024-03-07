using System;
using UnityEngine;

public class PlayerManagement : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] animals;
    
    [SerializeField]
    private AnimalMove[] animalMove;
    
    [SerializeField] 
    private AnimalTyps animalTyps;

    [SerializeField]
    private GameObject cam;
    
    [SerializeField]
    [Range(0, 90)]
    public float CamVerticalClampAngle;
    
    [SerializeField]
    [Range(0, 10)]
    public float CamDistance;

    [SerializeField] 
    [Range(0, 1)] 
    public float CamSphereRaduis;

    [SerializeField]
    [Range(1, 5)]
    private float mouseSensitivityX;
    
    [SerializeField]
    [Range(1, 5)]
    private float mouseSensitivityY;

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
    private Transform orientation;
    
    private Transform _camTransform;
    
    public Rigidbody Rigidbody;
    public Transform Transform;

    [SerializeField]
    [Range(0, 20)]
    private int MAX_GROUNDED = 10;

    private int groundCollision;
    private bool grounded;
    private int groundedTimer;

    private float swapped;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Rigidbody = GetComponentInParent<Rigidbody>();
        Transform = GetComponentInParent<Transform>().transform;
        
        for (int i = 0; i < animals.Length; i++)
        {
            animals[i].SetActive((int)animalTyps == i);
            animalMove[i].SetPlayerManagement(this);
        }

        _camTransform = cam.GetComponent<Transform>().transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!grounded) {
            groundedTimer = Mathf.Max(groundedTimer - 1, -1);
        }
    
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -1*Input.GetAxis("Mouse Y");
        bool specialActive = Input.GetButton("Special");
        
        float camXr = _camTransform.rotation.eulerAngles.x + mouseY * mouseSensitivityY;
        float camYr = _camTransform.rotation.eulerAngles.y + mouseX * mouseSensitivityX;

        camXr = Mathf.Clamp(
            (camXr + 90) % 360, 
            90 - CamVerticalClampAngle, 
            90 + CamVerticalClampAngle
        ) - 90;

        _camTransform.rotation = Quaternion.Euler(camXr, camYr, 0);                

        Vector3 inputVec3 = new Vector3(inputH, 0, inputV);
        inputVec3.Normalize();
        inputVec3 = Quaternion.Euler(0, camYr, 0) * inputVec3;
        
        Vector2 inputVec2 = new Vector2(inputVec3.x,inputVec3.z);
        
        animalMove[(int) animalTyps].Move(inputVec2,specialActive);
    }

    public void checkSwap() 
    {
        float swap = Input.GetAxis("Switch");
        
        if (swap != 0) {
            if (swapped != swap) {
                int current = (int) animalTyps;
                int next = (current + (int) swap + 4) % animals.Length;
            
                animals[current].SetActive(false);
                animals[next].SetActive(true);

                animalTyps = (AnimalTyps) next;
                swapped = swap;
            }
        } else {
            swapped = 0;
        }
    }

    public bool IsGrounded() 
    {
        return groundedTimer >= 0;
    }

    public void camLookAt(Vector3 offset, float distance) 
    {
        Vector3 viewOffset = -1 * (_camTransform.rotation * Vector3.forward);
        
        RaycastHit hit;
        bool camCastHit = Physics.SphereCast(Transform.position + offset, CamSphereRaduis, viewOffset, out hit, distance);

        if (camCastHit) {
            _camTransform.position = Transform.position + offset + (hit.distance - CamSphereRaduis) * viewOffset;
        } else {
            _camTransform.position = Transform.position + offset + distance * viewOffset;
        }
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (collision.GetContact(0).normal.y > 0.9) {
            groundCollision = collision.gameObject.GetInstanceID();
            grounded = true;            
            groundedTimer = MAX_GROUNDED;
        }
    }
    
    void OnCollisionExit(Collision collision) 
    {
        if (collision.gameObject.GetInstanceID() == groundCollision) {
            grounded = false;
        }
    }

    internal void SetNotGrounded()
    {
        groundedTimer = -1;
    }
}
