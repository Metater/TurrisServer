using BitManipulation;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TurrisServer
{
    public class TurrisServerListener : INetEventListener
    {
        private TurrisServer turrisServer;

        private int nextIndex = 0;
        private Dictionary<NetPeer, int> players = new Dictionary<NetPeer, int>();

        public TurrisServerListener(TurrisServer turrisServer)
        {
            this.turrisServer = turrisServer;
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            if (turrisServer.server.ConnectedPeersCount < 100)
            {
                NetPeer client = request.Accept();
                Console.WriteLine(request.Data.GetString());
            }
            else
                request.Reject();
        }
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {

        }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {

        }
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {

        }
        public void OnPeerConnected(NetPeer peer)
        {

        }
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {

        }
    }
}
