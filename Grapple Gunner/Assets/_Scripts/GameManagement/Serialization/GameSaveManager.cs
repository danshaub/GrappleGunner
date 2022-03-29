using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class GameSaveManager : SingletonPersistent<GameSaveManager>
{
    public byte fileNumber { get; private set; } = 0;

    public void ChangeProfile(byte file)
    {
        fileNumber = file;
        
        if (!IsSaveFile())
        {
            InitializeFile();
        }
        
        LoadGame();
    }

    private string GetFilePath()
    {
        return string.Concat(Application.persistentDataPath, "/game_save", fileNumber.ToString(), "/");
    }
    public bool IsSaveFile()
    {
        return Directory.Exists(GetFilePath());
    }

    public void InitializeFile()
    {
        Directory.CreateDirectory(GetFilePath());

        // Initialize Profile
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(GetFilePath(), "profile_data"));
        bf.Serialize(file, JsonUtility.ToJson(ProfileData.CreateInstance<ProfileData>()));

        file.Close();

        // Initialize Options
        bf = new BinaryFormatter();
        file = File.Create(string.Concat(GetFilePath(), "options_data"));
        bf.Serialize(file, JsonUtility.ToJson(OptionsData.CreateInstance<OptionsData>()));

        file.Close();
    }

    public void SaveGame()
    {
        if (!IsSaveFile())
        {
            InitializeFile();
        }

        SaveProfileData();
        SaveOptionsData();
    }

    public void SaveProfileData()
    {
        if (GameManager.Instance.profile == null)
        {
            GameManager.Instance.profile = ProfileData.CreateInstance<ProfileData>();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(GetFilePath(), "profile_data"));
        bf.Serialize(file, JsonUtility.ToJson(GameManager.Instance.profile));

        file.Close();
    }

    public void SaveOptionsData()
    {

        if (GameManager.Instance.options == null)
        {
            GameManager.Instance.options = OptionsData.CreateInstance<OptionsData>();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(GetFilePath(), "options_data"));
        bf.Serialize(file, JsonUtility.ToJson(GameManager.Instance.options));

        file.Close();
    }

    public void LoadGame()
    {
        if (!IsSaveFile())
        {
            SaveGame();
        }

        LoadProfileData();
        LoadOptionsData();
    }

    public void LoadProfileData()
    {
        if (GameManager.Instance.profile == null)
        {
            GameManager.Instance.profile = ProfileData.CreateInstance<ProfileData>();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(string.Concat(GetFilePath(), "profile_data"), FileMode.Open);
        ProfileData data = ProfileData.CreateInstance<ProfileData>();
        JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), data);

        GameManager.Instance.profile.unlockedLevels = data.unlockedLevels;

        file.Close();
    }
    public void LoadOptionsData()
    {
        if (GameManager.Instance.options == null)
        {
            GameManager.Instance.options = OptionsData.CreateInstance<OptionsData>();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(string.Concat(GetFilePath(), "options_data"), FileMode.Open);
        OptionsData data = OptionsData.CreateInstance<OptionsData>();
        JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), data);

        GameManager.Instance.options.useSpeedLines = data.useSpeedLines;
        GameManager.Instance.options.snapTurn = data.snapTurn;

        file.Close();
    }

    public void ResetFile()
    {
        Directory.Delete(GetFilePath(), true);
        InitializeFile();
        LoadGame();
    }
}
