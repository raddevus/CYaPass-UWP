using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;


namespace CYaPass
{
    class UserPath
    {
        public List<Point> allPoints = new List<Point>();
        public HashSet<Segment> allSegments = new HashSet<Segment>();
        public HashSet<int> postPointSet = new HashSet<int>();
        private Point currentPoint;
        public int PointValue;

        public void append(Point currentPoint, int postValue)
        {
            this.currentPoint = currentPoint;
           
            if (allPoints.Count == 0)
            {
                PointValue += postValue;
            }
            if (allPoints.Count >= 1)
            {
                int currentSegmentCount = allSegments.Count;
                allSegments.Add(new Segment(allPoints[allPoints.Count - 1], currentPoint));
                
                if (currentSegmentCount < allSegments.Count)
                {
                    // a new segment was added so you add more to the PointValue
                    PointValue += postValue;
                }
            }
            allPoints.Add(currentPoint);

        }
    }
}
