using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutWeaponSelectionItem : MonoBehaviour
{
    public LoadoutSelectionScript loadoutSelection;
    public WeaponData weaponData;
    [SerializeField] Image weaponIcon;
    [SerializeField] Text weaponName;
    [SerializeField] Slider damageBar;
    [SerializeField] Slider ammoBar;
    [SerializeField] Slider fireRateBar;
    public int weaponIndex;
    // Start is called before the first frame update
    private void Awake()
    {
        loadoutSelection = GetComponentInParent<LoadoutSelectionScript>();
    }

    void Start()
    {
        damageBar.maxValue = 100f;
        damageBar.minValue = 0f;
        ammoBar.maxValue = 60;
        ammoBar.minValue = 0;
        fireRateBar.maxValue = 20f;
        fireRateBar.minValue = 0f;

        if (weaponData.weaponType == QuantityStatsHUD.WeaponType.Shotgun) SetMultipleDamageValue(weaponData.damagePerPellet, weaponData.pelletsPerFire);
        else SetDamageValue(weaponData.damage);
        SetAmmoValue(weaponData.maxAmmoPerMag);
        SetFireRateValue(weaponData.fireRate);
        SetWeaponName(weaponData.itemName);
        SetWeaponIcon(weaponData.itemIcon);
    }

    public void OnClickButton()
    {
        for(int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if(GlobalDatabase.singleton.allWeaponDatas[i] == weaponData)
            {
                loadoutSelection.loadoutPreviewUI.SetWeaponSlotInfo(loadoutSelection.forSelectedSlot, weaponData);
            }
        }
        loadoutSelection.EnablePreview();
        loadoutSelection.DisableWeaponSelection();
        loadoutSelection.OpenLoadoutButtonsVisual();
        loadoutSelection.SetLoadoutDataToPreferences();
        Launcher.Instance.SetLoadoutValuesToPlayer();
    }

    public void SetDamageValue(float amount)
    {
        damageBar.value = amount;
    }
    public void SetMultipleDamageValue(float amount, int slugs)
    {
        damageBar.value = amount * slugs;
    }
    public void SetAmmoValue(int amount)
    {
        ammoBar.value = amount;
    }
    public void SetFireRateValue(float amount)
    {
        fireRateBar.value = amount;
    }
    public void SetWeaponIcon(Sprite iconSprite)
    {
        weaponIcon.sprite = iconSprite;
    }
    public void SetWeaponName(string name)
    {
        weaponName.text = name;
    }
}
