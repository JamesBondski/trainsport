using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trainsport.Game;
using Trainsport.Util;

namespace Trainsport.UI
{
    public class MapPrinter
    {
        public World World { get; set; }

        private double scale = 4.5;
        public double Scale {
            get {
                return this.scale;
            }
            set {
                this.scale = value;
            }
        }

        public Coordinates Size {
            get {
                return new Coordinates(this.World.MapSize.X / scale, this.World.MapSize.Y / scale);
            }
        }

        public void Print() {
            for (double y = Scale / 2; y < World.MapSize.Y; y += Scale) {
                for (double x = Scale / 2; x < World.MapSize.X; x += Scale) {
                    var cities = World.Cities.Where(c => Math.Abs(c.Location.X - x) <= Scale / 2 && Math.Abs(c.Location.Y - y) <= Scale / 2);
                    var vehicles = World.Vehicles.Where(c => Math.Abs(c.Location.X - x) <= Scale / 2 && Math.Abs(c.Location.Y - y) <= Scale / 2);

                    if(vehicles.Any()) {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    if (cities.Any()) {
                        Console.Write(String.Join('/', cities.Select(c => c.Name.First().ToString())));
                    }
                    else if(vehicles.Any()) {
                        Console.Write("X");
                    }
                    else {
                        Console.Write(" ");
                    }

                    if (vehicles.Any()) {
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
