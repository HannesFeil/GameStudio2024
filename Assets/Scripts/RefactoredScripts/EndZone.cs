using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    [SerializeField] 
    private GameManagementRefactored gameManagement;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        gameManagement.StopTimer();
    }
}
