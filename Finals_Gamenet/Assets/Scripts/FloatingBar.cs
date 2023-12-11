using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingBar : MonoBehaviour
{
    private HealthComponent healthComponent;

    public TMPro.TMP_Text playerName_TXT;
    public Image hp_Bar;

    private void Start()
    {
        healthComponent = GetComponent<HealthComponent>();
    }

    private void FixedUpdate()
    {
        hp_Bar.fillAmount = healthComponent.currentHP / healthComponent.MaxHP;
    }
}
