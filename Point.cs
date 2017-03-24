
namespace ConsoleApplication
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        internal Point Clone()
        {
            return new Point(){ X = this.X, Y = this.Y};
        }
    }
}