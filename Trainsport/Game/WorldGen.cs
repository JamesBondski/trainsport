using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trainsport.Util;

namespace Trainsport.Game
{
    public class WorldGen
    {
        public int Seed { get; } = new Random().Next();

        public int NumCities { get; set; } = 10;
        public Coordinates Size { get; set; } = new Coordinates() { X = 100, Y = 100 };
        public string[] CityNames = new string[] {
            "Hamburg",
            "Leverkusen",
            "Wuppertal",
            "Köln",
            "Düsseldorf",
            "Essen",
            "Geesthacht",
            "Melsungen",
            "Bochum",
            "Oldenburg"
        };

        public World Generate() {
            World newWorld = new World();
            newWorld.Rnd = new Random(this.Seed);

            //Städte generieren
            for(int i=0; i<this.NumCities; i++) {
                newWorld.Cities.Add(new City() {
                    Name = this.CityNames[i],
                    Location = new Coordinates(newWorld.Rnd.NextDouble() * this.Size.X, newWorld.Rnd.NextDouble() * this.Size.Y)
                });
            }

            //Verbindungen hinzufügen, erstmal: Für jede Stadt 1-2 Verbindung zur nächsten noch nicht verbundenen Stadt
            for (int i = 0; i < this.NumCities; i++) {
                new Connection(newWorld.Cities[i],GetClosestUnconnectedCity(newWorld, newWorld.Cities[i]));
            }
            FixSeparations(newWorld);

            //Fahrzeug generieren
            newWorld.Vehicles.Add(new Vehicle() {
                CurrentCity = newWorld.Cities[0],
                Location = newWorld.Cities[0].Location
            });

            return newWorld;
        }

        private City GetClosestUnconnectedCity(World world, City city) {
            var otherCities = world.Cities.Where(c => c != city && !c.Connections.Any(con => con.Cities.Contains(c) && con.Cities.Contains(city))).ToList();
            otherCities.Sort((c1, c2) => c1.Location.GetDistanceTo(city.Location).CompareTo(c2.Location.GetDistanceTo(city.Location)));
            return otherCities[0];
        }

        private void FixSeparations(World world) {
            //We start at the first city and check which ones we can reach.
            List<City> reachedCities = new List<City>();
            List<Connection> checkedConnections = new List<Connection>();

            Queue<City> cityQueue = new Queue<City>();
            cityQueue.Enqueue(world.Cities[0]);
            
            while(cityQueue.Any()) {
                City city = cityQueue.Dequeue();
                foreach(Connection conn in city.Connections.Where(conn => !checkedConnections.Contains(conn))) {
                    City connectedCity = conn.Cities.Where(c => c != city).First();
                    if(!reachedCities.Contains(connectedCity) && !cityQueue.Contains(connectedCity)) {
                        cityQueue.Enqueue(connectedCity);
                    }
                }
                reachedCities.Add(city);
            }

            //Any city not in the reachedCities-list is disconnected from the first one
            //Find the shortest connection and make it
            if(reachedCities.Count != world.Cities.Count) {
                List<City> unreachedCities = world.Cities.Where(c => !reachedCities.Contains(c)).ToList();
                List<Tuple<City, City>> pairs = new List<Tuple<City, City>>();

                reachedCities.ForEach(reached => {
                    unreachedCities.ForEach(unreached => {
                        pairs.Add(new Tuple<City, City>(reached, unreached));
                    });
                });
                pairs.Sort((p1, p2) => {
                    return p1.Item1.Location.GetDiff(p1.Item2.Location).GetLength().CompareTo(
                        p2.Item1.Location.GetDiff(p2.Item2.Location).GetLength()
                        );
                });
                new Connection(pairs[0].Item1, pairs[0].Item2);

                FixSeparations(world);
            }
        }
    }
}
