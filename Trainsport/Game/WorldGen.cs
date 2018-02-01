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
    }
}
