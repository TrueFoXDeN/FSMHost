using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private string username;
        public Manager()
        {
        }


        public void processReceivedData(string receivedData, Message e)
        {
           

            string[] splittedString = receivedData.Split('$');
            username = splittedString[0];
            splittedString = splittedString.Skip(1).ToArray();

            switch (splittedString[0])
            {
                case "con":
                    connectClient(splittedString, e);
                    break;
                case "dcn":
                    disconnectClient(splittedString);
                    break;
                case "ccl":
                    createColumn(splittedString, e);
                    break;
                case "rcl":
                    removeColumn(splittedString);
                    break;
                case "cfs":
                    createFlightstrip(splittedString.Skip(1).ToArray(), e);
                    break;
                case "rfs":
                    removeFlightstrip(splittedString);
                    break;
                case "edt":
                    editFlightstrip(splittedString);
                    break;
                case "mov":
                    moveFlightstrip(splittedString);
                    break;

                case "sic":
                    setIdCounter(splittedString);
                    break;
            }
        }


        private void connectClient(string[] data, Message e)
        {
            usernames.Add(data[1]);
            Console.WriteLine(currentTimeStamp() + " User connected: " + data[1]);
            sendAllData(e);
        }

        private void disconnectClient(string[] data)
        {

        }


        private void sendAllData(Message e)
        {
            foreach (Column c in columns)
            {
                e.ReplyLine("ccl$" + c.name + "$" + c.id.ToString());

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
            Program.server.BroadcastLine(username+"$"+data);
        }



        private void createColumn(string[] data, Message e)
        {
            columns.Add(new Column(data[1], ColumnIdCounter));
            Console.WriteLine(currentTimeStamp() + " Column created: " + data[1]);
            //if(Int32.Parse(data[2])> ColumnIdCounter)
            //{
            //    ColumnIdCounter = Int32.Parse(data[2]);
            //}
            e.ReplyLine("eid$c$" + data[2] + "$" + ColumnIdCounter.ToString());
            ColumnIdCounter++;
        }
        private void removeColumn(string[] data)
        {

        }

        private void createFlightstrip(string[] data, Message e)
        {
            Column c = columns[Int32.Parse(data[1])];
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

            //if (Int32.Parse(data[1]) > FlightstripIdCounter){
            //    FlightstripIdCounter = Int32.Parse(data[1]);
            //}

            e.ReplyLine("eid$f$" + data[0] + "$" + FlightstripIdCounter.ToString());
            Console.WriteLine(currentTimeStamp() + " Flightstrip created. ID: " + FlightstripIdCounter.ToString() + ", Column: " + c.name + ", Type: " + type);
            FlightstripIdCounter++;

            string toSend="cfs$";
            Array.ForEach(data, x => toSend += (x + "$"));
            BroadcastMessage(toSend);
        }

        private void removeFlightstrip(string[] data)
        {

        }

        private void moveFlightstrip(string[] data)
        {

        }

        private void editFlightstrip(string[] data)
        {

        }

        private void setIdCounter(string[] data)
        {
            if (data[1] == "f")
            {

            }


        }

        public string currentTimeStamp()
        {
            string timestamp = "[" + DateTime.Now.ToLongTimeString() + "]";
            return timestamp;
        }

       

    }
}
