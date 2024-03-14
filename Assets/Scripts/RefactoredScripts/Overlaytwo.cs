using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Overlaytwo : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameManagementRefactored gameManagement;
    [FormerlySerializedAs("Time")] [FormerlySerializedAs("speedometer")] [SerializeField]
    private TMP_Text time;
    [SerializeField]
    private TMP_Text moveState;
    [SerializeField]
    private Transform[] displayAnimals = new Transform[4];
    [SerializeField]
    private Transform[] positions = new Transform[3];
    [SerializeField]
    private Slider[] stamina = new Slider[3];
    [SerializeField]
    private Image[] staminaImage = new Image[3];
    [SerializeField] 
    private GameObject winningBanner;
    [SerializeField]
    private TMP_Text finalTime;
    [SerializeField] 
    private GameObject[] medalion = new GameObject[3];

    [SerializeField] private float[] medalionTimes = new float[3];

    //B0BF1A

    [Header("Stamina")]
    //private Color[] staminaColor = {new Color(0xFF,0x00,0x00,1), new Color(0xFF,0xF2,0x05,1), new Color(0x7C,0xFF,0x01,1)};
    private Color[] staminaColor = { new Color(0xFF, 0x00, 0x00, 1), new Color(0x7B, 0xFE, 0x00, 1), new Color(0x00, 0xFF, 0x00, 1) };
    [SerializeField]
    private float[] staminaThreshold = {0.25f,0.5f,1f};
    [SerializeField]
    private float interpellationSpeed = 2f;
    [SerializeField]
    private float interpellationSwitchSpeed = 10f;


    private bool _useSwitchSpeed;
    private float _centerStamnia = 1;
    private AnimalType _lastAnimal;

    // Start is called before the first frame update
    private void Start()
    {
        winningBanner.SetActive(false);
        _lastAnimal = (AnimalType) (((int) gameManagement.PlayerMovement.GetAnimalTyp() + 1) % 4);
        print(staminaColor[0]);
        print(staminaColor[1]);
        print(staminaColor[2]);
        AnimalsDisplay();
        
    }
    // Update is called once per frame
    void Update()
    {
        time.text = "Speed :" + gameManagement.PlayerMovement.getVelocity();
        moveState.text = "Time :" + Mathf.Round(gameManagement.GameTime * 100) / 100;
        AnimalsDisplay();
        StaminaDisplay();
    }

    private void AnimalsDisplay()
    {
        int currentAnimal = (int) gameManagement.PlayerMovement.GetAnimalTyp();
        if ((int) _lastAnimal == currentAnimal) return;

        _lastAnimal = (AnimalType) currentAnimal;
        int left = (currentAnimal + 3) % 4;
        int right = (currentAnimal + 1) % 4;
        int box = (currentAnimal + 2) % 4;

        displayAnimals[left].position = positions[0].position;
        displayAnimals[left].localScale = Vector3.one * 0.75f;
        displayAnimals[left].localRotation = Quaternion.Euler(0, 153, 0);
        displayAnimals[currentAnimal].position = positions[1].position;
        displayAnimals[currentAnimal].localScale = Vector3.one;
        displayAnimals[currentAnimal].localRotation = Quaternion.Euler(0, 153, 0);
        displayAnimals[right].position = positions[2].position;
        displayAnimals[right].localScale = Vector3.one * 0.75f;
        displayAnimals[right].localRotation = Quaternion.Euler(0,-153,0);

        displayAnimals[left].gameObject.SetActive(true);
        displayAnimals[currentAnimal].gameObject.SetActive(true);
        displayAnimals[right].gameObject.SetActive(true);
        displayAnimals[box].gameObject.SetActive(false);

        _useSwitchSpeed = true;

    }

    

    private void StaminaDisplay()
    {

        int currentAnimal = (int)gameManagement.PlayerMovement.GetAnimalTyp();
        int left = (currentAnimal + 3) % 4;
        int right = (currentAnimal + 1) % 4;

        float leftStamina = gameManagement.PlayerMovement.GetStamina(left) / 100;
        stamina[0].value = leftStamina;

        Color leftCol = staminaColor[floatToIndex(leftStamina)];
        staminaImage[0].color = leftCol;


        float rightStamina = gameManagement.PlayerMovement.GetStamina(right) / 100;
        stamina[2].value = rightStamina;
        Color rightCol = staminaColor[floatToIndex(leftStamina)];
        staminaImage[2].color = rightCol;

        float desiredStamina = gameManagement.PlayerMovement.GetStamina(currentAnimal) / 100;
        _centerStamnia = Mathf.Lerp(_centerStamnia, desiredStamina, 
            (_useSwitchSpeed ? interpellationSwitchSpeed : interpellationSpeed) * UnityEngine.Time.deltaTime);
        if(_useSwitchSpeed && Mathf.Abs(desiredStamina - _centerStamnia) < 0.01f)
        {
            _useSwitchSpeed = false;
        }

        staminaImage[1].color = staminaColor[floatToIndex(_centerStamnia)];
        stamina[1].value = _centerStamnia;
    }

    private int floatToIndex(float s)
    {
        for (int i = 0; i < staminaThreshold.Length; i++)
        {
            if (s <= staminaThreshold[i])
            {
                return i;
            }
        }
        return -1;
    }

    public void DisplayWinnigBanner()
    {
        winningBanner.SetActive(true);
        Time.timeScale = 0;
        finalTime.text = "" + Mathf.Round(gameManagement.GameTime * 100) / 100;
        for (int i = 0; i < medalion.Length; i++)
        {
            if (medalionTimes[i] > gameManagement.GameTime)
            {
                medalion[i].SetActive(true);
            }
            else
            {
                medalion[i].SetActive(false);
            }
        }
    }
}
