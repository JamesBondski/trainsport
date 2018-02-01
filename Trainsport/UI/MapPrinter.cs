using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trainsport.Game;

namespace Trainsport.UI
{
    public class MapPrinter
    {
        public World World { get; set; }

        public void Print() {
            for (int y = 5; y < 100; y += 10) {
                for (int x = 5; x < 100; x += 10) {
                    var cities = World.Cities.Where(c => Math.Abs(c.Location.X - x) <= 5 && Math.Abs(c.Location.Y - y) <= 5);
                    var vehicles = World.Vehicles.Where(c => Math.Abs(c.Location.X - x) <= 5 && Math.Abs(c.Location.Y - y) <= 5);

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
