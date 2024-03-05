using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourInRow
{
    [Serializable]
    public struct Vector
    {
        public int x;
        public int y;
        public Vector(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    public static class VectorMath
    {
        public static Vector Substract(Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y);
        }
        public static Vector Sum(Vector a, Vector b)
        {
            return new Vector(a.x + b.x, a.y + b.y);
        }
        public static int Dot(Vector a, Vector b)
        {
            return (a.x * b.x + a.y * b.y);
        }
        public static double Cos(Vector a, Vector b)
        {
            return (double)(a.x * b.x + a.y * b.y) / (Math.Sqrt((double)(a.x * a.x + a.y * a.y)) * Math.Sqrt((double)(b.x * b.x + b.y * b.y)));
        }
        public static bool Equal(Vector a, Vector b)
        {
            return (a.x == b.x && a.y == b.y);
        }
        public static bool CBetweenAAndB(Vector a, Vector b, Vector c)
        {
            int ac_x = c.x - a.x;
            int ac_y = c.y - a.y;
            int ab_x = b.x - a.x;
            int ab_y = b.y - a.y;
            int bc_x = c.x - b.x;
            int bc_y = c.y - b.y;
            double tan_ac = (double)ac_y / ((double)ac_x);
            double tan_ab = (double)ab_y / ((double)ab_x);
            if (Math.Abs(tan_ac - tan_ab) < 0.01 && ac_x * ab_x > 0 && bc_x * ab_x < 0)
                return true;
            return false;
        }
    }
}
