using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class MoveMessage
{
    public string clientId;
    public Vector2 movePost;
}

public class NetMoveQueue : MonoBehaviour
{
    public static LockFreeQueue<MoveMessage> queue = new LockFreeQueue<MoveMessage>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveMessage movemsg;
        if (queue.Dequeue(out movemsg))
        {
            Debug.Log("RJ MOVE:"+movemsg.movePost);
            Entity player = NetworkState.playerEntityCache[movemsg.clientId];
            MovementComponent move = World.Active.EntityManager.GetComponentData<MovementComponent>(player);
            move.playerDirectionAxis = movemsg.movePost;
            World.Active.EntityManager.SetComponentData(player, move);

            /*
            EntityQuery query = World.Active.EntityManager.CreateEntityQuery(typeof(Player1_tag));
            using (var players = query.ToEntityArray(Allocator.Persistent))
            {
                Debug.Log("RJ found players:"+players.Length);
                foreach (var p in players)
                {
                    Debug.Log("Player:"+p.Index);
                    MovementComponent move = World.Active.EntityManager.GetComponentData<MovementComponent>(p);
                    move.playerDirectionAxis = movemsg.movePost;
                    World.Active.EntityManager.SetComponentData(p, move);
                }
            }
            */
        }
    }
}
