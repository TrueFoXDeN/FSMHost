using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Manager()
        {
        }


        public void processReceivedData(string receivedData, Message e)
        {
            string[] splittedString = receivedData.Split('$');

            switch (splittedString[0])
            {
                case "con":
                    connectClient(splittedString);
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
                    createFlightstrip(splittedString, e);
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


        private void connectClient(string[] data)
        {
            usernames.Add(data[1]);
            Console.WriteLine(currentTimeStamp() + " User connected: " + data[1]);
          
        }

        private void disconnectClient(string[] data)
        {

        }

        private void createColumn(string[] data, Message e)
        {
            columns.Add(new Column(data[1]));
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
            Column c = columns[Int32.Parse(data[2])];
            c.Flightstrips.Add(data);
            string rawtype = data[3];
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

            e.ReplyLine("eid$f$" + data[1] + "$" + FlightstripIdCounter.ToString());
            Console.WriteLine(currentTimeStamp() + " Flightstrip created. ID: " + FlightstripIdCounter.ToString() + ", Column: " + c.name + ", Type: " + type);
            FlightstripIdCounter++;
            
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
