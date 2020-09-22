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

        public Manager()
        {
        }

        public void prepareProcessingReceivedData(Socket socket)
        {
            try
            {
                //Debug.WriteLine("Preparing...");
                stringBuffer = stringBuffer.Trim();
                char[] charArray = stringBuffer.ToCharArray();
                //Debug.WriteLine("String Buffer: "+stringBuffer);

                int countDelimiter = 0;
                foreach (char c in charArray)
                {
                    //Debug.Write(c);
                    if (c.Equals('%'))
                    {
                        countDelimiter++;
                    }
                }
                
                //Debug.WriteLine("Packets found: " + countDelimiter);
                string[] splittedString = stringBuffer.Split('%');
                for (int i = 0; i < countDelimiter; i++)
                {
                    //Debug.WriteLine("RawMessage: " + splittedString[i]);
                    stringBuffer = stringBuffer.Remove(0, splittedString[i].Length + 1);
                    
                    processReceivedData(splittedString[i]);
                }
            }
            catch
            {
                Debug.WriteLine("--------------------------------");

                Debug.WriteLine(new String(stringBuffer));
                Debug.WriteLine("Length:" + stringBuffer.Length);
            }


        }



        public void processReceivedData(string receivedData)
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
                    createColumn(splittedString);
                    break;
                case "rcl":
                    removeColumn(splittedString);
                    break;
                case "cfs":
                    createFlightstrip(splittedString);
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

        private void createColumn(string[] data)
        {
            columns.Add(new Column(data[1]));
            Console.WriteLine(currentTimeStamp() + " Column created: " + data[1]);
        }
        private void removeColumn(string[] data)
        {

        }

        private void createFlightstrip(string[] data)
        {
            Console.WriteLine("");
            //Console.Write("Flightstrip created: ");
            foreach(string s in data)
            {
                Console.Write(s +";");
            }
            Console.WriteLine("");

            //columns[Int32.Parse(data[1])].Flightstrips.Add(new string[] { });
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

        public string currentTimeStamp()
        {
            string timestamp = "[" + DateTime.Now.ToLongTimeString() + "]";
            return timestamp;
        }

    }
}
