using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingComponent : MonoBehaviour
{
    [SerializeField] private LayerMask layer;
    public bool isLanded { get; private set; }
    private Action<bool> OnLanded;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if ((layer.value & (1 << col.transform.gameObject.layer)) > 0) 
        {
            isLanded = true;
            OnLanded?.Invoke(isLanded);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((layer.value & (1 << other.transform.gameObject.layer)) > 0) 
        {
            isLanded = false;
            OnLanded?.Invoke(isLanded);
        }
    }
}
