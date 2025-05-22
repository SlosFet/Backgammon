using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelectScreen : MonoBehaviour
{
    public void TwoPlayer()
    {
        UnityPipeCommunication.ClearLogFile();
        UnityPipeCommunication.SendMessageToElectron($"SCORE:{3}:{0}:{DateTime.Now}:{PlayBoxLauncherDataManager.RoundToNearestVolume()}");
        UnityPipeCommunication.SendMessageToElectron($"SCORE:{4}:{0}:{DateTime.Now}:{PlayBoxLauncherDataManager.RoundToNearestVolume()}");
        SceneManager.LoadScene(1);
    }

    public void FourPlayer()
    {
        UnityPipeCommunication.ClearLogFile();
        UnityPipeCommunication.SendMessageToElectron($"SCORE:{3}:{0}:{DateTime.Now}:{PlayBoxLauncherDataManager.RoundToNearestVolume()}");
        UnityPipeCommunication.SendMessageToElectron($"SCORE:{4}:{0}:{DateTime.Now}:{PlayBoxLauncherDataManager.RoundToNearestVolume()}");
        UnityPipeCommunication.SendMessageToElectron($"SCORE:{5}:{0}:{DateTime.Now}:{PlayBoxLauncherDataManager.RoundToNearestVolume()}");
        UnityPipeCommunication.SendMessageToElectron($"SCORE:{6}:{0}:{DateTime.Now}:{PlayBoxLauncherDataManager.RoundToNearestVolume()}");
        SceneManager.LoadScene(2);
    }
}
