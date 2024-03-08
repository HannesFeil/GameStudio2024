using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    [SerializeField]
    private Sprite[] animalIcons;

    private Image _previousAnimalPicture;
    private Image _currentAnimalPicture;
    private Image _nextAnimalPicture;

    // Start is called before the first frame update
    void Start()
    {
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        _previousAnimalPicture = images[0];
        _currentAnimalPicture = images[1];
        _nextAnimalPicture = images[2];
    }

    public void UpdateAnimalDisplay(AnimalType animal)
    {
        int index = (int) animal;
        _previousAnimalPicture.sprite = animalIcons[(index + 3) % 4];
        _currentAnimalPicture.sprite = animalIcons[index];
        _nextAnimalPicture.sprite = animalIcons[(index + 1) % 4];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
