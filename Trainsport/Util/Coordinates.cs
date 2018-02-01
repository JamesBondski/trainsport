using System;
using System.Collections.Generic;
using System.Text;

namespace Trainsport.Util
{
    public struct Coordinates
    {
        public Coordinates(double x, double y) : this() {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString() {
            return X + ":" + Y;
        }

        public override bool Equals(object obj) {
            if (!(obj is Coordinates)) {
                return false;
            }

            var coordinates = (Coordinates)obj;
            return X == coordinates.X &&
                   Y == coordinates.Y;
        }

        public override int GetHashCode() {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public double GetDistanceTo(Coordinates other) {
            return this.GetDiff(other).GetLength();
        }

        public double GetLength() {
            return Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2));
        }

        public Coordinates GetDiff(Coordinates other) {
            return new Coordinates() {
                X = this.X - other.X,
                Y = this.Y - other.Y
            };
        }

        public Coordinates GetScaled(double length) {
            return new Coordinates() {
                X = this.X * length / this.GetLength(),
                Y = this.Y * length / this.GetLength()
            };
        }
    }
}
