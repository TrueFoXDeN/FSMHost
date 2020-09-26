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

        public static Manager manager;
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
                                Consolemanager.help();
                                break;
                            case "kick":
                                Consolemanager.kick(commands, 0);
                                break;
                            case "kickall":
                                Consolemanager.kickall();
                                break;
                            case "ban":
                                Consolemanager.ban(commands);
                                break;
                            case "banip":
                                Consolemanager.banip(commands);
                                break;
                            case "unban":
                                Consolemanager.unban(commands);
                                break;
                            case "usewl":
                                Consolemanager.usewl(commands);
                                break;
                            case "addtowl":
                                Consolemanager.addtowl(commands);
                                break;
                            case "removefromwl":
                                Consolemanager.removefromwl(commands);
                                break;
                            case "recoverfrom":
                                Consolemanager.recoverfrom(commands);
                                break;
                            case "reset":
                                Consolemanager.reset();
                                break;
                            default:
                                Consolemanager.notvalid();
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
