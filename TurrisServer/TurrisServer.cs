using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace TurrisServer
{
    public class TurrisServer
    {
        public TurrisServerListener listener;
        public NetManager server;

        public TurrisServer()
        {
            listener = new TurrisServerListener(this);
            server = new NetManager(listener);
            server.Start(11000);
        }
    }
}
