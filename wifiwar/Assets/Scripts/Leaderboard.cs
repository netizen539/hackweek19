using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Current { get; set; }
    public int maxLeaders = 5;
    private List<Leader> _leaders = new List<Leader>();
    private ulong? _runningLeader;
    private uint _runningScore;

    public delegate void ScoreChangeDelegate();

    public event ScoreChangeDelegate scoreChanged;

    void Awake()
    {
        Current = this;
    }

    public struct Leader
    {
        public ulong playerId;
        public uint kills;

        public Leader(ulong id, uint kills)
        {
            this.playerId = id;
            this.kills = kills;
        }
    }

    public List<Leader> Leaders
    {
        get { return _leaders; }
    }

    public ulong? RunningLeader
    {
        get
        {
            ulong historicalId, historicalScore, runningId, runningScore;
            if (_runningLeader.HasValue)
            {
                runningId = _runningLeader.Value;
                runningScore = _runningScore;
            }
            else
            {
                runningId = 0;
                runningScore = 0;
            }

            if (_leaders.Count > 0)
            {
                historicalId = _leaders[0].playerId;
                historicalScore = _leaders[0].kills;
            }
            else
            {
                historicalId = 0;
                historicalScore = 0;
            }

            ulong? ret = null;
            if (runningScore > 0)
                ret = runningId;
            if (historicalScore > runningScore)
                ret = historicalId;
            return ret;
        }
    }

    public void AddScore(ulong id, uint kills)
    {
        if (kills <= 0)
            return;
        for (int i = 0; i < _leaders.Count; ++i)
            if (_leaders[i].playerId == id)
            {
                if (_leaders[i].kills < kills)
                {
                    _leaders.RemoveAt(i);
                    break;
                }

                return;
            }
        
        NewScore(id, kills);
    }

    public void RemovePlayer(ulong id)
    {
        for (int i = 0; i < _leaders.Count; ++i)
            if (_leaders[i].playerId == id)
            {
                _leaders.RemoveAt(i);
                if (scoreChanged != null)
                    scoreChanged();
                break;
            }
    }
    
    private void NewScore(ulong id, uint kills)
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

            _leaders.Add(new Leader(id, kills));
        }
        else
        {
            _leaders.Insert(i, new Leader(id, kills));
            if (_leaders.Count > maxLeaders)
                _leaders.RemoveAt(maxLeaders);
        }

        if (scoreChanged != null)
            scoreChanged();
    }

    public void UpdateRunningLeader(ulong id, uint score)
    {
        if (_runningLeader.HasValue)
        {
            if (_runningScore < score)
            {
                _runningLeader = id;
                _runningScore = score;
            }
        }
        else if (score > 0)
        {
            _runningLeader = id;
            _runningScore = score;
        }

        if (scoreChanged != null)
            scoreChanged();

        Debug.Log(string.Format("Current leader: {0} with {1} kills", RunningLeader.Value, _runningScore));
    }

    public void RemoveRunningLeader(ulong id)
    {
        if (_runningLeader.HasValue)
        {
            if (_runningLeader.Value == id)
            {
                _runningLeader = null;
                if (scoreChanged != null)
                    scoreChanged();
            }
        }
    }
}
