using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor.PackageManager;
using UnityEngine;

public class PrefabEntityLibrary : MonoBehaviour
{ 
    public GameObject playerPrefab;

   // public static Queue<ClientContext> PlayerSpawnQueue;
   public static LockFreeQueue<int> PlayerSpawnQueue; 
   
    // Start is called before the first frame update
    void Start()
    {
        PlayerSpawnQueue = new LockFreeQueue<int>();
    }

    // Update is called once per frame
    void Update()
    {
        int connectionIndex;
        if (PlayerSpawnQueue.Dequeue(out connectionIndex))
        {
            if (!ClientContext.ClientContexts.ContainsKey(connectionIndex))
            {
                /* This connection doesnt have a context... new player! */

                var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, World.Active);
                /* A client has connected. Create an entity to represent the client's player
                  and assign components as needed */
                var networkComponent = new NetworkComponent() {connectionIdx = connectionIndex};
                World.Active.EntityManager.AddComponentData(entity, networkComponent);
         
                ClientContext ctx = new ClientContext();
                ctx.connectionIndex = connectionIndex;
                ctx.playerEntity = entity;
            
                ClientContext.ClientContexts.Add(connectionIndex, ctx);
            }
        }
    }
}
