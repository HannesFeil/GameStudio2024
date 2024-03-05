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

    private Transform _camPos;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < animals.Length; i++)
        {
            animals[i].SetActive((int)animalTyps == i);
            animalMove[i].SetRb(GetComponentInParent<Rigidbody>());
        }

        _camPos = cam.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float inputH = Input.GetAxis("Horizontal");
        print(inputH);
        float inputV = Input.GetAxis("Vertical");
        print(inputV);
        bool specialActive = Input.GetButtonDown("Special");
        print(specialActive);
        
        float camYr = _camPos.rotation.y;
        print(camYr);

        Vector3 inputVec3 = Quaternion.Euler(0, camYr, 0) * new Vector3(inputH, 0, inputV);
        inputVec3.Normalize();
        Vector2 inputVec2 = new Vector2(inputVec3.x,inputVec3.z);
        print(inputVec2);
        animalMove[(int) animalTyps].Move(inputVec2,specialActive);
    }
}
