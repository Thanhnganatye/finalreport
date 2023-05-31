using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintClone
{
    public class MyRectangle : MyShape
    {

        public int Width { get; set; }
        public int Height { get; set; }
        public bool Fill { get; set; }
        public MyRectangle(MyPoint p1, MyPoint p2, bool fill) : base(p1, p2)
        {
            Width = P2.X - P1.X;
            Height = P2.Y - P1.Y;
            Fill = fill;
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (!Fill)
                g.DrawRectangle(pen, Math.Min(P1.X, P2.X), Math.Min(P1.Y, P2.Y), Math.Abs(Width), Math.Abs(Height));
            else
                g.FillRectangle(pen.Brush, Math.Min(P1.X, P2.X), Math.Min(P1.Y, P2.Y), Math.Abs(Width), Math.Abs(Height));

        }
    }
}
