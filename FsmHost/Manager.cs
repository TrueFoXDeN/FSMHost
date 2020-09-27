using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FsmHost
{
    public class Manager
    {
        public List<string> usernames = new List<string>();
        public List<Column> columns = new List<Column>();
        public static string stringBuffer;
        public static int FlightstripIdCounter = 0;
        public static int ColumnIdCounter = 0;
        public string username;
        public static Boolean useWhitelist = false;
        public Manager()
        {
        }


        public void processReceivedData(string receivedData, Message e)
        {

            string[] splittedString = receivedData.Split('$');
            if (splittedString.Length > 2)
            {
                username = splittedString[0];
                splittedString = splittedString.Skip(1).ToArray();

                switch (splittedString[0])
                {
                    case "con":
                        connectClient(splittedString, e);
                        break;
                    case "ccl":
                        createColumn(splittedString, e);
                        break;
                    case "rcl":
                        removeColumn(splittedString.Skip(1).ToArray());
                        break;
                    case "cfs":
                        createFlightstrip(splittedString.Skip(1).ToArray(), e);
                        break;
                    case "rfs":
                        removeFlightstrip(splittedString.Skip(1).ToArray());
                        break;
                    case "edt":
                        editFlightstrip(splittedString.Skip(1).ToArray());
                        break;
                    case "mov":
                        moveFlightstrip(splittedString.Skip(1).ToArray());
                        break;
                }
            }


        }


        private void connectClient(string[] data, Message e)
        {

            string ip = Program.clients[0].Client.RemoteEndPoint.ToString().Split(':')[0];
            string username = data[1];

            usernames.Add(data[1]);
            if (Filemanager.isBanned(data[1]) || Filemanager.isIpBanned(ip))
            {
                e.ReplyLine($"$kck${username}$1");
            }
            else if (useWhitelist && !Filemanager.isOnWhitelist(data[1]))
            {

                e.ReplyLine($"$kck${username}$2");

            }
            else
            {
                if (Program.isFirstConnection)
                {
                    Console.WriteLine("First connection, fetching Data...");
                    e.ReplyLine("$gad");
                    Program.isFirstConnection = false;
                }
                else
                {
                    Console.WriteLine("Not first connection.");
                    e.ReplyLine("$rad");
                    sendAllData(e);
                }

                Console.WriteLine(currentTimeStamp() + " User connected: " + data[1]);
            }

        }


        private void sendAllData(Message e)
        {

            foreach (Column c in columns)
            {

                e.ReplyLine("$ccl$" + c.name + "$" + c.id.ToString());


                foreach (string[] s in c.Flightstrips)
                {
                    string toSend = "";
                    Array.ForEach(s, x => toSend += (x + "$"));

                    e.ReplyLine("$cfs$" + toSend);
                }
            }


        }

        public void BroadcastMessage(string data)
        {
            Program.server.BroadcastLine(username + "$" + data);
        }



        private void createColumn(string[] data, Message e)
        {
            columns.Add(new Column(data[1], ColumnIdCounter));
            Console.WriteLine(currentTimeStamp() + " Column created: " + data[1]);
            e.ReplyLine("$eid$c$" + data[2] + "$" + ColumnIdCounter.ToString());
            BroadcastMessage(columns[^1].ToString());
            ColumnIdCounter++;
        }
        private void removeColumn(string[] data)
        {
            int delete = -1;
            for (int i = 0; i < columns.Count; i++)
            {
                if (Int32.Parse(data[0]) == columns[i].id)
                {
                    delete = i;
                }
            }
            if (delete >= 0)
            {
                Console.WriteLine(currentTimeStamp() + $" Column {columns[delete]} removed");
                columns.RemoveAt(delete);
                BroadcastMessage($"rcl${data[0]}");
            }

        }

        private void createFlightstrip(string[] data, Message e)
        {
            Column c = null;
            foreach (Column col in columns)
            {
                if (col.id == Int32.Parse(data[1]))
                {
                    c = col;
                }
            }
            if (c != null)
            {
                string originalID = data[0];
                data[0] = FlightstripIdCounter.ToString();

                c.Flightstrips.Add(data);
                string rawtype = data[2];
                string type = "";
                switch (rawtype)
                {
                    case "I":
                        type = "Inbound";
                        break;
                    case "O":
                        type = "Outbound";
                        break;
                    case "V":
                        type = "VFR";
                        break;
                }

                e.ReplyLine("$eid$f$" + originalID + "$" + FlightstripIdCounter.ToString());
                Console.WriteLine(currentTimeStamp() + " Flightstrip created. ID: " + FlightstripIdCounter.ToString() + ", Column: " + c.name + ", Type: " + type);
                FlightstripIdCounter++;

                string toSend = "cfs$";
                Array.ForEach(data, x => toSend += (x + "$"));
                BroadcastMessage(toSend);
            }


        }

        private void removeFlightstrip(string[] data)
        {
            Column col = null;
            string[] flightstrip = null;

            foreach (Column c in columns)
            {
                if (c.id == Int32.Parse(data[1]))
                {
                    foreach (string[] s in c.Flightstrips)
                    {
                        if (s[0] == data[0])
                        {
                            Console.WriteLine(currentTimeStamp() + $" Flightstrip removed. ID: {s[0]}");

                            col = c;
                            flightstrip = s;

                        }
                    }
                }
            }
            if (col != null && flightstrip != null)
            {
                col.Flightstrips.Remove(flightstrip);
                BroadcastMessage($"rfs${data[0]}${data[1]}");
            }

        }

        private void moveFlightstrip(string[] data)
        {
            //fsID;start;dest
            int start = -1, dest = -1, fsId = -1;
            for (int i = 0; i < columns.Count; i++)
            {
                if (columns[i].id == Int32.Parse(data[1]))
                {

                    start = i;

                    for (int j = 0; j < columns[i].Flightstrips.Count; j++)
                    {
                        if (columns[i].Flightstrips[j][0] == data[0])
                        {
                            fsId = j;
                        }
                    }
                }
                if (columns[i].id == Int32.Parse(data[2]))
                {
                    dest = i;
                }

            }

            columns[start].Flightstrips[fsId][1] = columns[dest].id.ToString();

            if (start >= 0 && dest >= 0 && fsId >= 0)
            {
                string[] fs = columns[start].Flightstrips[fsId];
                columns[start].Flightstrips.RemoveAt(fsId);
                columns[dest].Flightstrips.Add(fs);
                Console.WriteLine(currentTimeStamp() + $" Flightstrip moved from {columns[start].name} to {columns[dest].name}");
                BroadcastMessage($"mov${data[0]}${data[1]}${data[2]}");
            }


        }

        private void editFlightstrip(string[] data)
        {
            //fsId, colId, textboxIndex, changedText
            string oldData = "";
            Boolean hasChanged = false;
            foreach (Column c in columns)
            {
                if (c.id == Int32.Parse(data[1]))
                {
                    foreach (string[] s in c.Flightstrips)
                    {
                        if (s[0] == data[0])
                        {
                            if (s[Int32.Parse(data[2]) + 3] != data[3])
                            {
                                oldData = s[Int32.Parse(data[2]) + 3];
                                s[Int32.Parse(data[2]) + 3] = data[3];
                                hasChanged = true;
                            }


                        }
                    }
                }
            }
            if (hasChanged)
            {
                Console.WriteLine(currentTimeStamp() + $" Flightstrip edited: \"{oldData}\" to \"{data[3]}\"");
                BroadcastMessage($"edt${data[0]}${data[2]}${data[3]}");
            }

        }


        public string currentTimeStamp()
        {
            string timestamp = "[" + DateTime.Now.ToLongTimeString() + "]";
            return timestamp;
        }



    }
}
