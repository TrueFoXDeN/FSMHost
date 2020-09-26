using System;
using System.Collections.Generic;
using System.Text;

namespace FsmHost
{
    class Consolemanager
    {
        public static void help()
        {
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("kick         Enter kick followed by the username to force a disconnect on the specified user.");
            Console.WriteLine("kickall      Enter kickall to force a disconnect for every user.");
            Console.WriteLine("ban          Enter ban followed by the username to permanently ban the specified user.");
            Console.WriteLine("banip        Enter banip followed by the username to permanently ban the specified user including its ip.\n" +
                "             This option blocks every account from connecting, coming from this ip");
            Console.WriteLine("unban        Enter unban followed by the username to unban the specified user.");
            Console.WriteLine("usewl        Enter usewl followed by true or false to enable or disable the whitelist.\n" +
                "             If this option is enabled, only usernames in the textfile \"whitelist.txt\" will be allowed to connect.");
            Console.WriteLine("addtowl      Enter addtowl followed by the username to add a user to the whitelist.");
            Console.WriteLine("removefromwl Enter removefromwl followed by the username to remove a user from the whitelist.");
            Console.WriteLine("recoverfrom  Enter recoverfrom followed by the username to clear all current data and use the data from a client.");
            Console.WriteLine("reset        Enter reset to clear all current data.");
            Console.WriteLine("---------------------------------------------");

        }
        public static void kick(string[] s)
        {
            if (s.Length == 2)
            {
                int index = Program.manager.usernames.IndexOf(s[1]);
                if(index != -1)
                {

                    //Program.clients[index].Dispose();
                    Program.manager.username = s[1];
                    Program.manager.BroadcastMessage($"kck${s[1]}");
                    Console.WriteLine($"{s[1]} has been kicked from the server.");
                }
                else
                {
                    Console.WriteLine($"User {s[1]} was not found.");
                }
                
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void kickall()
        {
            Console.WriteLine(Filemanager.kickall());
        }
        public static void ban(string[] s)
        {
            if (s.Length == 2)
            {

                Console.WriteLine(Filemanager.blacklist(s[1], ""));
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void banip(string[] s)
        {
            if (s.Length == 2)
            {

                Console.WriteLine(Filemanager.blacklist(s[1], "127.0.0.1"));
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void unban(string[] s)
        {
            if (s.Length == 2)
            {
                Console.WriteLine(Filemanager.removeFromBlacklist(s[1], ""));
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void unbanip(string[] s)
        {
            if (s.Length == 2)
            {
                Console.WriteLine(Filemanager.removeFromBlacklist(s[1], "127.0.0.1"));
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void usewl(string[] s)
        {
            if (s.Length == 2)
            {
                Console.WriteLine(Filemanager.toggleWhitelist((Boolean.Parse(s[1]))));
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void addtowl(string[] s)
        {
            if (s.Length == 2)
            {
                Console.WriteLine(Filemanager.addtowl(s[1]));
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void removefromwl(string[] s)
        {
            if (s.Length == 2)
            {
                Console.WriteLine(Filemanager.removefromwl(s[1]));
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void recoverfrom(string[] s)
        {
            if (s.Length == 2)
            {
                Console.WriteLine(Filemanager.recoverfrom(s[1]));
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void reset()
        {
            Console.WriteLine(Filemanager.reset()); ;
        }
        public static void notvalid()
        {
            Console.WriteLine("No valid command entered. Type help to list all commands.");
        }
    }
}
