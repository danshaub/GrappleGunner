using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDirectory", menuName = "Grapple Gunner/LevelDirectory", order = 0)]
public class LevelDirectory : ScriptableObject {
    public List<string> levelNames;
    [SerializeField] private string mainMenuName;
    public string GetLevelName(int levelIndex){
        try
        {
            return levelNames[levelIndex];
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new ArgumentException("Index is out of range", nameof(levelIndex), ex);
        }
    }

    public string GetMainMenu(){
        return mainMenuName;
    }

    public bool ValidateLevelIndex(int levelIndex){
        try{
            string x = levelNames[levelIndex];
            return true;
        }
        catch{
            return false;
        }
    }
}