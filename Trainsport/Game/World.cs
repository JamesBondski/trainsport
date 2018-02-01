using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Trainsport.Game
{
    public class World
    {
        public Random Rnd {
            get; set;
        }

        public List<City> Cities {
            get;
        } = new List<City>();

        public List<Vehicle> Vehicles {
            get;
        } = new List<Vehicle>();

        public void Tick() {
            Vehicles.ForEach(v => v.Tick(1));
        }
    }
}
