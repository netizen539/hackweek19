using System.Collections;
using System.Collections.Generic;
using MessageProtocol;
using ServerMessages;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;
using UdpCNetworkDriver = Unity.Networking.Transport.GenericNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket, 
    Unity.Networking.Transport.DefaultPipelineStageCollection>;


namespace ClientMessages
{
    public class ServerConnection
    {
        private static bool initalized = false;
        public static UdpCNetworkDriver driver;
        public static NetworkConnection serverConnection;

        public static void init(UdpCNetworkDriver _driver, NetworkConnection _serverConnection)
        {
            driver = _driver;
            serverConnection = _serverConnection;
            initalized = true;
        }

        public static void Send(ClientMessageBase msg) 
        {
            if (initalized)
                msg.SendTo(driver, serverConnection);
        }
    }

    public abstract class ClientMessageBase
    {
        public abstract void SendTo(UdpCNetworkDriver driver, NetworkConnection peer);

        public abstract void Recieve(NetworkConnection connection, DataStreamReader stream,
            EntityCommandBuffer.Concurrent commandBuffer, int connectionIndex, UdpCNetworkDriver.Concurrent driver);
    }

    public class ClientMessageHello : ClientMessageBase
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_HELLO; }
        }
        
        public override void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }
        }

        public override void Recieve(NetworkConnection connection, DataStreamReader stream,
            EntityCommandBuffer.Concurrent commandBuffer, int connectionIndex, UdpCNetworkDriver.Concurrent driver)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);
            Debug.Log("SERVER: A client says hello.");
            PrefabEntityLibrary.PlayerSpawnQueue.Enqueue(connectionIndex);
        }
    }

    public class ClientMessageGoodbye : ClientMessageBase
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_GOODBYE; }
        }
        
        public override void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }
        }

        public override void Recieve(NetworkConnection connection, DataStreamReader stream,
            EntityCommandBuffer.Concurrent commandBuffer, int connectionIndex, UdpCNetworkDriver.Concurrent driver)
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
 
         public override void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
         {
             using (var writer = new DataStreamWriter(4, Allocator.Temp))
             {
                 writer.Write(id);
                 writer.Write((uint)moveDirection);
                 peer.Send(driver, writer);
             }
         }
 
         public override void Recieve(NetworkConnection connection, DataStreamReader stream,
             EntityCommandBuffer.Concurrent commandBuffer, int connectionIndex, UdpCNetworkDriver.Concurrent driver)
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

        public override void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }
        }

        public override void Recieve(NetworkConnection connection, DataStreamReader stream,
            EntityCommandBuffer.Concurrent commandBuffer, int connectionIndex, UdpCNetworkDriver.Concurrent driver)
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

        public override void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }
        }

        public override void Recieve(NetworkConnection connection, DataStreamReader stream,
            EntityCommandBuffer.Concurrent commandBuffer, int connectionIndex, UdpCNetworkDriver.Concurrent driver)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);

            Debug.Log("SERVER: A client wants to use their shield.");
        }
    }
    
    public class ClientMessageMoveJoy : ClientMessageBase
    {
        public Vector2 moveVector;
        
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_MOVE_JOY; }
        }
 
        public override void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(8*3, Allocator.Temp))
            {
                writer.Write(id);
                writer.Write(moveVector.x);
                writer.Write(moveVector.y);
                Debug.Log("Sending joy:"+moveVector);
                peer.Send(driver, writer);
            }
        }
 
        public override void Recieve(NetworkConnection connection, DataStreamReader stream,
            EntityCommandBuffer.Concurrent commandBuffer, int connectionIndex, UdpCNetworkDriver.Concurrent driver)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);
            float x = stream.ReadFloat(ref readerCtx);
            float y = stream.ReadFloat(ref readerCtx);
 
            Debug.Log("SERVER: Client Sent Joy:"+x+","+y);
        }
    }

    public class ClientMessagePing : ClientMessageBase
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.CLIENT_PING; }
        }
        
        public override void SendTo(UdpCNetworkDriver driver, NetworkConnection peer)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                peer.Send(driver, writer);
            }

        }

        public override void Recieve(NetworkConnection connection, DataStreamReader stream,
            EntityCommandBuffer.Concurrent commandBuffer, int connectionIndex, UdpCNetworkDriver.Concurrent driver)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);
            Debug.Log("SERVER: Client Ping");
            
            ServerMessagePong pong = new ServerMessagePong();
            pong.SendTo(driver, connection);

        }
        
    }
}
