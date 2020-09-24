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
            File.AppendAllText("blacklist.txt", $"ban;{user};{ip}\n");
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

    }
}