using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Overlaytwo : MonoBehaviour
{
    [SerializeField]
    private TMP_Text speedometer;
    [SerializeField]
    private TMP_Text moveState;
    [SerializeField]
    private PlayerMovement pm;

    private AnimalType _lastAnimal;

    [SerializeField]
    private Transform[] displayAnimals = new Transform[4];
    [SerializeField]
    private Transform[] positions = new Transform[3];
    [SerializeField]
    private Slider[] stamina = new Slider[3];
    [SerializeField]
    private Image[] staminaImage = new Image[3];
    [SerializeField]
    private Color[] staminaColor = {new Color(0xFF,0x00,0x00), new Color(0xFF,0xF2,0x05), new Color(0x7C,0xFF,0x01)};
    [SerializeField]
    private float[] staminaThreshold = {0.25f,0.5f,1f};

    private float _centerStamnia = 1;

    // Start is called before the first frame update
    private void Start()
    {
        _lastAnimal = (AnimalType) (((int) pm.GetAnimalTyp() + 1) % 4);
        AnimalsDisplay();
    }
    // Update is called once per frame
    void Update()
    {
        speedometer.text = "Speed :" + pm.getVelocity();
        moveState.text = pm.getMoveState().ToString();
        AnimalsDisplay();
        StaminaDisplay();
    }

    private void AnimalsDisplay()
    {
        int currentAnimal = (int) pm.GetAnimalTyp();
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

        _centerStamnia = Mathf.Lerp(_centerStamnia, pm.GetStamina(currentAnimal) / 100, 10 * Time.deltaTime);

    }

    

    private void StaminaDisplay()
    {
        int currentAnimal = (int)pm.GetAnimalTyp();
        int left = (currentAnimal + 3) % 4;
        int right = (currentAnimal + 1) % 4;

        float leftStamina = pm.GetStamina(left) / 100;
        stamina[0].value = leftStamina;
        staminaImage[0].color = floatToColor(leftStamina);


        float rightStamina = pm.GetStamina(right) / 100;
        stamina[2].value = rightStamina;
        staminaImage[2].color = floatToColor(rightStamina);

        _centerStamnia = Mathf.Lerp(_centerStamnia, pm.GetStamina(currentAnimal) / 100,2 * Time.deltaTime);
        staminaImage[1].color = floatToColor(_centerStamnia);
        stamina[1].value = _centerStamnia;
    }

    private Color floatToColor(float s)
    {
        for (int i = 0; i < staminaThreshold.Length; i++)
        {
            if (s <= staminaThreshold[i])
            {
                return staminaColor[i];
            }
        }
        return new Color();
    }
}
