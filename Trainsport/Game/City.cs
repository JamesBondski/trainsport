using System;
using System.Collections.Generic;
using System.Text;
using Trainsport.Util;

namespace Trainsport.Game
{
    public class City
    {
        public string Name { get; set; }
        public IList<Connection> Connections { get; } = new List<Connection>();
        public Coordinates Location { get; set; }
    }
}
