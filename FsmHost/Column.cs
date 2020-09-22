using System;
using System.Collections.Generic;
using System.Text;

namespace FsmHost
{
    public class Column
    {
        public List<string[]> Flightstrips = new List<string[]>();
        public int id;
        public string name;
        public Column(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }
}
