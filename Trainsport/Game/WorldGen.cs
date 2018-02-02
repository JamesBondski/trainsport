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
            for (int i = 0; i < this.NumCities; i++) {
                newWorld.Cities.Add(new City() {
                    Name = this.CityNames[i],
                    Location = new Coordinates(newWorld.Rnd.NextDouble() * this.Size.X, newWorld.Rnd.NextDouble() * this.Size.Y)
                });
            }

            //Verbindungen hinzufügen, erstmal: Für jede Stadt 1-2 Verbindung zur nächsten noch nicht verbundenen Stadt
            for (int i = 0; i < this.NumCities; i++) {
                if (newWorld.Rnd.NextDouble() < 0.95) {
                    new Connection(newWorld.Cities[i], GetClosestUnconnectedCity(newWorld, newWorld.Cities[i]));
                }

                if (newWorld.Rnd.NextDouble() < 0.2) {
                    new Connection(newWorld.Cities[i], GetClosestUnconnectedCity(newWorld, newWorld.Cities[i]));
                }
            }
            FixSeparations(newWorld);
            while (FixLongDrives(newWorld)) ;

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

        //Here we check for cities, that are way closer geographically than they are by using roadso
        //All cities need to be connected to each other in some way first!
        private bool FixLongDrives(World world) {
            List<Tuple<Tuple<City, City>, double>> distances = new List<Tuple<Tuple<City, City>, double>>();
            for (int i = 0; i < world.Cities.Count; i++) {
                for (int j = i + 1; j < world.Cities.Count; j++) {
                    Tuple<City, City> cityPair = new Tuple<City, City>(world.Cities[i], world.Cities[j]);
                    double distance = GetShortestConnection(cityPair.Item1, cityPair.Item2).Item2;
                    double direct = cityPair.Item1.Location.GetDiff(cityPair.Item2.Location).GetLength();
                    distances.Add(new Tuple<Tuple<City, City>, double>(cityPair, distance / direct));
                }
            }
            distances.Sort((d1, d2) => d1.Item2.CompareTo(d2.Item2));

            var found = distances.Where(d => d.Item2 > 3).FirstOrDefault();
            if (found != null) {
                Console.WriteLine("Adding connection " + found.Item1.Item1.Name + "<->" + found.Item1.Item2.Name + ":" + found.Item2);
                new Connection(found.Item1.Item1, found.Item1.Item2);
                return true;
            }
            return false;
        }

        private Tuple<List<Connection>, double> GetShortestConnection(City origin, City destination) {
            return GetShortestConnection(origin, destination, new List<Connection>(), 0);
        }

        private Tuple<List<Connection>, double> GetShortestConnection(City origin, City destination, List<Connection> currentPath, double currentDistance) {
            City lastCity = origin;
            if (currentPath.Any()) {
                int count = currentPath.Count;
                if (count == 1) {
                    lastCity = currentPath[0].Cities.First(c => c != origin);
                }
                else {
                    lastCity = currentPath[count - 1].Cities.First(c => !currentPath[count - 2].Cities.Contains(c));
                }

                if (lastCity == destination) {
                    return new Tuple<List<Connection>, double>(currentPath, currentDistance);
                }
            }
            var potentialPaths = lastCity.Connections.Where(con => !currentPath.Contains(con));
            if (potentialPaths.Any()) {
                var results = potentialPaths.Select(con => {
                    List<Connection> newCurrentPath = new List<Connection>(currentPath);
                    newCurrentPath.Add(con);
                    return GetShortestConnection(origin, destination, newCurrentPath, currentDistance + con.Distance);
                }).Where(t => t != null).ToList();
                if (results.Any()) {
                    results.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
                    return results[0];
                }
                else {
                    return null;
                }
            }
            else {
                return null;
            }
        }

        private void FixSeparations(World world) {
            //We start at the first city and check which ones we can reach.
            List<City> reachedCities = new List<City>();
            List<Connection> checkedConnections = new List<Connection>();

            Queue<City> cityQueue = new Queue<City>();
            cityQueue.Enqueue(world.Cities[0]);

            while (cityQueue.Any()) {
                City city = cityQueue.Dequeue();
                foreach (Connection conn in city.Connections.Where(conn => !checkedConnections.Contains(conn))) {
                    City connectedCity = conn.Cities.Where(c => c != city).First();
                    if (!reachedCities.Contains(connectedCity) && !cityQueue.Contains(connectedCity)) {
                        cityQueue.Enqueue(connectedCity);
                    }
                }
                reachedCities.Add(city);
            }

            //Any city not in the reachedCities-list is disconnected from the first one
            //Find the shortest connection and make it
            if (reachedCities.Count != world.Cities.Count) {
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
                Console.WriteLine("Added " + pairs[0].Item1.Name + "->" + pairs[0].Item2.Name);

                FixSeparations(world);
            }
        }
    }
}
