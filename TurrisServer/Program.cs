using System;
using System.Threading;

namespace TurrisServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TurrisServer turrisServer = new TurrisServer();
            while (true)
            {
                turrisServer.server.PollEvents();
                Thread.Sleep(1);
            }
        }
    }
}
