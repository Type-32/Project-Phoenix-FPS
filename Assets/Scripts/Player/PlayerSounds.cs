using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSounds : MonoBehaviour
{
    [Space]
    [Header("Scirpt References")]
    public PlayerControllerManager player;
    public PlayerStats stats;

    [Space]
    [Header("Player Attribute Sounds")]
    public AudioClip[] playerHurtClips = default;
    public AudioClip[] armorDamagedClips = default;
    public AudioClip[] hitmarkerClips = default;
    public AudioClip[] heavyHitmarkerClips = default;
    public AudioClip[] killmarkerClips = default;
    public AudioClip[] armorBreakMarkerClips = default;

    [Space]
    [Header("Ground Surface Material Audio Clips")]
    public AudioClip[] dirtClips = default;
    public AudioClip[] woolClips = default;
    public AudioClip[] woodClips = default;
    public AudioClip[] concreteClips = default;
    public AudioClip[] metalClips = default;
    public AudioClip[] thinLiquidClips = default;
    public AudioClip[] thickLiquidClips = default;
    public AudioClip[] defaultClips = default;
    private float footstepTimer = 0f;
    private float GetCurrentOffset => stats.isCrouching ? stats.baseStepSpeed * stats.crouchStepMultiplier : stats.isSprinting ? stats.baseStepSpeed * stats.sprintStepMultiplier : stats.baseStepSpeed;

    void Update()
    {
        if (!player.pv.IsMine) return;
        FootstepHandler();
    }
    
    void FootstepHandler()
    {
        if (!player.body.isGrounded) return;
        if (Input.GetAxis("Vertical") == 0f && Input.GetAxis("Horizontal") == 0f) return;
        footstepTimer -= Time.deltaTime;
        if(footstepTimer <= 0f)
        {
            InvokePlayerFootsteps();
        }
        stats.footstepAS.volume = Mathf.Lerp(stats.footstepAS.volume, stats.isSprinting ? 0.1f : stats.isCrouching ? 0.02f : 0.06f, 4 * Time.deltaTime);
    }
    public void InvokePlayerFootsteps()
    {
        player.pv.RPC(nameof(RPC_InvokePlayerFootsteps), RpcTarget.All, player.pv.ViewID);
    }

    [PunRPC]
    public void RPC_InvokePlayerFootsteps(int viewID)
    {
        if (player.pv.ViewID != viewID) return;
        if (Physics.Raycast(player.fpsCam.transform.position, Vector3.down, out RaycastHit hit, 4))
        {
            switch (hit.collider.tag)
            {
                case "Footsteps/Wood":
                    stats.footstepAS.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
                    break;
                case "Footsteps/Wool":
                    stats.footstepAS.PlayOneShot(woolClips[Random.Range(0, woolClips.Length - 1)]);
                    break;
                case "Footsteps/Concrete":
                    stats.footstepAS.PlayOneShot(concreteClips[Random.Range(0, concreteClips.Length - 1)]);
                    break;
                case "Footsteps/Metal":
                    stats.footstepAS.PlayOneShot(metalClips[Random.Range(0, metalClips.Length - 1)]);
                    break;
                case "Footsteps/Dirt":
                    stats.footstepAS.PlayOneShot(dirtClips[Random.Range(0, dirtClips.Length - 1)]);
                    break;
                case "Footsteps/ThinLiquid":
                    stats.footstepAS.PlayOneShot(thinLiquidClips[Random.Range(0, thinLiquidClips.Length - 1)]);
                    break;
                case "Footsteps/ThickLiquid":
                    stats.footstepAS.PlayOneShot(thickLiquidClips[Random.Range(0, thickLiquidClips.Length - 1)]);
                    break;
                default:
                    stats.footstepAS.PlayOneShot(defaultClips[Random.Range(0, defaultClips.Length - 1)]);
                    break;
            }
        }
        footstepTimer = GetCurrentOffset;
    }
    public void InvokePlayerHurtAudio()
    {
        stats.playerInternalAS.PlayOneShot(playerHurtClips[Random.Range(0, playerHurtClips.Length - 1)]);
    }
    public void InvokeArmorDamagedAudio()
    {
        stats.playerInternalAS.PlayOneShot(armorDamagedClips[Random.Range(0, armorDamagedClips.Length - 1)]);
    }
    public void InvokeHitmarkerAudio(UIManager.HitmarkerType type)
    {
        switch (type)
        {
            case UIManager.HitmarkerType.Hitmarker:
                stats.playerInternalAS.PlayOneShot(hitmarkerClips[Random.Range(0, hitmarkerClips.Length - 1)]);
                break;
            case UIManager.HitmarkerType.Killmarker:
                stats.playerInternalAS.PlayOneShot(killmarkerClips[Random.Range(0, killmarkerClips.Length - 1)]);
                break;
            case UIManager.HitmarkerType.HeavyHitmarker:
                stats.playerInternalAS.PlayOneShot(heavyHitmarkerClips[Random.Range(0, heavyHitmarkerClips.Length - 1)]);
                break;
            case UIManager.HitmarkerType.ArmorBreakMarker:
                stats.playerInternalAS.PlayOneShot(armorBreakMarkerClips[Random.Range(0, armorBreakMarkerClips.Length - 1)]);
                break;
        }
        
    }
}
