using System.Collections;
using System.Collections.Generic;
using MessageProtocol;
using ServerMessages;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UdpCNetworkDriver = Unity.Networking.Transport.GenericNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket, 
    Unity.Networking.Transport.DefaultPipelineStageCollection>;


namespace ClientMessages
{
    
    public class ClientMessageHello
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_HELLO; }
        }
        
        public void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }
        }

        public void Recieve(DataStreamReader stream)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);
            Debug.Log("SERVER: A client says hello.");
            
            ServerMessageHello hello = new ServerMessageHello();
        }
    }

    public class ClientMessageGoodbye
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_GOODBYE; }
        }
        
        public static void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }
        }

        public static void Recieve(DataStreamReader stream)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);

            Debug.Log("SERVER: A client says goodbye.");            
        }

    }

    public class ClientMessageMove
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_MOVE; }
        }

        public static void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }
        }

        public static void Recieve(DataStreamReader stream)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);

            Debug.Log("SERVER: A client wants to move.");
        }
    }
}
