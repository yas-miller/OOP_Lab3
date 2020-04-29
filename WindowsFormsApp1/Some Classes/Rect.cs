using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WindowsFormsApp1.Figures
{
    public partial class Rect: _Base
    {
        protected Rectangle rectangle;
        public new int x, y, width, height;
        public Rect() : base()
        {
            rectangle = new Rectangle();
        }
        public void Draw(Graphics gdi)
        {
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
            gdi.DrawRectangle(pen, rectangle);
        }
    }
}
