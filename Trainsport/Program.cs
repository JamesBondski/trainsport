using System;
using System.Linq;
using System.Threading;
using Trainsport.Game;
using Trainsport.UI;

namespace Trainsport
{
    class Program
    {
        static void Main(string[] args)
        {
            WorldGen gen = new WorldGen();
            World world = gen.Generate();
            /*
            Console.WriteLine("Seed: " + gen.Seed);
            foreach(City city in world.Cities) {
                Console.WriteLine(city.Name + "(" + (int)city.Location.X + ":" + (int)city.Location.Y + ")->" + String.Join(',', city.Connections.Select(con =>con.Cities.Where(c => c != city).First().Name)));
            }*/

            var printer = new MapPrinter() {
                World = world
            };

            Vehicle vehicle = world.Vehicles[0];
            string command = "";
            do {
                Clear();
                printer.Print();
                if (vehicle.CurrentCity != null) {
                    Console.WriteLine("You have reached your destination.");
                    command = GetNewDestination(vehicle);
                }
                else {
                    Console.WriteLine("You are driving towards " + vehicle.TargetCity.Name + "... Press <ENTER> to continue.");
                    Thread.Sleep(100);
                    world.Tick();
                }
                
                if(command == "l") {
                    Console.WriteLine(vehicle.Location);
                    if(vehicle.CurrentCity != null) {
                        Console.WriteLine(vehicle.CurrentCity.Location);
                    }
                    Console.ReadLine();
                }
            } while (command != "qu");
        }

        private static void Clear() {
            Console.SetCursorPosition(0, 11);
            for (int i = 0; i < 6; i++) {
                Console.WriteLine("                                                                          ");
            }
            Console.SetCursorPosition(0, 0);
        }

        private static string GetNewDestination(Vehicle vehicle) {
            string command;
            Console.WriteLine("You are currently in " + vehicle.CurrentCity.Name + ". Where do you want to go?");
            int i = 0;
            foreach (Connection road in vehicle.CurrentCity.Connections) {
                Console.WriteLine(i++ + ". " + road.Cities.Where(c => c != vehicle.CurrentCity).First().Name);
            }
            command = Console.ReadLine();
            int selected = 0;
            if(Int32.TryParse(command, out selected) && selected >=0 && selected < vehicle.CurrentCity.Connections.Count) {
                vehicle.Drive(vehicle.CurrentCity.Connections[selected]);
            }

            return command;
        }
    }
}
