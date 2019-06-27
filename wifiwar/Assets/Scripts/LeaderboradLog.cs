using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboradLog : MonoBehaviour
{
    void Start()
    {
        Leaderboard.Current.scoreChanged += ScoreChanged;
    }

    // Update is called once per frame
    void ScoreChanged()
    {
        var leaders = Leaderboard.Current.Leaders;
        if (leaders.Count > 0)
            Debug.Log(string.Format("Current leader: {0}, kills: {1}", leaders[0].name, leaders[0].kills));
    }
}
