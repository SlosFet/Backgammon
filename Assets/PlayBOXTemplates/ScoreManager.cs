using System;
using System.Collections.Generic;
using System.Linq;

public static class ScoreManager
{
    public static Dictionary<int, int> Scores = new Dictionary<int, int>();

    public static void AddScore(int playerId)
    {
        if (!Scores.ContainsKey(playerId))
            Scores.TryAdd(playerId, 0);
    }

    public static void RemoveScore(int playerId)
    {
        if (Scores.ContainsKey(playerId))
            Scores.Remove(playerId);
    }

    public static void UpdateScore(int playerId, int score)
    {
        Scores[playerId] = score;
        SendScoresToLauncher();
    }

    public static void ResetScore()
    {
        Scores.Clear();
    }

    public static void SendScoresToLauncher()
    {
        UnityPipeCommunication.ClearLogFile();
        foreach (var score in Scores)
        {
            UnityPipeCommunication.SendMessageToElectron($"SCORE:{score.Key}:{score.Value}:{DateTime.Now}:{PlayBoxLauncherDataManager.RoundToNearestVolume()}");
        }
    }

    public static List<ScoreList> GetOrderedDictionary()
    {
        var orderedDict = Scores.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        List<ScoreList> list = new List<ScoreList>();
        foreach (var dict in orderedDict)
        {
            list.Add(new ScoreList() { playerId = dict.Key, score = dict.Value });
        }

        return list;
    }
}


public class ScoreList
{
    public int playerId;
    public int score;
}
