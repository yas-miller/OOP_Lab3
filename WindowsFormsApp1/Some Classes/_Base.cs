using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization;

namespace WindowsFormsApp1.Figures
{
    public partial class _Base
    {
        protected Pen pen;
        protected Color color;
        protected decimal pen_size;
        public _Base()
        {
            this.color = new Color();
            this.pen_size = 1;
            pen = new Pen(color, (float)this.pen_size);
        }
    }
}
