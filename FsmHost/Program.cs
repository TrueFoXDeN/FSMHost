using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FsmHost
{
    class Program
    {

        static Manager manager;
        public static bool isFirstConnection = true;
        public static SimpleTcpServer server;
        public static List<TcpClient> clients = new List<TcpClient>();
        static void Main(string[] args)
        {
            manager = new Manager();
            SetupServer();
            Console.ReadLine();
        }


        private static void SetupServer()
        {
            char changePort = 'x';
            int port = 13000;
            string stringPort = string.Empty;
            Console.Title = "FlightStrip Manager Server";
            Console.WriteLine("Setup Server:");
            Console.WriteLine("Standard port is set to " + port + ".");


            while (!(changePort == 'y' || changePort == 'n'))
            {
                Console.Write("Do you want to use a custom port? (y/n): ");
                changePort = Console.ReadKey().KeyChar;

                if (changePort == 'y')
                {
                    Console.WriteLine();
                    Console.Write("Desired port: ");
                    stringPort = Console.ReadLine();
                    if (stringPort != "")
                    {
                        try
                        {
                            port = Int32.Parse(stringPort);
                            Console.WriteLine("Port set to " + port);
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("Input wasn't a number. Port set to 13000");
                        }

                    }
                }
                else if (changePort == 'n')
                {
                    Console.WriteLine();
                    Console.WriteLine("Port set to " + port);
                }
                else
                {
                    Console.WriteLine();
                }

            }




            Console.WriteLine("Setting up server...");
            try
            {
                server = new SimpleTcpServer().Start(port);
                server.Delimiter = 0x25;
                server.DelimiterDataReceived += dataReceived;
                server.ClientDisconnected += ClientDisconnected;
                server.ClientConnected += ClientConnected;
                Console.WriteLine();
                Console.WriteLine("Server is reachable under the following IP-addresses:");
                string hostName = Dns.GetHostName();
                for (int i = 0; i < Dns.GetHostEntry(hostName).AddressList.Length; i++)
                {
                    string ipAddress = Dns.GetHostEntry(hostName).AddressList[i].ToString();
                    if (!ipAddress.Contains(":"))
                    {
                        Console.WriteLine(ipAddress);
                    }
                }
                Console.WriteLine();
                Console.WriteLine("To list all commands, type: help");
                Console.WriteLine("Setup finished. Server started at " + DateTime.Now.ToLongTimeString());
                Console.WriteLine("---------------------------------------------");
                while (true)
                {
                    string line = Console.ReadLine();
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        string[] commands = line.Split(' ');
                        switch (commands[0])
                        {
                            case "help":
                                Console.WriteLine("kick         Enter kick followed by the username to force a disconnect on the specified user.");
                                Console.WriteLine("ban          Enter ban followed by the username to permanently ban a specified user.");
                                Console.WriteLine("banip        Enter ban followed by the username to permanently ban a specified user.\n" +
                                    "             This option blocks every account from connecting, coming from this ip");
                                Console.WriteLine("unban        Enter unban followed by the username to unban a specified user.");
                                Console.WriteLine("unbanip      Enter unbanip followed by the ip adress to unban a specified ip adresss.");
                                Console.WriteLine("usewhitelist Enter usewhitelist followed by true or false to enable or disable the whitelist.\n" +
                                    "             If this option is enabled, only usernames in the textfile \"whitelist.txt\" will be allowed to connect.");
                                break;
                            case "ban":
                                
                                if (commands.Length == 2)
                                {
                                    
                                    Console.WriteLine(Filemanager.blacklist(commands[1], ""));
                                }
                                else
                                {
                                    Console.WriteLine("Wrong number of arguments");
                                }

                                break;
                            case "banip":
                                break;
                            case "unban":
                                if (commands.Length == 2)
                                {
                                    Console.WriteLine(Filemanager.removeFromBlacklist(commands[1], ""));
                                }
                                else
                                {
                                    Console.WriteLine("Wrong number of arguments");
                                }
                                break;
                            case "unbanip":
                                break;

                            default:
                                Console.WriteLine("No valid command entered. Type help to list all commands.");
                                break;
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error occured:");
                Console.WriteLine(e);
            }
        }

        public static void dataReceived(object sender, Message e)
        {


            Debug.WriteLine(e.MessageString);
            manager.processReceivedData(e.MessageString, e);
        }

        private static void ClientConnected(object sender, TcpClient client)
        {
            clients.Add(client);
        }


        private static void ClientDisconnected(object sender, TcpClient client)
        {
            int index = clients.IndexOf(client);
            Console.WriteLine(manager.currentTimeStamp() + " User disconnected: " + manager.usernames[index]);
            manager.usernames.RemoveAt(index);
            clients.Remove(client);
        }


    }
}
