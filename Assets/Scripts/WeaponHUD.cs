using UnityEngine;
using TMPro;

public class WeaponHUD : MonoBehaviour
{
    [SerializeField] private WeaponController weapon;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadText;

    void Update()
    {
        if (weapon != null)
        {
            UpdateAmmoDisplay();
        }
    }

    void UpdateAmmoDisplay()
    {
        if (ammoText != null)
        {
            ammoText.text = weapon.GetCurrentAmmo() + " / " + weapon.GetMaxAmmo();
        }

        if (reloadText != null)
        {
            reloadText.gameObject.SetActive(weapon.IsReloading());
        }
    }

    public void SetWeapon(WeaponController newWeapon)
    {
        weapon = newWeapon;
    }
}
