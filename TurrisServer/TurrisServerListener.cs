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
                request.AcceptIfKey("Turris");
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
            BitReader br = new BitReader(reader.GetRemainingBytes());
            switch (br.GetInt(2))
            {
                case 0: // Player Update
                    ulong update = br.GetULong(58);
                    BitWriter bw = new BitWriter();
                    bw.Put(1, 2);
                    bw.Put(players[peer], 8);
                    bw.Put(update, 58);
                    byte[] data = bw.Assemble();
                    Console.WriteLine($"Got player update");
                    foreach (KeyValuePair<NetPeer, int> player in players)
                    {
                        if (player.Key == peer) continue;
                        player.Key.Send(data, DeliveryMethod.Sequenced);
                        Console.WriteLine($"Sent data to: {player.Value}");
                    }
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {

        }
        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine($"Client connected, id: " + nextIndex);
            BitWriter bw = new BitWriter();
            bw.Put(0, 2);
            bw.Put((byte)nextIndex);
            byte[] data = bw.Assemble();
            string output = "";
            foreach (byte b in data)
            {
                output += $"{b}, ";
            }
            Console.WriteLine("Sent: " + output);
            foreach (KeyValuePair<NetPeer, int> player in players)
            {
                BitWriter we = new BitWriter();
                we.Put(0, 2);
                we.Put((byte)player.Value);
                peer.Send(we.Assemble(), DeliveryMethod.ReliableOrdered);
                player.Key.Send(data, DeliveryMethod.ReliableOrdered);
            }
            players.Add(peer, nextIndex);
            nextIndex++;
        }
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            players.Remove(peer);
        }
    }
}
