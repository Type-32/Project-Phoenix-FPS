using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using LauncherManifest;
using UserConfiguration;
using PrototypeLib.Modules.FileOpsIO;

public class UserDatabase : MonoBehaviour
{
    public static UserDatabase Instance;
    public UserDataJSON emptyUserDataJSON;
    public int levelLimiter = 800;
    public delegate void UpdateUserData(UserDataJSON userData);
    public static event UpdateUserData OnUserDataUpdated;
    private void Awake()
    {
        Instance = this;
        levelLimiter = 600;
    }
    private void Start()
    {
        ReadUserDataFromJSON();
        ReadAppearanceDataFromJSON();
        AddUserLevelXP(0);
    }
    public int CurrentLevelLimit
    {
        get
        {
            return FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).userLevel * levelLimiter;
        }
    }
    public void ReadUserDataFromJSON()
    {
        UserDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        //MenuManager.instance.SetUserGUIData(PlayerPrefs.GetString("Username"), jsonData.userLevel, (float)jsonData.userLevelXP, jsonData.userCoins);
        //Debug.Log((jsonData.userLevelXP / (jsonData.userLevel * UserDatabase.Instance.levelLimiter)));
        if (!jsonData.hasInitialized)
        {
            string content = "You have unlocked your first three weapons:\n-AK-47\n-M16\n-Beretta\nYou can go equip them in your loadouts now.";
            MenuManager.instance.AddModalWindow("Unlocked", content);
            jsonData.hasInitialized = true;
            FileOps<UserDataJSON>.WriteFile(jsonData, UserSystem.UserDataPath);
        }
    }
    public void ReadAppearanceDataFromJSON()
    {
        AppearancesDataJSON jsonData = FileOps<AppearancesDataJSON>.ReadFile(UserSystem.AppearancesConfigPath);
    }
    public void SetUserData(UserDataJSON userData)
    {
        FileOps<UserDataJSON>.WriteFile(userData, UserSystem.UserDataPath);
        OnUserDataUpdated?.Invoke(userData);
    }
    public void AddUserCurrency(int amount)
    {
        UserDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        jsonData.userCoins += amount;
        FileOps<UserDataJSON>.WriteFile(jsonData, UserSystem.UserDataPath);
        OnUserDataUpdated?.Invoke(jsonData);
    }
    public bool AddUserLevelXP(int amount)
    {
        int m_am = amount;
        UserDataJSON jsonData = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        string unlockedContent = "";
        bool ret = false;
        while (jsonData.userLevelXP + m_am >= CurrentLevelLimit)
        {
            int levelLim = CurrentLevelLimit;
            ret = true;
            int afterLeft = levelLim - jsonData.userLevelXP;
            m_am -= afterLeft;
            jsonData.userLevelXP = 0;
            jsonData.userLevel++;
            for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
            {
                if (GlobalDatabase.singleton.allWeaponDatas[i].unlockingLevel <= jsonData.userLevel)
                    unlockedContent = unlockedContent + "-" + GlobalDatabase.singleton.allWeaponDatas[i].itemName + "\n";
            }
            if (RoomManager.Instance.currentSceneIndex != 0)
            {
                MenuManager.PopupData tmp;
                tmp.title = "Level Up";
                tmp.content = "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent);
                tmp.queueType = MenuManager.PopupQueue.OnMainMenuLoad;
                if (!MenuManager.instance.queuedModalWindows.Contains(tmp)) MenuManager.instance.QueueModalWindow(tmp.title, tmp.content, MenuManager.PopupQueue.OnMainMenuLoad);
            }
            else MenuManager.instance.AddModalWindow("Level Up", "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent));
        }
        if (jsonData.userLevelXP + m_am < CurrentLevelLimit && amount != 0)
        {
            if (RoomManager.Instance.currentSceneIndex != 0) MenuManager.instance.QueueModalWindow("Level Up", "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent), MenuManager.PopupQueue.OnMainMenuLoad);
            else MenuManager.instance.AddModalWindow("Level Up", "Congratulations! You have leveled up!" + (string.IsNullOrEmpty(unlockedContent) ? "" : "\nYou have unlocked the following content:\n" + unlockedContent));
            jsonData.userLevelXP += m_am;
        }
        FileOps<UserDataJSON>.WriteFile(jsonData, UserSystem.UserDataPath);
        OnUserDataUpdated?.Invoke(jsonData);
        return ret;
    }
    public int GetUserXPValue()
    {
        return FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).userLevelXP;
    }
    public int GetUserXPLevelValue()
    {
        return FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).userLevel;
    }
    public int GetUserCoinValue()
    {
        return FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).userCoins;
    }
}
