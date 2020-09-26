using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FsmHost
{
    class Filemanager
    {

        public static string blacklist(string user, string ip)
        {
            if (!File.Exists("blacklist.txt"))
            {
                File.Create("blacklist.txt").Close();
            }
            Boolean alreadyBanned = false;
            var lines = File.ReadAllLines("blacklist.txt");
            foreach (string s in lines)
            {
                alreadyBanned = alreadyBanned == false ? user != "" ? s.Contains(user) : ip != "" ? s.Contains(ip) : false : false;
            }
            if (alreadyBanned)
            {
                return $"{user} has already been banned from the server.";
            }
            File.AppendAllText("blacklist.txt", $"{user};{ip}\n");
            return $"{user} has been banned from the server.";
        }
        public static string removeFromBlacklist(string user, string ip)
        {
            if (File.Exists("blacklist.txt"))
            {
                File.WriteAllLines("blacklist.txt", File.ReadLines("blacklist.txt")
                    .Where(l => !(user != "" ? l.Contains(user) : ip != "" ? l.Contains(ip) : false)).ToList());
            }
            return $"{user} has been unbanned from the server.";

        }

        public static Boolean isBanned(string user)
        {
            if (File.Exists("blacklist.txt"))
            {
                var lines = File.ReadAllLines("blacklist.txt");
                
                foreach (string s in lines)
                {
                    if (s.Contains(user)) return true;
                }
                return false;

            }
            return false;
        }
        public static Boolean isIpBanned(string ip)
        {
            if (File.Exists("blacklist.txt"))
            {
                var lines = File.ReadAllLines("blacklist.txt");
                foreach (string s in lines)
                {
                    return lines.Contains(ip);
                }
                return false;
            }
            return false;
        }

        public static string toggleWhitelist(Boolean b)
        {
            Manager.useWhitelist = b;
            if (b)
            {
                if (!File.Exists("whitelist.txt"))
                {
                    File.Create("whitelist.txt").Close();

                }

                return "Whitelist has been enabled";
            }
            else
            {
                //if (File.Exists("whitelist.txt"))
                //{
                //    File.Delete("whitelist.txt");
                //}

                return "Whitelist has been disabled";
            }

        }
        public static string addtowl(string user)
        {
            Boolean alreadyAdded = false;
            var lines = File.ReadAllLines("whitelist.txt");
            foreach (string s in lines)
            {
                alreadyAdded = alreadyAdded == false ? user != "" ? s.Contains(user) : false : false;
            }
            if (alreadyAdded)
            {
                return $"{user} has already been added to the whitelist.";
            }
            File.AppendAllText("whitelist.txt", $"{user}\n");
            return $"{user} has been added to the whitelist.";
        }

        public static string removefromwl(string user)
        {
            if (File.Exists("whitelist.txt"))
            {
                File.WriteAllLines("whitelist.txt", File.ReadLines("whitelist.txt")
                    .Where(l => !(user != "" ? l.Contains(user) : false)).ToList());
            }
            return $"{user} has been removed from the whitelist.";
        }
        public static string recoverfrom(string user)
        {
            return $"Data cleared and recovered from {user}.";
        }
        public static string reset()
        {
            return "Data cleared.";
        }



    }
}