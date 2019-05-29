using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WaTalkServer
{
    static class Program
    {
        private static bool loop = true;
        static List<Socket> ClientList = new List<Socket>();
        static Socket client;
        static IPEndPoint ipep;
        static string message = null;
        static string clientIP = null;
        static void Main(string[] args)
        {
            Task.Run(ServerWork);

            while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;

            loop = false;
        }

        static void ServerWork()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipep = new IPEndPoint(IPAddress.Any, 12345);
            

            client.Bind(ipep);
            client.Listen(10);

            Task.Run(connection);

            Console.WriteLine("Waiting");
            while (loop)
            {
                using (Socket clientCheck = client.Accept())
                {
                    clientIP = clientCheck.RemoteEndPoint.ToString();

                    using (var stream = new NetworkStream(clientCheck))
                    using (var reader = new StreamReader(stream))
                    {
                        message = "[" + clientIP + "]'s message : " + reader.ReadLine();
                        Console.WriteLine(message);
                    }
                }

                using (var delay = Task.Delay(100))
                    delay.Wait();
            }
        }
        /*
        static void IPManager()
        {
            Socket tempClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint tempIpep = new IPEndPoint(IPAddress.Any, 12345);

            tempClient.Bind(ipep);
            tempClient.Listen(10);

            Console.WriteLine("Waiting");
            while (loop)
            {
                using (Socket clientChekc = tempClient.Accept())
                {
                    Console.WriteLine(client.RemoteEndPoint.ToString());

                    using (var stream = new NetworkStream(client))
                    using (var reader = new StreamReader(stream))
                    {
                        Console.WriteLine(reader.ReadLine());
                    }



                    client.Disconnect(false);
                }

                using (var delay = Task.Delay(100))
                    delay.Wait();
            }
        }*/

        static void connection()
        {
            while (loop)
            {
                using (Socket clientCheck = client.Accept())
                {
                    clientIP = clientCheck.RemoteEndPoint.ToString();

                    bool check = true;
                    foreach(Socket temp in ClientList)
                    {
                        if (temp.RemoteEndPoint.ToString().Equals(clientIP))
                        {
                            check = false;
                            break;
                        }
                    }
                    if(check)
                    {
                        ClientList.Add(clientCheck);
                    }


                    using (var stream = new NetworkStream(clientCheck))
                    using (var reader = new StreamReader(stream))
                    {
                        message = "[" + clientIP + "]'s message : " + reader.ReadLine();
                        Console.WriteLine(message);
                    }
                }

                foreach(Socket eachClient in ClientList)
                {
                    client.Connect(ipep);
                    using (var stream = new NetworkStream(client))
                    using (var writer = new StreamWriter(stream))
                        writer.WriteLine(message);
                }
                using (var delay = Task.Delay(100))
                    delay.Wait();
            }
        }
    }
}
