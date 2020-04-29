using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WindowsFormsApp1.Figures
{
    public partial class Triangle: _Base
    {
        protected PointF[] points;
        public new int x1, y1, x2, y2, x3, y3;
        public Triangle(): base()
        {
            points = new PointF[3];
        }
        public void Draw(Graphics gdi)
        {
            points[0] = new PointF(x1, y1);
            points[1] = new PointF(x2, y2);
            points[2] = new PointF(x3, y3);
            gdi.DrawPolygon(pen, points);
        }
    }
}
