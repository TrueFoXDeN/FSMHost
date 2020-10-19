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
            Boolean removed = isBanned(user) || isIpBanned(ip);
            if (File.Exists("blacklist.txt"))
            {
                File.WriteAllLines("blacklist.txt", File.ReadLines("blacklist.txt")
                    .Where(l => !(user != "" ? l.Contains(user) : ip != "" ? l.Contains(ip) : false)).ToList());
            }
            if(removed) return $"{user} has been unbanned from the server.";
            else return $"{user} was not banned.";

        }

        public static string[] bannedUsers()
        {
            if (File.Exists("blacklist.txt"))
            {
                return File.ReadAllLines("blacklist.txt");
            }
            return null;
            
        }
        public static string[] whitelistedUsers()
        {
            if (File.Exists("whitelist.txt"))
            {
                return File.ReadAllLines("whitelist.txt");
            }
            return null;
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
                    if (s.Contains(ip)) return true;
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

                return "Whitelist has been enabled.";
            }
            else
            {
                return "Whitelist has been disabled.";
            }

        }
        public static string addtowl(string user)
        {
            if (!File.Exists("whitelist.txt"))
            {
                File.Create("whitelist.txt").Close();
            }
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
            Boolean onWhitelist = isOnWhitelist(user);
            if (File.Exists("whitelist.txt"))
            {
                
                File.WriteAllLines("whitelist.txt", File.ReadLines("whitelist.txt")
                    .Where(l => !(user != "" ? l.Contains(user) : false)).ToList());
            }
            if (onWhitelist) return $"{user} has been removed from the whitelist.";
            else return $"{user} was not on the whitelist.";

        }
      
        public static Boolean isOnWhitelist(string user)
        {
            if (File.Exists("whitelist.txt"))
            {
                var lines = File.ReadAllLines("whitelist.txt");

                foreach (string s in lines)
                {
                    if (s.Contains(user)) return true;
                }
                return false;

            }
            return false;
        }


    }
}