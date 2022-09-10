using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;
using Photon.Realtime;

public class PlayerControllerManager : MonoBehaviourPunCallbacks, IDamagable
{

    [Header("Script Control")]
    public PlayerControls controls;
    public PlayerStats stats;
    public PlayerSounds sfx;
    public UIManager ui;
    public MouseLookScript cam;
    public EquipmentHolder holder;
    public IDamagable damagable;
    //public GadgetUsageScript gadgetFunc;

    [Space]
    [Header("Character Customization")]
    [SerializeField] GameObject playerHead, playerBody, playerFeet1, playerFeet2;
    [SerializeField] Material playerHeadMaterial, playerBodyMaterial, playerFeetMaterial;


    [Space]
    [Header("References")]
    public CharacterController body;
    public CapsuleCollider capsuleCollider;
    public MouseLookScript fpsCam;
    public GameObject deathCam;
    public Animator cameraAnim;
    public Recoil recoilScript;
    public Transform groundCheck;
    public GameObject playerDeathEffect;

    [Space]
    [Header("Ground Masks")]
    public LayerMask groundMask;

    [Space]
    [Header("Volume Effects")]
    [SerializeField] Volume playerVolumeEffect, playerHurtEffect, armorHurtEffect;

    [Space]
    [Header("Multiplayer")]
    public PhotonView pv;
    public PlayerManager playerManager;

    private bool hasArmor = false;

    private void Awake()
    {
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
        pv = GetComponent<PhotonView>();
        //ui = FindObjectOfType<UIManager>();
        //if(playerHeadMat != null) gameObject.GetComponent<MeshRenderer>playerHeadMat
    }
    private void Start()
    {
        if (pv.IsMine)
        {
            recoilScript = FindObjectOfType<Recoil>();
            DerivePlayerStatsToHUDInitialize();
        }
        else
        {
            Destroy(ui.gameObject);
        }
    }
    private void Update()
    {
        if (!pv.IsMine) return;
        if (transform.position.y < -35) Die();
        DerivePlayerStatsToHUD();
        PlayerGUIReference();

        if (playerManager.openedLoadoutMenu)
        {
            stats.playerMovementEnabled = false;
            stats.mouseMovementEnabled = false;
        }else if (playerManager.openedOptions)
        {
            stats.playerMovementEnabled = false;
            stats.mouseMovementEnabled = false;
        }
        else
        {
            stats.playerMovementEnabled = true;
            stats.mouseMovementEnabled = true;
        }
    }
    public void GetPickupsForPlayer()
    {
        RaycastHit detectRay;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out detectRay, 3f))
        {
            Pickup temp = detectRay.collider.GetComponent<Pickup>();
            if(temp != null && temp.itemData != null)
            {
                
            }
        }
    }
    public void SetMouseSensitivity(float value)
    {
        stats.mouseSensitivity = value;
        controls.aimingMouseSensitivity = stats.mouseSensitivity * 0.8f;
    }

    public bool TakeDamage(float amount, bool bypassArmor)
    {
        bool tempflag = false;
        if (stats.health - amount <= 0) tempflag = true;
        pv.RPC(nameof(RPC_TakeDamage), pv.Owner, amount, bypassArmor);//Running the function on everyone's computer
        return tempflag;
        //ui.ShowHealthBar(2f);
        
    }
    
    #region Body Materials
    public void SetBodyMaterialColor(Color color)
    {
        playerBodyMaterial.color = color;
    }
    public void SetFeetMaterialColor(Color color)
    {
        playerFeetMaterial.color = color;
    }
    public void SetHeadMaterialColor(Color color)
    {
        playerHeadMaterial.color = color;
    }
    #endregion
    
    private void TakeHitEffect()
    {

    }
    private void DerivePlayerStatsToHUD()
    {
        if (ui == null) return;
        ui.healthBar.value = Mathf.Lerp(ui.healthBar.value, stats.health, 8 * Time.deltaTime);
        //ui.healthBarFill.color = Color.Lerp(ui.healthBarFill.color, (stats.health >= 50f) ? Color.green : (stats.health < 50f && stats.health >= 30f) ? Color.yellow : Color.red, 5 * Time.deltaTime);
        //ui.healthText.text = ((int)stats.health).ToString();
        ui.armorBar.value = Mathf.Lerp(ui.armorBar.value, stats.armor, 8 * Time.deltaTime);
        playerVolumeEffect.weight = 1f - (stats.health / 100f);
        playerHurtEffect.weight = Mathf.Lerp(playerHurtEffect.weight, 0f, 8 * Time.deltaTime);
        armorHurtEffect.weight = Mathf.Lerp(armorHurtEffect.weight, 0f, 8 * Time.deltaTime);
    }
    private void DerivePlayerStatsToHUDInitialize()
    {
        if (ui == null) return;
        ui.healthBar.maxValue = stats.healthLimit;
        ui.armorBar.maxValue = stats.armorLimit;
        stats.stress = 0;
    }
    private void PlayerGUIReference()
    {
        if (ui == null) return;
        if (playerManager.openedInventory) { stats.mouseMovementEnabled = false; stats.playerMovementEnabled = false; }
        else { stats.mouseMovementEnabled = true; stats.playerMovementEnabled = true; }
    }
    public void Die()
    {
        InvokePlayerDeathEffects();
        playerManager.Die();
        Debug.Log("Player " + stats.playerName + " was Killed");
        return;
    }
    [PunRPC]
    void RPC_TakeDamage(float amount, bool bypassArmor, PhotonMessageInfo info)
    {
        Debug.Log("Took Damage " + amount);

        //Core Take Damage Functions
        recoilScript.RecoilFire(0.4f, 0.8f, 4, 0.12f, 0, 5, 12, 5, 12);
        if (bypassArmor)
        {
            stats.health -= amount;
            playerHurtEffect.weight = 1f;
            sfx.InvokePlayerHurtAudio();
        }
        else
        {
            if (stats.armor - amount <= 0)
            {
                if (hasArmor) sfx.InvokeArmorDamagedAudio();
                hasArmor = false;
                if (stats.armor - amount < 0)
                {
                    float temp = stats.armor - amount;
                    stats.armor = 0f;
                    stats.health += temp;
                }
                else
                {
                    stats.armor = 0f;
                }
                playerHurtEffect.weight = 1f;
            }
            else
            {
                stats.armor -= amount;
                hasArmor = true;
                armorHurtEffect.weight = 1f;
                sfx.InvokePlayerHurtAudio();
            }

        }
        stats.totalAbsorbedDamage += amount;
        if (stats.health <= 0f)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
        return;
    }
    public void CallShootRPCDecals(RaycastHit hit)
    {
        Debug.LogWarning("Invoking Shoot RPC...");
        pv.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);
    }
    public void InvokeGunEffects()
    {
        Debug.LogWarning("Invoking Gun Effects RPC...");
        pv.RPC(nameof(RPC_InvokeGunEffects), RpcTarget.All);
    }
    public void InvokePlayerDeathEffects()
    {
        Debug.LogWarning("Invoking PLayer Death Effects RPC...");
        pv.RPC(nameof(RPC_InvokePlayerDeathEffects), RpcTarget.All);
    }
    [PunRPC]
    public void RPC_InvokePlayerDeathEffects()
    {
        GameObject obj = Instantiate(playerDeathEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);
        Destroy(obj, 5f);
    }
    [PunRPC]
    public void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        Debug.LogWarning("Finding Colliders...");
        if (colliders.Length != 0)
        {
            Debug.LogWarning("Colliders Found");
            GameObject bulletImpactObject = Instantiate(holder.weaponSlots[holder.weaponIndex].bulletImpactPrefab, hitPosition + hitNormal * 0.01f, Quaternion.LookRotation(-hitNormal, Vector3.up));
            Destroy(bulletImpactObject, 5f);
            bulletImpactObject.transform.SetParent(colliders[0].transform);
            //bulletImpactObject.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x,transform.rotation.y, Random.Range(0f, 90f)));
        }
    }
    [PunRPC]
    public void RPC_InvokeGunEffects()
    {
        holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound();
        holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.Play();
    }
}
