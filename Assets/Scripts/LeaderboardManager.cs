using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LeaderboardEntry
{
    public float time;
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{

    private LeaderboardData leaderboardData = new LeaderboardData();
    private const string PlayerPrefsKey = "LeaderboardData";

    public void AddTime(float time)
    {
        leaderboardData.entries.Add(new LeaderboardEntry { time = time });
        leaderboardData.entries.Sort((a, b) => a.time.CompareTo(b.time));
        SaveLeaderboard();
    }

    public List<float> GetTopTimes(int count)
    {
        List<float> topTimes = new List<float>();
        for (int i = 0; i < Mathf.Min(count, leaderboardData.entries.Count); i++)
        {
            topTimes.Add(leaderboardData.entries[i].time);
        }
        return topTimes;
    }

    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboardData);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    private void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
        }
    }
}
