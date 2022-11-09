using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;
    public CurrentMatchManager matchManager;
    public Transform killMSGHolder;
    public GameObject killMSGPrefab;
    public GameObject FreeForAllUI;
    public GameObject TeamDeathMatchUI;
    public GameObject KingOfTheHillUI;
    public GameObject DropZonesUI;
    public GameObject MatchFinishUI;
    public GameObject MatchFinishStatsUI;

    [Space]
    [Header("Main References")]
    public Text endMatchMessage;
    public Text KDRatio;
    public Text playerName;
    public Text totalKills;
    public Text totalDeaths;
    public Text totalGainedXP;
    public Text totalGainedCoins;
    public Slider XPSlider;
    public Text levelText;

    [Space]
    [Header("FFA References")]
    public Text topPlayerName;
    public Text topPlayerScore;
    public Text timeText;
    public Text requirementText;
    float sliderXPTemp;

    bool endMatchMenuEnabled = false;

    private void Awake()
    {
        instance = this;
        matchManager = FindObjectOfType<CurrentMatchManager>();
    }
    private void Update()
    {
        if (endMatchMenuEnabled)
        {
            XPSlider.value = Mathf.Lerp(XPSlider.value, sliderXPTemp, Time.deltaTime * 0.8f);
        }
    }
    public Sprite FindWeaponIcon(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.singleton.allWeaponDatas[i].itemIcon;
        }
        return null;
    }
    public void SetMatchEndMessage(string msg)
    {
        endMatchMessage.text = msg;
    }
    public void ToggleMatchEndUI(bool toggle)
    {
        MatchFinishUI.SetActive(toggle);
    }
    public void ToggleTDM_UI(bool toggle)
    {
        TeamDeathMatchUI.SetActive(toggle);
    }
    public void ToggleFFA_UI(bool toggle)
    {
        FreeForAllUI.SetActive(toggle);
    }
    public void ToggleKOTH_UI(bool toggle)
    {
        KingOfTheHillUI.SetActive(toggle);
    }
    public void ToggleDZ_UI(bool toggle)
    {
        DropZonesUI.SetActive(toggle);
    }
    public void ToggleMatchEndStats(bool toggle, float delay)
    {
        StartCoroutine(TGL_MatchEndStats(toggle, delay));
        endMatchMenuEnabled = toggle;
    }
    IEnumerator TGL_MatchEndStats(bool toggle, float delay)
    {
        yield return new WaitForSeconds(delay);
        MatchFinishStatsUI.SetActive(toggle);
    }
    public void SetFFAMaxKillRequirement(int amount)
    {
        requirementText.text = "Get " + amount + " kills to win the game!";
    }
    public void SetMatchEndStats(string playerName, int totalKills, int totalDeaths, int totalGainedXP, int totalGainedCoins, int level, int xp)
    {
        this.totalKills.text = "Total Kills: " + totalKills.ToString();
        this.totalDeaths.text = "Total Deaths: " + totalDeaths.ToString();
        KDRatio.text = "K/D: " + ((float)totalKills / (float)totalDeaths).ToString();
        this.totalGainedCoins.text = "Resulting Money Gained: " + totalGainedCoins.ToString();
        this.totalGainedXP.text = "Gained XP in match: " + totalGainedXP.ToString();
        this.XPSlider.value = xp / (level * 500);
        this.levelText.text = (totalGainedXP + xp >= level * 500) ? ("Level " + level.ToString() + " > " + (level + 1).ToString()) : ("Level " + level.ToString());
        sliderXPTemp = ((float)totalGainedXP / ((float)level * 500f)) + xp;
    }
}
