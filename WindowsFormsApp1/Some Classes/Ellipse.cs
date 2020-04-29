using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WindowsFormsApp1.Figures
{
    public partial class Ellipse: Rect
    {
        public new int x, y, radiusX, radiusY;
        public Ellipse(): base()
        {

        }

        public new void Draw(Graphics gdi)
        {
            int X, Y;
            X = x - radiusX;
            Y = y - radiusY;
            rectangle.X = X;
            rectangle.Y = Y;
            rectangle.Width = radiusX * 2;
            rectangle.Height = radiusY * 2;
            gdi.DrawEllipse(pen, rectangle);
        }
    }
}
