using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelBarUI : GenericBarUI
{
    void Awake()
    {
        UIManager.SetFuelBarUI(this);
        gameObject.SetActive(false);
    }
}
