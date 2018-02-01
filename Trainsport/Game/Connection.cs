using System;
using System.Collections.Generic;
using System.Text;

namespace Trainsport.Game
{
    public class Connection
    {
        public IList<City> Cities {
            get;
        } = new List<City>();

        public double Distance {
            get {
                return Cities[0].Location.GetDistanceTo(Cities[1].Location);
            }
        }

        public Connection(City city1, City city2) {
            this.Cities.Add(city1);
            this.Cities.Add(city2);

            city1.Connections.Add(this);
            city2.Connections.Add(this);
        }
    }
}
