using System.Collections;
using System.Collections.Generic;
using MessageProtocol;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UdpCNetworkDriver = Unity.Networking.Transport.GenericNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket, 
    Unity.Networking.Transport.DefaultPipelineStageCollection>;

namespace ServerMessages
{
    public class ServerMessageHello
    {
        public static uint id
        {
            get { return (ushort) MessageIDs.SERVER_HELLO; }
        }

        public void SendTo(UdpCNetworkDriver.Concurrent driver, NetworkConnection client)
        {
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                writer.Write(id);
                driver.Send(NetworkPipeline.Null, client, writer);
            }
        }

        public void Recieve(DataStreamReader stream)
        {
            var readerCtx = default(DataStreamReader.Context);
            uint number = stream.ReadUInt(ref readerCtx);

            Debug.Log("CLIENT: Server says hello.");
        }
    }


}
