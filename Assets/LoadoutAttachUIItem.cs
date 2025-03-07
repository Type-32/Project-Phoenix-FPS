using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutAttachUIItem : MonoBehaviour
{
    [HideInInspector] public LoadoutSelectionScript script;
    public WeaponAttachmentData weaponAttachmentData;
    public Image icon;
    public Text text;
    //public int attachmentGlobalIndex = 0;
    private void Awake()
    {
        script = GetComponentInParent<LoadoutSelectionScript>();
    }
    public void SetInfo(WeaponAttachmentData data)
    {
        weaponAttachmentData = data;
        icon.sprite = data.attachmentIcon;
        text.text = data.attachmentName;
        //attachmentGlobalIndex = FindIndexFromData(data);
    }
    public int FindIndexFromData(WeaponAttachmentData data)
    {
        for(int i = 0; i < GlobalDatabase.singleton.allWeaponAttachmentDatas.Count; i++)
        {
            if (GlobalDatabase.singleton.allWeaponAttachmentDatas[i] == data) return i;
        }
        return -1;
    }
    public void OnButtonClick()
    {
        script.loadoutCustomization.ToggleAllAttachmentUI(false);
        script.ToggleCustomizeButtonsUI(true);
        script.ToggleCustomizeSelectionUI(false);
        script.loadoutDataList[script.selectedLoadoutIndex].SetAttachment(weaponAttachmentData, weaponAttachmentData.attachmentType, script.forSelectedSlot);
        script.customButtonsHolder.SetAllIcons(script.forSelectedSlot);
        script.SetLoadoutDataToPreferences();
        Launcher.Instance.SetLoadoutValuesToPlayer();
    }
}
