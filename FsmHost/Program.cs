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
        private static byte[] _buffer = new byte[8192];
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> _clientSockets = new List<Socket>();
        static Manager manager;
        public static bool isFirstConnection = true;
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
                received = 0;
                received = socket.EndReceive(AR);
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);
                Array.Clear(_buffer, 0, _buffer.Length);
                //Debug.WriteLine("Bytes received: " + received);

                receivedText = Encoding.ASCII.GetString(dataBuf);
                //Debug.WriteLine("Received Text: " + receivedText);
                //Array.Clear(_buffer, 0, _buffer.Length);
                if (receivedText != "")
                {

                    Manager.stringBuffer += receivedText;
                   manager.prepareProcessingReceivedData(socket);

                    if (isFirstConnection)
                    {
                        sendString("getAllData", socket);
                        isFirstConnection = false;
                    }
                }
                else
                {
                    Disconnect(socket);
                }



            }
            catch (SocketException sockEx)
            {
                try
                {
                    int i = _clientSockets.IndexOf(socket);
                    Disconnect(socket);
                }
                catch (Exception e)
                {

                }

                return;
            }



            
        }

        public static void continueReceiving(Socket socket)
        {
            if (socket.Connected)
            {

                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);

            }
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
                int index = _clientSockets.IndexOf(socket);
                Debug.WriteLine("Index is: " + index);

                try
                {
                    socket.Disconnect(true);
                    Debug.WriteLine("Anzahl User:" + manager.usernames.Count);
                    foreach (string s in manager.usernames)
                    {
                        Debug.WriteLine("Usernames in List: " + s);
                    }

                    //Debug.WriteLine(manager.usernames[index]);
                    string username = manager.usernames[index];
                    manager.usernames.RemoveAt(index);
                    _clientSockets.Remove(socket);
                    Console.WriteLine(manager.currentTimeStamp() + " User disconnected: " + username);
                    Console.WriteLine("Connected Clients: " + _clientSockets.Count);
                }
                catch (SocketException ex)
                {

                }


                return;
            }
            else
            {
                try
                {
                    int index = _clientSockets.IndexOf(socket);
                    string username = manager.usernames[index];
                    manager.usernames.RemoveAt(index);
                    _clientSockets.Remove(socket);
                    Console.WriteLine(manager.currentTimeStamp() + " User disconnected: " + username);
                    Console.WriteLine("Connected Clients: " + _clientSockets.Count);
                }
                catch (Exception e)
                {

                }

                return;
            }
        }
    }
}
