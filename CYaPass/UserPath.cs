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
        private Point currentPoint;
        public int PointValue;
        private int previousPostValue;

        public void append(Point currentPoint, int postValue)
        {
            this.currentPoint = currentPoint;
            previousPostValue = postValue;
            
            if (allPoints.Count >= 1)
            {
                int currentSegmentCount = allSegments.Count;
                allSegments.Add(new Segment(allPoints[allPoints.Count - 1], currentPoint, postValue + previousPostValue));
            }
            allPoints.Add(currentPoint);

        }

        public void CalculateGeometricValue()
        {
            this.PointValue = 0;
            foreach (Segment s in allSegments)
            {
                this.PointValue += s.PointValue;
            }
        }
    }
}
