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
    public class ServerConnection
    {
        public static UdpCNetworkDriver driver;
        public static NetworkConnection serverConnection;

        public static void init(UdpCNetworkDriver _driver, NetworkConnection _serverConnection)
        {
            driver = _driver;
            serverConnection = _serverConnection;
        }

        public static void Send(ClientMessageBase msg) 
        {
            msg.SendTo(driver, serverConnection);
        }
    }

    public abstract class ClientMessageBase
    {
        public virtual void SendTo(UdpCNetworkDriver driver, NetworkConnection peer) {}

        public virtual void Recieve(DataStreamReader stream) {}
    }

    public class ClientMessageHello : ClientMessageBase
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_HELLO; }
        }
        
        public new void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }
        }

        public new void Recieve(DataStreamReader stream)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);
            Debug.Log("SERVER: A client says hello.");
            
            ServerMessageHello hello = new ServerMessageHello();
        }
    }

    public class ClientMessageGoodbye : ClientMessageBase
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_GOODBYE; }
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

            Debug.Log("SERVER: A client says goodbye.");            
        }

    }

    public class ClientMessageMove : ClientMessageBase
    {
        public MovementDirection moveDirection;
        
         public static uint id
         {
             get { return (ushort) MessageIDs.CLIENT_MOVE; }
         }
 
         public void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
         {
             using (var writer = new DataStreamWriter(4, Allocator.Temp))
             {
                 writer.Write(id);
                 writer.Write((uint)moveDirection);
                 peer.Send(driver, writer);
             }
         }
 
         public void Recieve(DataStreamReader stream)
         {
             var readerCtx = default(DataStreamReader.Context);
             uint number = stream.ReadUInt(ref readerCtx);
 
             Debug.Log("SERVER: A client wants to move.");
         }
    }
    
    public class ClientMessageFire : ClientMessageBase
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_FIRE; }
        }

        public new void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }
        }

        public new void Recieve(DataStreamReader stream)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);

            Debug.Log("SERVER: A client wants to fire their weapon.");
        }
    }
    
    public class ClientMessageShield : ClientMessageBase
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_SHIELD; }
        }

        public new void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }
        }

        public new void Recieve(DataStreamReader stream)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);

            Debug.Log("SERVER: A client wants to use their shield.");
        }
    }
}
