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
        private static byte[] _buffer = new byte[1024];
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> _clientSockets = new List<Socket>();
        static Manager manager;
        static void Main(string[] args)
        {
            manager = new Manager();
            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            string changePort = string.Empty;
            int port = 13000;
            string stringPort = string.Empty;
            Console.Title = "FlightStrip Manager Server";
            Console.WriteLine("Setup Server:");
            Console.WriteLine("Standard port is set to " + port + ".");
            Console.Write("Do you want to use a custom port? (y/n): ");
            changePort = Console.ReadLine();
            if (changePort.ToLower() == "y")
            {
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
            else
            {
                Console.WriteLine("Port set to " + port);
            }


            Console.WriteLine("Setting up server...");
            try
            {
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                _serverSocket.Listen(25);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

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
                Console.WriteLine("Setup finished. Server started at " + DateTime.Now.ToLongTimeString());
                Console.WriteLine("---------------------------------------------");

            }
            catch (SocketException e)
            {
                Console.WriteLine("Error occured:");
                Console.WriteLine(e);
            }
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = _serverSocket.EndAccept(AR);
            _clientSockets.Add(socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            //Console.WriteLine("Received Something...");
            Socket socket = (Socket)AR.AsyncState;
            int received;
            string receivedText = "";
            try
            {
                received = socket.EndReceive(AR);
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);
                receivedText = Encoding.ASCII.GetString(dataBuf);

            }
            catch (SocketException sockEx)
            {
                Disconnect(socket);
                return;
            }

            if (receivedText != "")
            {
                manager.processReceivedData(receivedText);
                sendString("Du bist connected", socket);

            }
            else
            {
                Disconnect(socket);
            }

            if (socket.Connected)
            {
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);

            }


            //string response = string.Empty;

            //if (text.ToLower() == "get time")
            //{
            //    response = DateTime.Now.ToLongTimeString();
            //}

            //byte[] data = Encoding.ASCII.GetBytes(response);

            //foreach (Socket s in _clientSockets)
            //{
            //    if (s != socket)
            //    {
            //        s.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), s);

            //    }
            //    s.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), s);
            //}
        }


        public static void sendString(string dataToSend, Socket s)
        {
            byte[] data = Encoding.ASCII.GetBytes(dataToSend);
            s.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), s);
            s.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), s);
        }

        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);

        }

        private static void Disconnect(Socket socket)
        {

            if (socket.Connected)
            {
                try
                {
                    socket.Disconnect(true);
                }catch(SocketException ex)
                {

                }
                

                int index = _clientSockets.IndexOf(socket);
                string username = manager.usernames[index];
                manager.usernames.RemoveAt(index);
                _clientSockets.Remove(socket);
                Console.WriteLine(manager.currentTimeStamp() + " " + username + " disconnected...");
                Console.WriteLine("Connected Clients: " + _clientSockets.Count);
                return;
            }
            else
            {
                int index = _clientSockets.IndexOf(socket);
                string username = manager.usernames[index];
                manager.usernames.RemoveAt(index);
                _clientSockets.Remove(socket);
                Console.WriteLine(username + " disconnected...");
                Console.WriteLine("Connected Clients: " + _clientSockets.Count);
                return;
            }
        }



        private static void DisconnectCallback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;

            //socket.EndDisconnect(result);
            //socket.Close();
            Console.WriteLine("Client disconnected");
            Console.WriteLine("Connected Clients: " + _clientSockets.Count);
            //_serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

        }
    }
}
