using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientMessages;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UIElements;
using UdpCNetworkDriver = Unity.Networking.Transport.GenericNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket, 
    Unity.Networking.Transport.DefaultPipelineStageCollection>;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WebSocketServer;

public class ServerWebSocket : MonoBehaviour
{
    public int listenPort = 9002;
    public ushort proxyPort = 9001;
    
    private Dictionary<string, int> connectionIndexMap;

    /*
       Rather than rewrite the underlying network system for web sockets
       we'll just connect our server to itself and forward messages between the
       two systems.
       */
    private UdpCNetworkDriver mProxy;
    //public NativeArray<NetworkConnection> mProxConnections;
    public ArrayList mProxyConnections;
    private int connectionCount = 0;

    void ConnectClientProxy(string clientId)
    {
        Debug.Log("WebSocketSever: Connecting:"+clientId+" to internal proxy...");
        try
        {
            NetworkEndPoint ep = new NetworkEndPoint();
            var endpoint = NetworkEndPoint.Parse("127.0.0.1", proxyPort);
            NetworkConnection con = mProxy.Connect(endpoint);
           // mProxConnections[connectionCount] = con;
           mProxyConnections.Add(con);
            connectionIndexMap.Add(clientId, connectionCount);
            connectionCount++;
            Debug.Log("WebSocketServer: Connected to proxy.");
        }
        catch (Exception e)
        {
            Debug.Log("WebSocketSever: ERROR:"+e.Message+" stack:"+e.StackTrace);
        }
    }

    void DestroyProxy()
    {
        //mProxConnections.Dispose();
        mProxy.Dispose();
    }

    void Start()
    {
        connectionIndexMap = new Dictionary<string, int>();
        mProxyConnections = new ArrayList();
        //mProxConnections = new NativeList<NetworkConnection>(420, Allocator.Persistent);        
        mProxy = new UdpCNetworkDriver(new INetworkParameter[0]);
        if (mProxy.Bind(NetworkEndPoint.Parse(IPAddress.Loopback.ToString(), proxyPort)) != 0)
        {
            Debug.Log("Proxy server failed to bind.");
            return;
        }
        mProxy.Listen();

            // Create a listen server on localhost with port 80
        Server server = new Server(new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort));

        Debug.Log("RJ STARTING WEBSOCKET SERVER");
        /*
         * Bind required events for the server
         */

        server.OnClientConnected += (object sender, OnClientConnectedHandler e) => 
        {
            Debug.Log("RJ CONNECTED");
            
            // Connects the incoming websocket to the com.unity.transport systems.
            ConnectClientProxy(e.GetClient().GetGuid());
            
        };

        server.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
        {
            Debug.Log("RJ DISCONNECTED:"+e.GetClient().GetGuid());
            
            // Disconnect from the proxy server when a client is gone.
       //     int connectionIdx = connectionIndexMap[e.GetClient().GetGuid()];
      //      mProxConnections[connectionIdx] = default(NetworkConnection);
        };

        server.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
        {
            //e.GetClient().GetGuid()
            string clientId = e.GetClient().GetGuid();
            string msg = e.GetMessage();
            Debug.Log("RJ MESSAGE:"+msg+" from:"+clientId);

            string[] args = msg.Split(',');
            if (args[0].Equals("move"))
            {
                
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
    
    /* THIS IS THE FLECK 
    // Start is called before the first frame update
    void Start()
    {
        FleckLog.Level = LogLevel.Debug;
        webServer = new WebSocketServer("ws://0.0.0.0:"+listenPort);
        mProxConnections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        connectionIndexMap = new Dictionary<Guid, int>();
        
        Debug.Log("RJ Starting websocket server to listen on port:"+listenPort);
        
        webServer.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                //ConnectClientProxy(socket);
                Debug.Log("RJ WebSocket OPEN!");
                //socket.Send("Helllooooo");
                webSocketConnections.Add(socket);
                
            };

            socket.OnClose = () =>
            {
                Debug.Log("RJ WebSocket CLOSED!");
                webSocketConnections.Remove(socket);
            };
            socket.OnMessage = message =>
            {
                Debug.Log("RJ On Message:"+message);
                
                // Find the associated proxy connection, then forward the message.
                int idx = connectionIndexMap[socket.ConnectionInfo.Id];
                Debug.Log("OnMessage:"+message);
                //TODO data stream writer.
                //DataStreamWriter writer = new DataStreamWriter();
                //ClientMessageMoveJoy moveJoy = new ClientMessageMoveJoy();
                //moveJoy.SendTo(mProxy, mProxConnections[idx]);
                
                // writer.WriteBytes(message.
                //mProxConnections[idx].Send(mProxy, new DataStreamWriter());

                //connections.ToList().ForEach(s => s.Send("Echo:" + message));
            };

        });
    }
*/
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnDestroy()
    {
       DestroyProxy();
    }
}
