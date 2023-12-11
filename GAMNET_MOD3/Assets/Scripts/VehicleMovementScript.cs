using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovementScript : MonoBehaviour
{
    public float speed = 20;
    public float rotationSpeed = 200;
    public float currentSpeed = 0;

    public bool isControlEnable;

    private void Start()
    {
        isControlEnable = false;
    }

    private void LateUpdate()
    {
        if(!isControlEnable) return;
        
        float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        
        transform.Translate(0,0,translation);
        currentSpeed = translation;
        
        transform.Rotate(0,rotation,0);
    }
}
