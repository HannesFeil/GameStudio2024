using TMPro;
using UnityEngine;

public class Overlaytwo : MonoBehaviour
{
    [SerializeField]
    private TMP_Text speedometer;
    [SerializeField]
    private TMP_Text moveState;
    [SerializeField]
    private PlayerMovement pm;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        speedometer.text = "Speed :" + pm.getVelocity();
        moveState.text = pm.getMoveStateString();
    }
}
