using System.Collections;
using System.Collections.Generic;
using MessageProtocol;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;
using UdpCNetworkDriver = Unity.Networking.Transport.GenericNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket, 
    Unity.Networking.Transport.DefaultPipelineStageCollection>;

namespace ServerMessages
{
    public abstract class ServerMessageBase
    {
        public abstract void SendTo(UdpCNetworkDriver.Concurrent driver, NetworkConnection peer);

        public abstract void Recieve(UdpCNetworkDriver driver, NetworkConnection connection, DataStreamReader stream);
    }
    
    public class ServerMessageHello : ServerMessageBase
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.SERVER_HELLO; }
        }

        public override void SendTo(UdpCNetworkDriver.Concurrent driver, NetworkConnection client)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                driver.Send(NetworkPipeline.Null, client, writer);
            }
        }

        public override void Recieve(UdpCNetworkDriver driver, NetworkConnection connection, DataStreamReader stream)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);
            Debug.Log("CLIENT: Server says hello.");
        }
    }

    public class ServerMessagePong : ServerMessageBase
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.SERVER_PONG; }
        }

        public override void SendTo(UdpCNetworkDriver.Concurrent driver, NetworkConnection client)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                driver.Send(NetworkPipeline.Null, client, writer);
            }
        }

        public override void Recieve(UdpCNetworkDriver driver, NetworkConnection connection, DataStreamReader stream)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);
            Debug.Log("CLIENT: Server PONG");
        }
    }
}
