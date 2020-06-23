using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS1
{
    class MyPoint
    {
        public double x;
        public double y;
        public double color;//-1 - red; 1 - blue
        public MyPoint(double x, double y, double color)
        {
            this.x = x;
            this.y = y;
            this.color = color;
        }
    }
}
