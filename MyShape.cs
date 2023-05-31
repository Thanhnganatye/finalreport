using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintClone
{
    public abstract class MyShape
    {
        protected MyPoint P1 { get; set; }
        protected MyPoint P2 { get; set; }

        protected MyShape(MyPoint p1, MyPoint p2)
        {
            P1 = p1;
            P2 = p2;
        }
        public abstract void Draw(Graphics g, Pen pen);
    }
}
