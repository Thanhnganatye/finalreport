using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintClone
{
    public class MyLine : MyShape
    {
        public MyLine(MyPoint p1, MyPoint p2) : base(p1, p2)
        {
        }
        public override void Draw(Graphics g, Pen pen)
        {
            g.DrawLine(pen, P1.X, P1.Y, P2.X, P2.Y);
        }
    }
}
