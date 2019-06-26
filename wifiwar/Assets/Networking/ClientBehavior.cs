using System;
using System.Collections;
using System.Collections.Generic;
using ClientMessages;
using ServerMessages;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEditor.VersionControl;
using UdpCNetworkDriver = Unity.Networking.Transport.GenericNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket, 
    Unity.Networking.Transport.DefaultPipelineStageCollection>;

static class IncomingServerMessageParser
{
    public static void ParseIncomingMessage(UdpCNetworkDriver driver, NetworkConnection connection, DataStreamReader stream)
    {
        var readerCtx = default(DataStreamReader.Context);
        uint messageID = stream.ReadUInt(ref readerCtx);

        if (messageID == ServerMessages.ServerMessageHello.id)
        {
            ServerMessageHello hello = new ServerMessageHello();
           // hello.Recieve(stream);
        }
    }
}

struct ClientUpdateJob : IJob
{
    public UdpCNetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NativeArray<byte> done;

    public void Execute()
    {
        if (!connection[0].IsCreated)
        {
            if (done[0] != 1)
                Debug.Log("Something went wrong during connect");
            return;
        }
		
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        
        while ((cmd = connection[0].PopEvent(driver, out stream)) != 
               NetworkEvent.Type.Empty)
        {
            switch (cmd)
            {
                case NetworkEvent.Type.Connect:
                    Debug.Log("CLIENT: Our client is now connected to the server");
                    ServerConnection.init(driver, connection[0]);            
                    // Sending a hello...
                    ClientMessageHello hello = new ClientMessageHello();
                    ServerConnection.Send(hello);
                    break;
                case NetworkEvent.Type.Data:
                    Debug.Log("CLIENT: Got Back data from the server.");
                    try
                    {
                        IncomingServerMessageParser.ParseIncomingMessage(driver, connection[0], stream);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Debug.Log("CLIENT: IndexOutOfRange, bad server data?? disconnecting.");
                        connection[0] = default(NetworkConnection);
                    }

                    break;
                case NetworkEvent.Type.Disconnect:
                    Debug.Log("CLIENT: Our client was disconnected...");
                    //connection[0] = default(NetworkConnection);
                   // connection[0] = driver.Connect(endpoint);
                    break;
                
            }
        }
    }
}

public class ClientBehavior : MonoBehaviour
{
    public UdpCNetworkDriver m_Driver;
    public NativeArray<NetworkConnection> m_Connection;
    public NativeArray<byte> m_Done;
    
    public JobHandle ClientJobHandle;

    public string serverAddress;
    public ushort serverPort;
    
    void Start ()
    {
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        
        m_Connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        m_Done = new NativeArray<byte>(1, Allocator.Persistent);
        NetworkEndPoint ep = new NetworkEndPoint();
        
        var endpoint = NetworkEndPoint.Parse(serverAddress, serverPort);
        Debug.Log("CLIENT: Connecting to server at "+serverAddress);
        m_Connection[0] = m_Driver.Connect(endpoint);
    }
    
    public void OnDestroy()
    {
        ClientJobHandle.Complete();
        m_Connection.Dispose();
        m_Driver.Dispose();
        m_Done.Dispose();
    }
	
    void Update()
    {
        ClientJobHandle.Complete();
        var job = new ClientUpdateJob
        {
            driver = m_Driver,
            connection = m_Connection,
            done = m_Done
        };
        ClientJobHandle = m_Driver.ScheduleUpdate();
        ClientJobHandle = job.Schedule(ClientJobHandle);
    }
}
