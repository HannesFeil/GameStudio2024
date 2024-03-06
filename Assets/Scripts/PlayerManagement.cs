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
    [Range(0, 10)]
    private float camDistance;

    [SerializeField] [Range(0, 1)] private float camSphereRaduis;

    [SerializeField]
    [Range(1, 5)]
    private float mouseSensitivityX;
    
    [SerializeField]
    [Range(1, 5)]
    private float mouseSensitivityY;

    [SerializeField]
    [Range(0f, 20f)]
    public float movementForce = 10;
    
    [SerializeField]
    [Range(0f, 1f)]
    public float airMovementFactor = 0.5f;

    [SerializeField]
    [Range(100f, 300f)]
    public float jumpForce = 200;
    
    private Transform _camTransform;
    
    public Rigidbody rb;
    public Transform transform;

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
        rb = GetComponentInParent<Rigidbody>();
        transform = GetComponentInParent<Transform>().transform;
        
        for (int i = 0; i < animals.Length; i++)
        {
            animals[i].SetActive((int)animalTyps == i);
            animalMove[i].SetPlayerManagement(this);
        }

        _camTransform = cam.GetComponent<Transform>().transform;
    }

    // Update is called once per frame
    void FixedUpdate() // Update_________________________________________________________
    {
        if (!grounded) {
            groundedTimer = Mathf.Max(groundedTimer - 1, -1);
        }

        if (Input.GetButton("Jump") && IsGrounded()) {
            animalMove[(int) animalTyps].Jump();
            groundedTimer = -1;
        }
    
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        float swap = Input.GetAxis("Switch");
        bool specialActive = Input.GetButtonDown("Special");

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

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -1*Input.GetAxis("Mouse Y");
        
        float camXr = _camTransform.rotation.eulerAngles.x + mouseY * mouseSensitivityY;
        float camYr = _camTransform.rotation.eulerAngles.y + mouseX * mouseSensitivityX;

        _camTransform.rotation = Quaternion.Euler(camXr, camYr, 0);                
        
        Vector3 viewOffset = -1 * (_camTransform.rotation * Vector3.forward);
        
        RaycastHit hit;
        bool camCastHit = Physics.SphereCast(transform.position, camSphereRaduis, viewOffset, out hit, camDistance);

        if (camCastHit) {
            _camTransform.position = hit.point; //Maybe nicht point sondern die Mitte des gehitteten Körpers?
        } else {
            _camTransform.position = transform.position + camDistance * viewOffset;
        }
        

        Vector3 inputVec3 = new Vector3(inputH, 0, inputV);
        inputVec3.Normalize();
        inputVec3 = Quaternion.Euler(0, camYr, 0) * inputVec3;
        
        Vector2 inputVec2 = new Vector2(inputVec3.x,inputVec3.z);
        
        if (inputVec3.magnitude > 0.001) {
            Quaternion targetRotation = Quaternion.LookRotation(inputVec3, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.2f);
        }
        animalMove[(int) animalTyps].Move(inputVec2,specialActive);
    }

    public bool IsGrounded() {
        return groundedTimer >= 0;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.GetContact(0).normal.y > 0.9) {
            groundCollision = collision.gameObject.GetInstanceID();
            grounded = true;            
            groundedTimer = MAX_GROUNDED;
        }
    }
    
    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.GetInstanceID() == groundCollision) {
            grounded = false;
        }
    }
}
