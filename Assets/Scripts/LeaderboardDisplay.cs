using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardDisplay : MonoBehaviour
{
    public TMP_Text leaderboardText;
    public int maxEntries = 5;

    void Start()
    {
        UpdateLeaderboard();
    }

    public void UpdateLeaderboard()
    {
        List<float> topTimes = GetComponent<LeaderboardManager>().GetTopTimes(maxEntries);
        leaderboardText.text = "";

        for (int i = 0; i < topTimes.Count; i++)
        {
            leaderboardText.text += FormatTime(topTimes[i]) + "\n";
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000F);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
