using System;
using System.Collections.Generic;
using System.Text;

namespace FsmHost
{
    public class Column
    {
        public List<string[]> Flightstrips = new List<string[]>();
        public string name;
        public Column(string name)
        {
            this.name = name;
        }
    }
}
