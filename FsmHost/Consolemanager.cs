using System;
using System.Collections.Generic;
using System.Text;

namespace FsmHost
{
    class Consolemanager
    {
        public static void help()
        {
            Console.WriteLine("kick         Enter kick followed by the username to force a disconnect on the specified user.");
            Console.WriteLine("ban          Enter ban followed by the username to permanently ban a specified user.");
            Console.WriteLine("banip        Enter banip followed by the username to permanently ban a specified user including its ip.\n" +
                "             This option blocks every account from connecting, coming from this ip");
            Console.WriteLine("unban        Enter unban followed by the username to unban a specified user.");
            Console.WriteLine("unbanip      Enter unbanip followed by the ip adress to unban a specified ip adresss.");
            Console.WriteLine("usewl        Enter usewl followed by true or false to enable or disable the whitelist.\n" +
                "             If this option is enabled, only usernames in the textfile \"whitelist.txt\" will be allowed to connect.");
            Console.WriteLine("addtowl      Enter addtowl followed by the username to add a user to the whitelist.");
            Console.WriteLine("removefromwl Enter removefromwl followed by the username to remove a user from the whitelist.");

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

        }
        public static void usewl(string[] s)
        {

        }
        public static void addtowl(string[] s)
        {

        }
        public static void removefromwl(string[] s)
        {

        }
        public static void notvalid()
        {
            Console.WriteLine("No valid command entered. Type help to list all commands.");
        }
    }
}
