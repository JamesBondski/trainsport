using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trainsport.Util;

namespace Trainsport.Game
{
    public class Vehicle
    {
        public Coordinates Location { get; set; }
        public Connection CurrentRoad { get; set; }
        public City CurrentCity { get; set; }
        public Coordinates CurrentSpeed { get; set; }
        public City TargetCity { get; set; }

        public double Speed { get; set; } = 3;

        public void Tick(double length) {
            Coordinates newLocation = new Coordinates() {
                X = this.Location.X + this.CurrentSpeed.X * length,
                Y = this.Location.Y + this.CurrentSpeed.Y * length
            };

            //Check if we have reached our destination
            if ((TargetCity.Location.X - this.Location.X) * this.CurrentSpeed.X < 0) {
                this.CurrentCity = this.TargetCity;
                this.TargetCity = null;
                this.CurrentRoad = null;
                this.CurrentSpeed = new Coordinates();
                this.Location = this.CurrentCity.Location;
            }
            else {
                this.Location = newLocation;
            }
        }

        public void Drive(Connection conn) {
            this.TargetCity = conn.Cities.Where(c => c != this.CurrentCity).First();
            this.CurrentSpeed = this.TargetCity.Location.GetDiff(this.Location).GetScaled(this.Speed);

            this.CurrentCity = null;
            this.CurrentRoad = conn;
        }
    }
}
