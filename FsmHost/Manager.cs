using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FsmHost
{
    public class Manager
    {
        public  List<string> usernames = new List<string>();

        public Manager()
        {
        }

        public void processReceivedData(string receivedData)
        {
            string[] splittedString = receivedData.Split(';');

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
            Console.WriteLine(currentTimeStamp()+ " User connected: " + data[1]);
        }

        private void disconnectClient(string[] data)
        {

        }

        private void createColumn(string[] data)
        {

        }
        private void removeColumn(string[] data)
        {

        }

        private void createFlightstrip(string[] data)
        {

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
