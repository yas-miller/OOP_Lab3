using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WindowsFormsApp1.Figures
{
    public partial class Square: Rect
    {
        public new int x, y, width;
        public Square() : base()
        {

        }
        public new void Draw(Graphics gdi)
        {
            rectangle = new Rectangle();
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = width;
            gdi.DrawRectangle(pen, rectangle);
        }
    }
}
