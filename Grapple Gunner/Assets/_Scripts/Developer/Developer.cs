// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;

// public class Developer
// {
//     [MenuItem("Developer/LoadLevel/1")]
//     public static void LoadLevelOne()
//     {
//         SceneLoader.Instance?.LoadLevel(1);
//     }
//     [MenuItem("Developer/LoadLevel/2")]
//     public static void LoadLevelTwo()
//     {
//         SceneLoader.Instance?.LoadLevel(2);
//     }
//     [MenuItem("Developer/LoadLevel/3")]
//     public static void LoadLevelThree()
//     {
//         SceneLoader.Instance?.LoadLevel(3);
//     }
//     [MenuItem("Developer/LoadLevel/4")]
//     public static void LoadLevelFour()
//     {
//         SceneLoader.Instance?.LoadLevel(4);
//     }
//     [MenuItem("Developer/LoadLevel/Playground")]
//     public static void LoadLevelPlayground()
//     {
//         SceneLoader.Instance?.LoadLevel(0);
//     }
//     [MenuItem("Developer/LoadLevel/MainMenu")]
//     public static void LoadMainMenu()
//     {
//         SceneLoader.Instance?.LoadMainMenu(true);
//     }

//     [MenuItem("Developer/KillPlayer")]
//     public static void KillPlayer()
//     {
//         LocationManager.Instance?.KillPlayer();
//     }
//     [MenuItem("Developer/RespawnPlayer")]
//     public static void RespawnPlayer()
//     {
//         LocationManager.Instance?.RespawnPlayer();
//     }

//     [MenuItem("Developer/LoadNextLevel")]
//     public static void LoadNextLevel()
//     {
//         LocationManager.Instance?.LoadNextLevel();
//     }

//     [MenuItem("Developer/Serialization/SaveGame")]
//     public static void SaveGame()
//     {
//         GameSaveManager.Instance?.SaveGame();
//     }
//     [MenuItem("Developer/Serialization/LoadGame")]
//     public static void LoadGame()
//     {
//         GameSaveManager.Instance?.LoadGame();
//     }

//     [MenuItem("Developer/Serialization/ChangeProfile/0")]
//     public static void ChangeProfile0()
//     {
//         ChangeProfile(0);
//     }
//     [MenuItem("Developer/Serialization/ChangeProfile/1")]
//     public static void ChangeProfile1()
//     {
//         ChangeProfile(1);
//     }
//     [MenuItem("Developer/Serialization/ChangeProfile/2")]
//     public static void ChangeProfile2()
//     {
//         ChangeProfile(2);
//     }
//     [MenuItem("Developer/Serialization/ChangeProfile/3")]
//     public static void ChangeProfile3()
//     {
//         ChangeProfile(3);
//     }
//     public static void ChangeProfile(byte profile)
//     {
//         GameSaveManager.Instance?.ChangeProfile(profile);
//     }
//     [MenuItem("Developer/Serialization/ResetProfile")]
//     public static void ResetProfile()
//     {
//         GameSaveManager.Instance?.ResetFile();
//     }
//     [MenuItem("Developer/Transition/Start")]
//     public static void StartTransition()
//     {
//         VFXManager.Instance?.transitionSystem.StartTransition();
//     }
//     [MenuItem("Developer/Transition/End")]
//     public static void EndTransition()
//     {
//         VFXManager.Instance?.transitionSystem.EndTransition();
//     }
// }
