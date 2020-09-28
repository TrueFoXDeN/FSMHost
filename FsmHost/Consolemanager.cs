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
            Console.WriteLine("msg [message]            Sends a message to every connected user.");
            Console.WriteLine("msguser [user] [message] Sends a message to the specified user.");
            Console.WriteLine("listuser                 Lists all connected users.");
            Console.WriteLine("kick [user]              Kicks a user from the server.");
            Console.WriteLine("kickall                  Kicks every user on the server.");
            Console.WriteLine("ban [user]               Permanently bans the specified user.");
            Console.WriteLine("banip [user]             Permanently bans the specified user including its ip adress.");
            Console.WriteLine("unban [user]             Unbans the specified user.");
            Console.WriteLine("listbanned               Lists all banned users.");
            Console.WriteLine("usewl [true/false]       Enables or disables the whitelist.");
            Console.WriteLine("addtowl [user]           Add the specified user to the whitelist.");
            Console.WriteLine("removefromwl [user]      Removes a user from the whitelist.");
            Console.WriteLine("listwl                   Lists all whitelisted users.");
            Console.WriteLine("recoverfrom [user]       Clears all current data and recovers the data from a client.");
            Console.WriteLine("reset                    Clears all current data.");
            Console.WriteLine("---------------------------------------------");

        }

        public static void msg(string[] s)
        {
            if (s.Length >= 2)
            {
                Program.manager.username = "";
                string msg = "";
                for (int i = 1; i < s.Length; i++)
                {
                    msg += s[i] + " ";
                }
                Program.manager.BroadcastMessage($"msg${msg}");
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }

        }

        public static void msguser(string[] s)
        {
            if (s.Length >= 3)
            {
                if (Program.manager.usernames.Contains(s[1]))
                {
                    Program.manager.username = "";
                    string msg = "";
                    for (int i = 2; i < s.Length; i++)
                    {
                        msg += s[i] + " ";
                    }
                    Program.manager.BroadcastMessage($"msguser${s[1]}${msg}");
                }
                else
                {
                    Console.WriteLine($"User {s[1]} is not connected to the server.");
                }

            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }

        }

        public static void listuser()
        {
            if (Program.manager.usernames == null || Program.manager.usernames.Count == 0)
            {
                Console.WriteLine("No users are connected.");
            }
            else
            {
                foreach (string user in Program.manager.usernames)
                {
                    Console.WriteLine(user);
                }
            }
        }

        public static void listbanned()
        {
            if (Filemanager.bannedUsers() == null)
            {
                Console.WriteLine("No users were banned.");
            }
            else
            {
                foreach (string s in Filemanager.bannedUsers())
                {
                    Console.WriteLine(s.Replace(";", ""));
                }
            }

        }

        public static void listwl()
        {
            if (Filemanager.whitelistedUsers() == null)
            {
                Console.WriteLine("No users were whitelisted.");
            }
            else
            {
                foreach (string s in Filemanager.whitelistedUsers())
                {
                    Console.WriteLine(s);
                }
            }
        }
        public static void kick(string[] s, int banned)
        {
            if (s.Length == 2)
            {
                int index = Program.manager.usernames.IndexOf(s[1]);
                if (index != -1)
                {

                    //Program.clients[index].Dispose();
                    Program.manager.username = s[1];
                    Program.manager.BroadcastMessage($"kck${s[1]}${banned}");
                    if (banned != 1)
                    {
                        Console.WriteLine($"{s[1]} has been kicked from the server.");
                    }

                }
                else
                {
                    if (banned != 1)
                    {
                        Console.WriteLine($"User {s[1]} was not found.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void kickall()
        {
            if (Program.manager.usernames.Count == 0)
            {
                Console.WriteLine("No users connected to kick.");
            }
            for (int i = 0; i < Program.manager.usernames.Count; i++)
            {
                Program.manager.username = Program.manager.usernames[i];
                Program.manager.BroadcastMessage($"kck${Program.manager.usernames[i]}");
                Console.WriteLine($"{Program.manager.usernames[i]} has been kicked from the server.");
            }

        }


        public static void ban(string[] s)
        {
            if (s.Length == 2)
            {
                Console.WriteLine(Filemanager.blacklist(s[1], ""));
                kick(s, 1);
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
                kick(s, 1);
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
                //Console.WriteLine(Filemanager.recoverfrom(s[1]));
                if (Program.manager.usernames.Contains(s[1]))
                {
                    Program.manager.username = "";
                    Manager.ColumnIdCounter = 0;
                    Manager.FlightstripIdCounter = 0;
                    Program.manager.columns.Clear();
                    Program.manager.BroadcastMessage($"gad${s[1]}");
                    Console.WriteLine($"Recovering data from {s[1]}...");
                }
                else
                {
                    Console.WriteLine($"User {s[1]} is not connected. Data recovery failed.");
                }

            }
            else
            {
                Console.WriteLine("Wrong number of arguments");
            }
        }
        public static void reset()
        {
            //Console.WriteLine(Filemanager.reset()); ;
            Manager.ColumnIdCounter = 0;
            Manager.FlightstripIdCounter = 0;
            Program.manager.columns.Clear();
            Console.WriteLine("Data has been cleared.");
            Program.manager.username = "";
            Program.manager.BroadcastMessage("rad");
        }
        public static void error(string s)
        {
            Console.WriteLine($"#############################################\nError occured during [{s}]");
            if (Program.manager.usernames.Count == 0)
            {
                Console.WriteLine("Server restart is recommended.");
                Console.WriteLine("Data recovery is not available.");
            }
            else
            {
                Console.WriteLine("Server restart is recommended.");
                Console.WriteLine("Enter: recoverfrom [user] to restore the data.");
            }
            Console.WriteLine("#############################################");
        }
        public static void notvalid()
        {
            Console.WriteLine("No valid command entered. Type help to list all commands.");
        }
    }
}
