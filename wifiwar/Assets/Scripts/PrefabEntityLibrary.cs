using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEngine;

public class PrefabEntityLibrary : MonoBehaviour
{ 
    public GameObject playerPrefab;

   // public static Queue<ClientContext> PlayerSpawnQueue;
   public static LockFreeQueue<int> PlayerSpawnQueue;

   private EntityManager em;
   
    // Start is called before the first frame update
    void Start()
    {
        PlayerSpawnQueue = new LockFreeQueue<int>();
        em = World.Active.EntityManager;
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

                var entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, World.Active);
                var entity = em.Instantiate(entityPrefab);
                
                /* Network component stores the connection index on the entity. */
                var networkComponent = new NetworkComponent() {connectionIdx = connectionIndex};
                em.AddComponentData(entity, networkComponent);

                Translation translation = em.GetComponentData<Translation>(entity);
                translation.Value = new float3(0.0f,0.0f,0.0f);
                
                ClientContext ctx = new ClientContext();
                ctx.connectionIndex = connectionIndex;
                ctx.playerEntity = entity;
            
                ClientContext.ClientContexts.Add(connectionIndex, ctx);
            }
        }
    }
}
