using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Weapon Attachment Data", menuName = "New Weapon Attachment Data", order = 1)]
public class WeaponAttachmentData : ScriptableObject
{
    public GameObject attachmentPrefab;
    public string attachmentName;
    public Sprite attachmentIcon;
    public GunAttachments.AttachmentTypes attachmentType;
    [HideInInspector] public int attachmentIndex;
}
