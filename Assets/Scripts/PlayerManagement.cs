using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


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
    [Range(0f, 20f)]
    public float movementForce = 10;
    
    [SerializeField]
    [Range(0f, 1f)]
    public float airMovementFactor = 0.5f;

    [SerializeField]
    [Range(100f, 300f)]
    public float jumpForce = 200;
    
    private Transform _camPos;
    
    public Rigidbody rb;

    [SerializeField]
    [Range(0, 20)]
    private int MAX_GROUNDED = 10;

    private int groundCollision;
    private bool grounded;
    private int groundedTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        
        for (int i = 0; i < animals.Length; i++)
        {
            animals[i].SetActive((int)animalTyps == i);
            animalMove[i].SetPlayerManagement(this);
        }

        _camPos = cam.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!grounded) {
            groundedTimer = Mathf.Max(groundedTimer - 1, -1);
        }
        // print(groundedTimer);

        if (Input.GetButtonDown("Jump") && IsGrounded()) {
            animalMove[(int) animalTyps].Jump();
            groundedTimer = -1;
        }
    
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        bool specialActive = Input.GetButtonDown("Special");
        
        float camYr = _camPos.rotation.eulerAngles.y;

        Vector3 inputVec3 = new Vector3(inputH, 0, inputV);
        inputVec3.Normalize();
        inputVec3 = Quaternion.Euler(0, camYr, 0) * inputVec3;
        
        Vector2 inputVec2 = new Vector2(inputVec3.x,inputVec3.z);
        
        Transform t = GetComponentInParent<Transform>();
        if (inputVec3.magnitude > 0.001) {
            Quaternion targetRotation = Quaternion.LookRotation(inputVec3, Vector3.up);
            t.transform.rotation = Quaternion.Lerp(t.transform.rotation, targetRotation, 0.2f);
        }
        animalMove[(int) animalTyps].Move(inputVec2,specialActive);
    }

    public bool IsGrounded() {
        return groundedTimer >= 0;
    }

    void OnCollisionEnter(Collision collision) {
        print(collision.GetContact(0).normal.y);
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
