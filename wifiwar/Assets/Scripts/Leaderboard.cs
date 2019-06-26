using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Current { get; set; }
    public int maxLeaders = 5;
    private List<Leader> _leaders = new List<Leader>();

    public delegate void ScoreChangeDelegate();

    public event ScoreChangeDelegate scoreChanged;

    void Start()
    {
        Current = this;
    }

    public struct Leader
    {
        public string name;
        public int kills;

        public Leader(string name, int kills)
        {
            this.name = name;
            this.kills = kills;
        }
    }

    public List<Leader> Leaders
    {
        get { return _leaders; }
    }

    public void AddScore(string name, int kills)
    {
        for (int i = 0; i < _leaders.Count; ++i)
            if (_leaders[i].name == name)
            {
                if (_leaders[i].kills < kills)
                {
                    _leaders.RemoveAt(i);
                    break;
                }

                return;
            }
        
        NewScore(name, kills);
    }

    public void RemovePlayer(string name)
    {
        for (int i = 0; i < _leaders.Count; ++i)
            if (_leaders[i].name == name)
            {
                _leaders.RemoveAt(i);
                if (scoreChanged != null)
                    scoreChanged();
                break;
            }
    }
    
    private void NewScore(string name, int kills)
    {
        int i;
        for (i = _leaders.Count; i > 0; --i)
        {
            if (_leaders[i - 1].kills >= kills)
                break;
        }

        if (i == _leaders.Count)
        {
            if (_leaders.Count == maxLeaders)
                return;

            _leaders.Add(new Leader(name, kills));
        }
        else
        {
            _leaders.Insert(i, new Leader(name, kills));
            if (_leaders.Count > maxLeaders)
                _leaders.RemoveAt(maxLeaders);
        }

        if (scoreChanged != null)
            scoreChanged();
    }
}
