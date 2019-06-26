using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderbordUI : MonoBehaviour
{
    public InputField nameField;
    public InputField killsField;
    public InputField leadersField;
    
    void Start()
    {
        Leaderboard.Current.scoreChanged += ScoreChanged;
    }

    public void AddLeader()
    {
        string name = nameField.text;
        int kills = int.Parse(killsField.text);
        
        Leaderboard.Current.AddScore(name, kills);
    }

    public void RemovePlayer()
    {
        string name = nameField.text;
        Leaderboard.Current.RemovePlayer(name);
    }

    void ScoreChanged()
    {
        string leaderString = "";
        foreach (var leader in Leaderboard.Current.Leaders)
            leaderString += leader.name + ": " + leader.kills.ToString() + "\r\n";

        leadersField.text = leaderString;
    }
}
