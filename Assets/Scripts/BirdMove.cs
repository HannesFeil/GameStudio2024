using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMove : AnimalMove
{
    private PlayerManagement playerManagement;
    private bool grounded;
    private int groundedTimer;

    private float swapped;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (!grounded) {
            groundedTimer = Mathf.Max(groundedTimer - 1, -1);
        }

        if (Input.GetButton("Jump") && playerManagement.IsGrounded()) {
            playerManagement.animalMove[(int) playerManagement.animalTyps].Jump();
            groundedTimer = -1;
        }
    
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        float swap = Input.GetAxis("Switch");
        bool specialActive = Input.GetButton("Special");

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
        
    }
}
