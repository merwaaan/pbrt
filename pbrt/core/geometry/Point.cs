using System;

namespace pbrt.core.geometry
{
    public class Point2<T>
    {
        public T X;
        public T Y;

        public static Point2<T> Zero => new Point2<T>((dynamic)0, (dynamic)0);
        public static Point2<T> One => new Point2<T>((dynamic)1, (dynamic)1);

        public Point2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public Point2(Point2<T> p)
        {
            X = p.X;
            Y = p.Y;
        }

        public Point2<int> ToInt()
        {
            return new Point2<int>((int)(dynamic)X, (int)(dynamic)Y);
        }

        public Point2<float> ToFloat()
        {
            return new Point2<float>((float)(dynamic)X, (float)(dynamic)Y);
        }

        public Point2<T> Floor()
        {
            return new Point2<T>((T)Math.Floor((dynamic)X), (T)Math.Floor((dynamic)Y));
        }

        public Point2<T> Ceiling()
        {
            return new Point2<T>((T)Math.Ceiling((dynamic)X), (T)Math.Ceiling((dynamic)Y));
        }

        public static Point2<T> Min(Point2<T> a, Point2<T> b)
        {
            return new Point2<T>((T)Math.Min((dynamic)a.X, (dynamic)b.X), (T)Math.Min((dynamic)a.Y, (dynamic)b.Y));
        }

        public static Point2<T> Max(Point2<T> a, Point2<T> b)
        {
            return new Point2<T>((T)Math.Max((dynamic)a.X, (dynamic)b.X), (T)Math.Max((dynamic)a.Y, (dynamic)b.Y));
        }

        public static Point2<T> operator +(Point2<T> p1, Point2<T> p2)
        {
            return new Point2<T>((dynamic)p1.X + p2.X, (dynamic)p1.Y + p2.Y);
        }

        public static Vector2<T> operator -(Point2<T> p1, Point2<T> p2)
        {
            return new Vector2<T>((dynamic)p1.X - p2.X, (dynamic)p1.Y - p2.Y);
        }

        public static Point2<T> operator +(Point2<T> p, Vector2<T> v)
        {
            return new Point2<T>((dynamic)p.X + v.X, (dynamic)p.Y + v.Y);
        }

        public static Point2<T> operator -(Point2<T> p, Vector2<T> v)
        {
            return new Point2<T>((dynamic)p.X - v.X, (dynamic)p.Y - v.Y);
        }

        public static Point2<T> operator *(Point2<T> p, T s)
        {
            return new Point2<T>((dynamic)p.X * s, (dynamic)p.Y * s);
        }
    }

    public class Point3<T>
    {
        public T X;
        public T Y;
        public T Z;

        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public static Point3<T> Zero => new Point3<T>((dynamic)0, (dynamic)0, (dynamic)0);
        public static Point3<T> One => new Point3<T>((dynamic)1, (dynamic)1, (dynamic)1);

        public Point3(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3(T x)
        {
            X = Y = Z = x;
        }

        public Point3(Point3<T> p)
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }
        
        public static Point3<T> operator +(Point3<T> p1, Point3<T> p2)
        {
            return new Point3<T>((dynamic)p1.X + p2.X, (dynamic)p1.Y + p2.Y, (dynamic)p1.Z + p2.Z);
        }

        public static Vector3<T> operator -(Point3<T> p1, Point3<T> p2)
        {
            return new Vector3<T>((dynamic)p1.X - p2.X, (dynamic)p1.Y - p2.Y, (dynamic)p1.Z - p2.Z);
        }

        public static Point3<T> operator +(Point3<T> p, Vector3<T> v)
        {
            return new Point3<T>((dynamic)p.X + v.X, (dynamic)p.Y + v.Y, (dynamic)p.Z + v.Z);
        }

        public static Point3<T> operator -(Point3<T> p, Vector3<T> v)
        {
            return new Point3<T>((dynamic)p.X - v.X, (dynamic)p.Y - v.Y, (dynamic)p.Z - v.Z);
        }

        public static Point3<T> operator *(Point3<T> p, T s)
        {
            return new Point3<T>((dynamic)p.X * s, (dynamic)p.Y * s, (dynamic)p.Z * s);
        }
    }
}
