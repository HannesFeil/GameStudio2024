using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManagment : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] medalion = new GameObject[3];

    [SerializeField] 
    private float[] timeRequirerment = new float[3];

    [SerializeField] 
    private string worldName = "World00";

    [SerializeField] 
    private TMP_Text bestTime;
    
    // Start is called before the first frame update
    void Start()
    {
        float time = PlayerPrefs.GetFloat(worldName, -1);
        for (int i = 0; i < medalion.Length; i++)
        {
            medalion[i].SetActive(time > 0 && time < timeRequirerment[i]);
        }

        bestTime.text = time > 0 ? time.ToString() : "--.--";
    }
}
