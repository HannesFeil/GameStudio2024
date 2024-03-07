using System;
using UnityEngine;

public class PlayerManagement : MonoBehaviour
{
    [Header("Animals")]
    [SerializeField] 
    private GameObject[] animals; //die 4 Tiere
    
    [SerializeField]
    private AnimalMove[] animalMove; //Bewegungsskripte der Tiere
    
    [SerializeField] 
    private AnimalTyps animalTyps = AnimalTyps.SNAKE; //der Typ des Tieres, aus dem enum 

    [Header("Camera")]
    [SerializeField]
    private GameObject cam; //das Kameraobjekt
    
    [SerializeField]
    [Range(0, 90)]
    public float CamVerticalClampAngle = 70; //Der Kamerawinkel, wie weit man von oben/unten auf den Player schauen kann
    
    [SerializeField]
    [Range(0, 10)]
    public float CamDistance = 5; //Die Entfernung zwischen Spieler und Kamera

    [SerializeField]
    [Range(0, 10)]
    private float zoomCamDistance = 2; //Die Entfernung die heran/weg gezoomt wird

    [SerializeField]
    [Range(0, 1.5f)]
    private float zoomYOffset = 0.5f; //Höhenänderung der Kamera beim Zoomen

    [SerializeField] 
    [Range(0, 1)] 
    public float CamSphereRaduis = 0.3f; //Der Raduis der Sphere um die Kamera herum

    [Header("Mouse")]
    [SerializeField]
    [Range(1, 5)]
    private float mouseSensitivityX = 6; //Bewegungssensibility in X Richtung
    
    [SerializeField]
    [Range(1, 5)]
    private float mouseSensitivityY = 2.6; //Bewegungssensibility in Y Richtung

    [Header("Forces")]
    [SerializeField]
    [Range(0f, 20f)]
    public float MovementForce = 10; //Kraft die beim Laufen auf den Spieler wirkt
    
    [SerializeField]
    [Range(0f, 1f)]
    public float AirMovementFactor = 0.5f; //Multiplikator für "In der Luft" Zustand

    [SerializeField]
    [Range(100f, 300f)]
    public float JumpForce = 200; //Kraft die beim Springen auf den Spieler wirkt

    [Header("Bodenvariablen")]
    [SerializeField]
    [Range(0, 20)]
    private int maxGrounded = 10; //Max Wert des groundTimers

    private int groundCollisionID; //ID welches Objekt berührt wird
    private bool grounded; //Ist true, wenn der Boden berührt wird
    private int groundedTimer; //Ist MAX_GROUNDED, wenn der Boden berührt wird

    private float swapped; //speichert, ob das Erscheinungsbild des Tieres gerade gewechstelt wird/wurde
    //maybe besser ein bool
    
    public Rigidbody Rigidbody; //Der Collider des Spielers???
    public Transform Transform; //Position, Rotation und Skalierung des Spielers
    private Transform _camTransform; //Position und Rotation und Skalierung der Kamera
    
    // Start is called before the first frame update
    void Start()
    {
        //Mause wird unsichtbar und zentriert im Bildschirm
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        //holt sich die statischen Variablen
        Rigidbody = GetComponentInParent<Rigidbody>();
        Transform = GetComponentInParent<Transform>().transform;
        _camTransform = cam.GetComponent<Transform>().transform;
        
        //setzt alle Tiere an die gleiche Stelle und nur ein Tier wird sichtbar
        for (int i = 0; i < animals.Length; i++) 
        {
            animals[i].SetActive((int)animalTyps == i);
            animalMove[i].SetPlayerManagement(this);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //zählt den groundTimer runter, wenn der Spieler sich in der Luft befindet
        if (!grounded) {
            groundedTimer = Mathf.Max(groundedTimer - 1, -1);
        }
        
        //holt sich alle Benutzereingaben (Tasten und Mausebewegung)
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -1*Input.GetAxis("Mouse Y");
        bool specialActive = Input.GetButton("Fire1");
        
        //berechnet die Kamerarotation um X und Y Achse
        float camXr = _camTransform.rotation.eulerAngles.x + mouseY * mouseSensitivityY;
        float camYr = _camTransform.rotation.eulerAngles.y + mouseX * mouseSensitivityX;

        //die Begrenzung der Kamera, damit diese nicht im Boden verschwindet (Drehung um X-Achse <=> Kamerabewegung hoch/runter)
        camXr = Mathf.Clamp(
            (camXr + 90) % 360, 
            90 - CamVerticalClampAngle, 
            90 + CamVerticalClampAngle
        ) - 90;

        //??? Aaron/Hannes fragen
        _camTransform.rotation = Quaternion.Euler(camXr, camYr, 0);                
        
        //??? Aaron/Hannes fragen
        Vector3 inputVec3 = new Vector3(inputH, 0, inputV);
        inputVec3.Normalize();
        inputVec3 = Quaternion.Euler(0, camYr, 0) * inputVec3;
        
        //inputs für die Move Methode
        Vector2 inputVec2 = new Vector2(inputVec3.x,inputVec3.z);
        
        //ruft die Bewegungsmethoden der Tiere (extern) auf
        animalMove[(int) animalTyps].Move(inputVec2,specialActive);
    }

    ///<summary>
    /// Methode mit der man durch die verschiedenen Tiere wechslen kann
    /// </summary>
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

    ///<summary>
    /// Getter der wieder gibt, ob der Spieler auf dem Boden ist
    /// </summary>
    public bool IsGrounded() 
    {
        return _groundedTimer >= 0;
    }

    ///<summary>
    /// Methode bewegt die Kamera und korrigiert ihre Position mit einem SphereCast
    /// </summary>
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

    ///<summary>
    /// Wird aufgerufen, wenn der Rigidbody/Collider etwas zu berühren beginnt
    /// Aktualisiert die Bodenvariablen des Spielers für den Zustand "Auf dem Boden"
    /// 
    /// </summary>
    void OnCollisionEnter(Collision collision) 
    {
        if (collision.GetContact(0).normal.y > 0.9) {
            _groundCollisionID = collision.gameObject.GetInstanceID();
            _grounded = true;            
            _groundedTimer = maxGrounded;
        }
    }
    
    ///<summary>
    /// Wird aufgerufen, wenn der Rigidbody/Collider aufhört etwas zu berühren
    /// Aktualisiert die Bodenvariablen des Spielers für den Zustand "In der Luft"
    /// </summary>
    void OnCollisionExit(Collision collision) 
    {
        if (collision.gameObject.GetInstanceID() == _groundCollisionID) {
            _grounded = false;
        }
    }

    ///<summary>
    /// Zählt den <c>groundenTimer</c> runter
    /// </summary>
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
