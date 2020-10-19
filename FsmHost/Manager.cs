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
            try
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
                        case "efs":
                            editFlightstripStatus(splittedString.Skip(1).ToArray());
                            break;
                        case "mov":
                            moveFlightstrip(splittedString.Skip(1).ToArray());
                            break;
                        case "gco":
                            sendClientOverview(e);
                            break;
                    }
                }
            }
            catch
            {
                Consolemanager.error("process data");
            }
        }



        private void connectClient(string[] data, Message e)
        {
            try
            {
                string username = data[1];
                Boolean isAlreadyConnected = usernames.Contains(username);

                usernames.Add(username);
                if (isAlreadyConnected)
                {
                    e.ReplyLine($"$kck${username}$3");
                    return;
                }


                int index = usernames.IndexOf(username);
                string ip = Program.clients[index].Client.RemoteEndPoint.ToString().Split(':')[0];


                if (Filemanager.isBanned(username) || Filemanager.isIpBanned(ip))
                {
                    e.ReplyLine($"$kck${username}$1");
                    return;
                }
                else if (useWhitelist && !Filemanager.isOnWhitelist(username))
                {
                    e.ReplyLine($"$kck${username}$2");
                    return;
                }
                else
                {
                    if (Program.isFirstConnection)
                    {
                        //Console.WriteLine("First connection, fetching Data...");
                        e.ReplyLine("$gad");
                        Program.isFirstConnection = false;
                    }
                    else
                    {
                        //Console.WriteLine("Not first connection.");
                        e.ReplyLine("$rad");
                        sendAllData(e);
                    }

                    Console.WriteLine(currentTimeStamp() + " User connected: " + username);
                }
            }
            catch
            {
                Consolemanager.error("connect client");
            }

        }


        private void sendAllData(Message e)
        {
            try
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
            catch
            {
                Consolemanager.error("send all data");
            }


        }

        public void BroadcastMessage(string data)
        {
            try
            {
                Program.server.BroadcastLine(username + "$" + data);
            }
            catch
            {
                Consolemanager.error("broadcast message");

            }
        }



        private void createColumn(string[] data, Message e)
        {
            try
            {
                columns.Add(new Column(data[1], ColumnIdCounter));
                Console.WriteLine(currentTimeStamp() + " Column created: " + data[1]);
                e.ReplyLine("$eid$c$" + data[2] + "$" + ColumnIdCounter.ToString());
                BroadcastMessage(columns[^1].ToString());
                ColumnIdCounter++;
            }
            catch
            {
                Consolemanager.error("create column");
            }

        }
        private void removeColumn(string[] data)
        {
            try
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
            catch
            {
                Consolemanager.error("remove column");
            }

        }

        private void createFlightstrip(string[] data, Message e)
        {
            try
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
            catch
            {
                Consolemanager.error("create flightstrip");
            }
        }

        private void removeFlightstrip(string[] data)
        {
            try
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
            catch
            {
                Consolemanager.error("remove flightstrip");
            }



        }

        private void moveFlightstrip(string[] data)
        {
            try
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
            catch
            {
                Consolemanager.error("move flightstrip");
            }




        }

        private void editFlightstrip(string[] data)
        {
            try
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
            catch
            {
                Consolemanager.error("edit flightstrip");
            }
        }

        private void editFlightstripStatus(string[] data)
        {
            try
            {
                //fsId, colId, direction, changedText
                string oldData = "";
                //Boolean hasChanged = false;
                foreach (Column c in columns)
                {
                    if (c.id == Int32.Parse(data[1]))
                    {
                        foreach (string[] s in c.Flightstrips)
                        {
                            if (s[0] == data[0])
                            {
                                oldData = s[11];
                                s[11] = data[3];
                                s[^1] = data[4];

                                if(oldData != data[3])
                                {
                                    Console.WriteLine(currentTimeStamp() + $" Flightstrip edited: \"{oldData}\" to \"{data[3]}\"");
                                    BroadcastMessage($"efs${data[0]}${data[1]}${data[2]}${data[4]}");
                                }

                                break;
                            }
                        }
                    }
                }
                
                    
                
            }
            catch
            {
                Consolemanager.error("edit flightstripStatus");
            }
        }

        private void sendClientOverview(Message e)
        {
            try
            {
                string msg = "";
                foreach (string user in usernames)
                {
                    msg += $"${user}";
                }
                e.ReplyLine($"$gco{msg}");
            }
            catch
            {
                Consolemanager.error("send client overview");
            }


        }


        public string currentTimeStamp()
        {

            string timestamp = "[" + DateTime.Now.ToLongTimeString() + "]";
            return timestamp;

        }



    }
}
