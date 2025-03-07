using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelectionScript : MonoBehaviour
{
    public GameObject loadoutSelectionItemPrefab;
    public GameObject loadoutWeaponSelectionItemPrefab;
    [Space]
    public GameObject loadoutButtonsUI;
    public GameObject loadoutWeaponSelectionUI;
    public GameObject loadoutAttachmentSelections;

    [Space]
    public Transform loadoutButtonsHolder;
    public Transform loadoutWeaponSelectsHolder;
    public LoadoutPreviewUI loadoutPreviewUI;
    public LoadoutCustomButtonsHolder customButtonsHolder;
    public LoadoutCustomization loadoutCustomization;
    public List<LoadoutSelectionItem> loadoutItems = new List<LoadoutSelectionItem>();
    public List<LoadoutWeaponSelectionItem> loadoutWeaponSelects = new List<LoadoutWeaponSelectionItem>();
    [HideInInspector] public int forSelectedSlot = 0;

    [Space]
    public int selectedLoadoutIndex;
    public int selectedMainWeaponIndex;
    public int selectedSecondWeaponIndex;
    public int selectedEquipmentIndex1;
    public int selectedEquipmentIndex2;

    [Space]
    [Header("More References")]
    public List<LoadoutData> loadoutDataList = new List<LoadoutData>();
    // Start is called before the first frame update
    private void Awake()
    {
        LoadoutSelectionItem[] tempItems = loadoutPreviewUI.GetComponentsInChildren<LoadoutSelectionItem>();
        for (int i = 0; i < tempItems.Length; i++)
        {
            loadoutItems.Add(tempItems[i]);
        }

        //MainMenuUIManager.instance.CloseLoadoutSelectionMenu();
    }
    void Start()
    {
        InstantiateLoadoutSelections();
        SetLoadoutDataFromPrefs();
        InstantiateLoadoutWeaponSelections();
        OpenLoadoutButtonsVisual();
        loadoutPreviewUI.QuitCustomizationUI();
        DisablePreview();
        loadoutItems[selectedLoadoutIndex].ToggleSelectVisual(true);
        DisableWeaponSelection();
    }
    public void SetLoadoutDataToPreferences()
    {
        int selectedMainWeaponIndex = Launcher.Instance.FindGlobalWeaponIndex(loadoutDataList[selectedLoadoutIndex].weaponData[0]);
        int selectedSecondWeaponIndex = Launcher.Instance.FindGlobalWeaponIndex(loadoutDataList[selectedLoadoutIndex].weaponData[1]);
        int SMWA_SightIndex1 = loadoutDataList[selectedLoadoutIndex].selectedSightIndex[0];
        int SMWA_SightIndex2 = loadoutDataList[selectedLoadoutIndex].selectedSightIndex[1];
        int SMWA_BarrelIndex1 = loadoutDataList[selectedLoadoutIndex].selectedBarrelIndex[0];
        int SMWA_BarrelIndex2 = loadoutDataList[selectedLoadoutIndex].selectedBarrelIndex[1];
        int SMWA_UnderbarrelIndex1 = loadoutDataList[selectedLoadoutIndex].selectedUnderbarrelIndex[0];
        int SMWA_UnderbarrelIndex2 = loadoutDataList[selectedLoadoutIndex].selectedUnderbarrelIndex[1];
        int SMWA_LeftbarrelIndex1 = loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeftIndex[0];
        int SMWA_LeftbarrelIndex2 = loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeftIndex[1];
        int SMWA_RightbarrelIndex1 = loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRightIndex[0];
        int SMWA_RightbarrelIndex2 = loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRightIndex[1];
        int SMWA_AppearanceIndex1 = loadoutDataList[selectedLoadoutIndex].selectedAppearanceDataIndex[0];
        int SMWA_AppearanceIndex2 = loadoutDataList[selectedLoadoutIndex].selectedAppearanceDataIndex[1];
        PlayerPrefs.SetInt("selectedLoadoutIndex", selectedLoadoutIndex);
        PlayerPrefs.SetInt("selectedMainWeaponIndex", selectedMainWeaponIndex);
        PlayerPrefs.SetInt("selectedSecondWeaponIndex", selectedSecondWeaponIndex);
        PlayerPrefs.SetInt("SMWA_SightIndex1", SMWA_SightIndex1);
        PlayerPrefs.SetInt("SMWA_SightIndex2", SMWA_SightIndex2);
        PlayerPrefs.SetInt("SMWA_BarrelIndex1", SMWA_BarrelIndex1);
        PlayerPrefs.SetInt("SMWA_BarrelIndex2", SMWA_BarrelIndex2);
        PlayerPrefs.SetInt("SMWA_UnderbarrelIndex1", SMWA_UnderbarrelIndex1);
        PlayerPrefs.SetInt("SMWA_UnderbarrelIndex2", SMWA_UnderbarrelIndex2);
        PlayerPrefs.SetInt("SMWA_LeftbarrelIndex1", SMWA_LeftbarrelIndex1);
        PlayerPrefs.SetInt("SMWA_LeftbarrelIndex2", SMWA_LeftbarrelIndex2);
        PlayerPrefs.SetInt("SMWA_RightbarrelIndex1", SMWA_RightbarrelIndex1);
        PlayerPrefs.SetInt("SMWA_RightbarrelIndex2", SMWA_RightbarrelIndex2);
        PlayerPrefs.SetInt("SMWA_AppearanceIndex1", SMWA_AppearanceIndex1);
        PlayerPrefs.SetInt("SMWA_AppearanceIndex2", SMWA_AppearanceIndex2);
    }
    public int GetLoadoutDataFromPreferences(string key)
    {
        int returner = 0;
        switch (key)
        {
            case "selectedLoadoutIndex":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "selectedMainWeaponIndex":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "selectedSecondWeaponIndex":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_SightIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_SightIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_BarrelIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_BarrelIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_UnderbarrelIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_UnderbarrelIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_LeftbarrelIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_LeftbarrelIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_RightbarrelIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_RightbarrelIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_AppearanceIndex1":
                returner = PlayerPrefs.GetInt(key);
                break;
            case "SMWA_AppearanceIndex2":
                returner = PlayerPrefs.GetInt(key);
                break;
        }
        return returner;
    }

    public WeaponData FindWeaponDataFromIndex(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.singleton.allWeaponDatas[i];
        }
        return null;
    }
    public WeaponAttachmentData FindAttachmentDataFromIndex(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponAttachmentDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.singleton.allWeaponAttachmentDatas[i];
        }
        return null;
    }
    public void SetLoadoutDataFromPrefs()
    {
        if (PlayerPrefs.HasKey("selectedLoadoutIndex")) selectedLoadoutIndex = GetLoadoutDataFromPreferences("selectedLoadoutIndex");
        if (PlayerPrefs.HasKey("selectedMainWeaponIndex")) selectedMainWeaponIndex = GetLoadoutDataFromPreferences("selectedMainWeaponIndex");
        if (PlayerPrefs.HasKey("selectedSecondWeaponIndex")) selectedSecondWeaponIndex = GetLoadoutDataFromPreferences("selectedSecondWeaponIndex");
        if (PlayerPrefs.HasKey("SMWA_SightIndex1")) loadoutDataList[selectedLoadoutIndex].selectedSightIndex[0] = GetLoadoutDataFromPreferences("SMWA_SightIndex1");
        if (PlayerPrefs.HasKey("SMWA_SightIndex2")) loadoutDataList[selectedLoadoutIndex].selectedSightIndex[1] = GetLoadoutDataFromPreferences("SMWA_SightIndex2");
        if (PlayerPrefs.HasKey("SMWA_BarrelIndex1")) loadoutDataList[selectedLoadoutIndex].selectedBarrelIndex[0] = GetLoadoutDataFromPreferences("SMWA_BarrelIndex1");
        if (PlayerPrefs.HasKey("SMWA_BarrelIndex2")) loadoutDataList[selectedLoadoutIndex].selectedBarrelIndex[1] = GetLoadoutDataFromPreferences("SMWA_BarrelIndex2");
        if (PlayerPrefs.HasKey("SMWA_UnderbarrelIndex1")) loadoutDataList[selectedLoadoutIndex].selectedUnderbarrelIndex[0] = GetLoadoutDataFromPreferences("SMWA_UnderbarrelIndex1");
        if (PlayerPrefs.HasKey("SMWA_UnderbarrelIndex2")) loadoutDataList[selectedLoadoutIndex].selectedUnderbarrelIndex[1] = GetLoadoutDataFromPreferences("SMWA_UnderbarrelIndex2");
        if (PlayerPrefs.HasKey("SMWA_LeftbarrelIndex1")) loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeftIndex[0] = GetLoadoutDataFromPreferences("SMWA_LeftbarrelIndex1");
        if (PlayerPrefs.HasKey("SMWA_LeftbarrelIndex2")) loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeftIndex[1] = GetLoadoutDataFromPreferences("SMWA_LeftbarrelIndex2");
        if (PlayerPrefs.HasKey("SMWA_RightbarrelIndex1")) loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRightIndex[0] = GetLoadoutDataFromPreferences("SMWA_RightbarrelIndex1");
        if (PlayerPrefs.HasKey("SMWA_RightbarrelIndex2")) loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRightIndex[1] = GetLoadoutDataFromPreferences("SMWA_RightbarrelIndex2");
        if (PlayerPrefs.HasKey("SMWA_AppearanceIndex1")) loadoutDataList[selectedLoadoutIndex].selectedAppearanceDataIndex[0] = GetLoadoutDataFromPreferences("SMWA_AppearanceIndex1");
        if (PlayerPrefs.HasKey("SMWA_AppearanceIndex2")) loadoutDataList[selectedLoadoutIndex].selectedAppearanceDataIndex[1] = GetLoadoutDataFromPreferences("SMWA_AppearanceIndex2");

        loadoutItems[selectedLoadoutIndex].SelectLoadout();
        loadoutDataList[selectedLoadoutIndex].weaponData[0] = FindWeaponDataFromIndex(selectedMainWeaponIndex);
        loadoutDataList[selectedLoadoutIndex].weaponData[1] = FindWeaponDataFromIndex(selectedSecondWeaponIndex);
        loadoutDataList[selectedLoadoutIndex].selectedSight[0] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSightIndex[0]);
        loadoutDataList[selectedLoadoutIndex].selectedSight[1] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSightIndex[1]);
        loadoutDataList[selectedLoadoutIndex].selectedBarrel[0] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedBarrelIndex[0]);
        loadoutDataList[selectedLoadoutIndex].selectedBarrel[1] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedBarrelIndex[1]);
        loadoutDataList[selectedLoadoutIndex].selectedUnderbarrel[0] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedUnderbarrelIndex[0]);
        loadoutDataList[selectedLoadoutIndex].selectedUnderbarrel[1] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedUnderbarrelIndex[1]);
        loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeft[0] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeftIndex[0]);
        loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeft[1] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSidebarrelLeftIndex[1]);
        loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRight[0] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRightIndex[0]);
        loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRight[1] = FindAttachmentDataFromIndex(loadoutDataList[selectedLoadoutIndex].selectedSidebarrelRightIndex[1]);
    }

    public void InstantiateLoadoutSelections()
    {
        for (int i = 0; i < loadoutDataList.Count; i++)
        {
            LoadoutSelectionItem temp = Instantiate(loadoutSelectionItemPrefab, loadoutButtonsHolder).GetComponent<LoadoutSelectionItem>();
            temp.itemLoadoutData = loadoutDataList[i];
            temp.DeselectLoadout();
            temp.loadoutIndex = i;
            loadoutDataList[i].loadoutIndex = i;
            temp.SetLoadoutName(loadoutDataList[i].loadoutName);
            loadoutItems.Add(temp);
            if (loadoutDataList[i].isDefault)
            {
                temp.SelectLoadout();
                temp.ToggleSelectVisual(true);
            }
        }
    }
    public void InstantiateLoadoutWeaponSelections()
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            LoadoutWeaponSelectionItem temp = Instantiate(loadoutWeaponSelectionItemPrefab, loadoutWeaponSelectsHolder).GetComponent<LoadoutWeaponSelectionItem>();
            //loadoutDataList[i].loadoutIndex = i;
            loadoutWeaponSelects.Add(temp);
            temp.weaponData = GlobalDatabase.singleton.allWeaponDatas[i];
            temp.weaponIndex = i;
        }
    }

    public void OnSelectLoadoutCallback(int selectedIndex, int selectedWeaponIndex1, int selectedWeaponIndex2, int selectedEquipment1, int selectedEquipment2)
    {
        selectedLoadoutIndex = selectedIndex;
        selectedMainWeaponIndex = selectedWeaponIndex1;
        selectedSecondWeaponIndex = selectedWeaponIndex2;
        selectedEquipmentIndex1 = selectedEquipment1;
        selectedEquipmentIndex2 = selectedEquipment2;
        loadoutPreviewUI.SetPreviewInfo(loadoutDataList[selectedLoadoutIndex]);
        DisableAllSelectedVisuals();
        loadoutItems[selectedLoadoutIndex].ToggleSelectVisual(true);
        EnablePreview();
        SetLoadoutDataToPreferences();
    }
    public void DisableAllSelectedVisuals()
    {
        for (int i = 0; i < loadoutItems.Count; i++)
        {
            loadoutItems[i].DeselectLoadout();
        }
    }
    public void DisablePreview()
    {
        loadoutPreviewUI.gameObject.SetActive(false);
    }
    public void EnablePreview()
    {
        loadoutPreviewUI.gameObject.SetActive(true);
    }
    public void EnableWeaponSelection()
    {
        loadoutWeaponSelectionUI.SetActive(true);
    }
    public void DisableWeaponSelection()
    {
        loadoutWeaponSelectionUI.SetActive(false);
    }
    public void CloseLoadoutButtonsVisual()
    {
        loadoutButtonsUI.SetActive(false);
    }
    public void OpenLoadoutButtonsVisual()
    {
        loadoutButtonsUI.SetActive(true);
    }
    public void ToggleCustomizationMenu(bool value)
    {
        loadoutCustomization.gameObject.SetActive(value);
    }
    public void ToggleCustomizeSelectionUI(bool value)
    {
        loadoutAttachmentSelections.SetActive(value);
    }
    public void ToggleCustomizeButtonsUI(bool value)
    {
        customButtonsHolder.gameObject.SetActive(value);

        if (loadoutCustomization.sightObjects.Count <= 0) customButtonsHolder.buttons[0].SetActive(false);
        else customButtonsHolder.buttons[0].SetActive(value);

        if (loadoutCustomization.barrelObjects.Count <= 0) customButtonsHolder.buttons[1].SetActive(false);
        else customButtonsHolder.buttons[1].SetActive(value);

        if (loadoutCustomization.underbarrelObjects.Count <= 0) customButtonsHolder.buttons[2].SetActive(false);
        else customButtonsHolder.buttons[2].SetActive(value);

        if (loadoutCustomization.leftbarrelObjects.Count <= 0) customButtonsHolder.buttons[3].SetActive(false);
        else customButtonsHolder.buttons[3].SetActive(value);

        if (loadoutCustomization.rightbarrelObjects.Count <= 0) customButtonsHolder.buttons[4].SetActive(false);
        else customButtonsHolder.buttons[4].SetActive(value);
    }
}
