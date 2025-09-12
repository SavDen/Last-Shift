using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponControllerView 
{
    [SerializeField] private Image weapon;
    [SerializeField] private TextMeshProUGUI countAmmo;
    [SerializeField] private Image throwable;
    [SerializeField] private TextMeshProUGUI countThrowable;

    public void UpdateWeaponView(Sprite weapon, Sprite throwable, float ammo, float ammoCapacity, int ammoThrowable)
    {
        this.weapon.sprite = weapon;
        this.throwable.sprite = throwable;
        countAmmo.text = $"{ammo:F0}/{ammoCapacity}";
        countThrowable.text = $"{ammoThrowable}";
    }

    public void UpdateAmmoWeapon(float ammo, float ammoCapacity)
    {
        countAmmo.text = $"{ammo:F0}/{ammoCapacity}";
    }

    public void UpodateAmmoThrowable(Sprite throwableIcon, int ammoThrowable)
    {
        throwable.sprite = throwableIcon;
        countThrowable.text = $"{ammoThrowable}";
    }
}
