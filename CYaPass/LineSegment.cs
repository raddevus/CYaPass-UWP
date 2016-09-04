using Windows.Foundation;

namespace CYaPass
{
    class LineSegment
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public LineSegment(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }
}
