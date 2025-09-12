using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class PlayerUIHub 
{
    [SerializeField] Slider hpSlider, armorSlider;

    public void Init(float MaxHealth, float MaxArmor)
    {
        hpSlider.maxValue = MaxHealth;
        armorSlider.maxValue = MaxArmor;
        hpSlider.value = MaxHealth;
    }

    public void UpdateSliders(float health, float armor)
    {
        armorSlider.value = armor;
        hpSlider.value = health;
    }
}
