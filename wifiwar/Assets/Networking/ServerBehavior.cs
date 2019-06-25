﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using ClientMessages;
using ServerMessages;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Assertions;
using UdpCNetworkDriver = Unity.Networking.Transport.GenericNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket, 
    Unity.Networking.Transport.DefaultPipelineStageCollection>;

static class IncomingClientMessageParser
{
    public static void ParseIncomingMessage(DataStreamReader stream)
    {
        var readerCtx = default(DataStreamReader.Context);
        uint messageID = stream.ReadUInt(ref readerCtx);

        if (messageID == ClientMessages.ClientMessageHello.id)
        {
            ClientMessageHello hello = new ClientMessageHello();
            hello.Recieve(stream);
        }
    }
}
struct ServerUpdateConnectionsJob : IJob
{
    public UdpCNetworkDriver driver;
    public NativeList<NetworkConnection> connections;

    public void Execute()
    {
        // CleanUpConnections
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
        // AcceptNewConnections
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("SERVER: Accepted a connection");
        }
    }
}

struct ServerUpdateJob : IJobParallelForDefer
{
    public UdpCNetworkDriver.Concurrent driver;
    public NativeArray<NetworkConnection> connections;

    public void Execute(int index)
    {
        DataStreamReader stream;
        if (!connections[index].IsCreated)
            Assert.IsTrue(true);

        NetworkEvent.Type cmd;
        while ((cmd = driver.PopEventForConnection(connections[index], out stream)) !=
               NetworkEvent.Type.Empty)
        {
            switch (cmd)
            {
                case NetworkEvent.Type.Connect:
                    // doesnt seem to be run... not used on server?
                    Debug.Log("SERVER: Client Connection detected");
                    break;
                case NetworkEvent.Type.Data:
                    Debug.Log("SERVER: Client data detected");
                    try
                    {
                        IncomingClientMessageParser.ParseIncomingMessage(stream);
                        ServerMessageHello hello = new ServerMessageHello();
                        hello.SendTo(driver, connections[index]);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Debug.Log("SERVER: IndexOutOfRange, bad client data?? disconnecting.");
                        connections[index] = default(NetworkConnection);
                    }

                    break;
                case NetworkEvent.Type.Disconnect:
                    Debug.Log("SERVER: Client Disconnected");
                    connections[index] = default(NetworkConnection);
                    break;
                case NetworkEvent.Type.Empty:
                    // Nothing to do. 
                    break;
                
                    
            }
        }
    }
}

public class ServerBehavior : MonoBehaviour
{
    public UdpCNetworkDriver m_Driver;
    public NativeList<NetworkConnection> m_Connections;
    private JobHandle ServerJobHandle;

    public ushort listenPort;
    // Start is called before the first frame update
    void Start()
    {
        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        if (m_Driver.Bind(NetworkEndPoint.Parse(IPAddress.Any.ToString(), listenPort)) != 0)
            Debug.Log("SERVER: Failed to bind to port 9000");
        else
            m_Driver.Listen();
    }
    
    public void OnDestroy()
    {
        // Make sure we run our jobs to completion before exiting.
        ServerJobHandle.Complete();
        m_Connections.Dispose();
        m_Driver.Dispose();
    }


    // Update is called once per frame
    void Update()
    {
        ServerJobHandle.Complete();
        
        var connectionJob = new ServerUpdateConnectionsJob
        {
            driver = m_Driver, 
            connections = m_Connections
        };
        
        var serverUpdateJob = new ServerUpdateJob
        {
            driver = m_Driver.ToConcurrent(),
            connections = m_Connections.AsDeferredJobArray()
        };
        
        ServerJobHandle = m_Driver.ScheduleUpdate();
        ServerJobHandle = connectionJob.Schedule(ServerJobHandle);
        ServerJobHandle = serverUpdateJob.Schedule(m_Connections, 1, ServerJobHandle);        
    }
}
