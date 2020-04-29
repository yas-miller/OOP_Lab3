using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WindowsFormsApp1.Figures
{
    public partial class Circle: Ellipse
    {
        public int x, y, radius;
        public Circle() : base()
        {

        }
        /*public new void Draw(Graphics gdi)
        {
            int X, Y;
            X = x - radius;
            Y = y - radius;
            rectangle.X = X;
            rectangle.Y = Y;
            rectangle.Width = radius * 2;
            rectangle.Height = rectangle.Width;
            gdi.DrawEllipse(pen, rectangle);
        }*/
    }
}
