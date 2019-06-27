using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientMessages;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UIElements;
using UdpCNetworkDriver = Unity.Networking.Transport.GenericNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket, 
    Unity.Networking.Transport.DefaultPipelineStageCollection>;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Unity.Entities;
using WebSocketServer;

public class ServerWebSocket : MonoBehaviour
{
    public int listenPort = 9002;
    
    private Dictionary<string, int> connectionIndexMap;
    
    void Start()
    {
        connectionIndexMap = new Dictionary<string, int>();
        Server server = new Server(new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort));
      
        server.OnClientConnected += (object sender, OnClientConnectedHandler e) => 
        {
            Debug.Log("Player Connected:"+e.GetClient().GetGuid());
            PrefabEntityLibrary.PlayerSpawnQueue.Enqueue(e.GetClient().GetGuid());

            // Connects the incoming websocket to the com.unity.transport systems.
                  
        };

        server.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
        {
            Debug.Log("Player Disconnected:"+e.GetClient().GetGuid());
 
            // Disconnect from the proxy server when a client is gone.
       //     int connectionIdx = connectionIndexMap[e.GetClient().GetGuid()];
      //      mProxConnections[connectionIdx] = default(NetworkConnection);
        };

        server.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
        {
            //e.GetClient().GetGuid()
            string clientId = e.GetClient().GetGuid();
            string msg = e.GetMessage();

            string[] args = msg.Split(',');
            if (args[0].Equals("move"))
            {
                Debug.Log("RJXXXXXXXXXXX message from server.....");
               MoveMessage mm = new MoveMessage();
               mm.clientId = e.GetClient().GetGuid();
               mm.movePost.x = (float) Convert.ToDouble(args[1]);
               mm.movePost.y = (float) (float) Convert.ToDouble(args[2]);
               NetMoveQueue.queue.Enqueue(mm);
               //  EntityQuery query = GetEntityQuery(typeof(NetworkComponent));
            } else if (args[0].Equals("fire"))
            {
                
            } else if (args[0].Equals("shield"))
            {
                
            }
        };

        server.OnSendMessage += (object sender, OnSendMessageHandler e) =>
        {
            //            Console.WriteLine("Sent message: '{0}' to client {1}", e.GetMessage(), e.GetClient().GetGuid());
        };
    }
       
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnDestroy()
    {
    }
}
