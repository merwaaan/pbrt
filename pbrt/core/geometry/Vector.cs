using System;

namespace pbrt.core.geometry
{
    public class Vector2<T>
    {
        public T X;
        public T Y;

        public static Vector2<T> Zero => new Vector2<T>((dynamic)0, (dynamic)0);
        public static Vector2<T> One => new Vector2<T>((dynamic)1, (dynamic)1);

        public Vector2()
        {
        }

        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public Vector2(Vector2<T> v)
        {
            X = v.X;
            Y = v.Y;
        }

        public static Vector2<T> operator +(Vector2<T> a, Vector2<T> b)
        {
            return new Vector2<T>((dynamic)a.X + b.X, (dynamic)a.Y + b.Y);
        }

        public static Vector2<T> operator -(Vector2<T> a, Vector2<T> b)
        {
            return new Vector2<T>((dynamic)a.X - b.X, (dynamic)a.Y - b.Y);
        }

        public static Vector2<T> operator *(Vector2<T> a, T s)
        {
            return new Vector2<T>((dynamic)a.X * s, (dynamic)a.Y * s);
        }

        public float LengthSquared()
        {
            return (dynamic)X * X + (dynamic)Y * Y;
        }

        public float Length()
        {
            return (float)Math.Sqrt(LengthSquared());
        }
    }

    public class Vector3<T>
    {
        public T X;
        public T Y;
        public T Z;

        public static Vector3<T> Zero => new Vector3<T>((dynamic)0, (dynamic)0, (dynamic)0);
        public static Vector3<T> One => new Vector3<T>((dynamic)1, (dynamic)1, (dynamic)1);

        public Vector3()
        {
        }

        public Vector3(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(Vector3<T> v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public static Vector3<T> operator +(Vector3<T> a, Vector3<T> b)
        {
            return new Vector3<T>((dynamic)a.X + b.X, (dynamic)a.Y + b.Y, (dynamic)a.Z + b.Z);
        }

        public static Vector3<T> operator -(Vector3<T> a, Vector3<T> b)
        {
            return new Vector3<T>((dynamic)a.X - b.X, (dynamic)a.Y - b.Y, (dynamic)a.Z - b.Z);
        }

        public static Vector3<T> operator *(Vector3<T> a, T s)
        {
            return new Vector3<T>((dynamic)a.X * s, (dynamic)a.Y * s, (dynamic)a.Z * s);
        }

        public static Vector3<T> operator -(Vector3<T> a)
        {
            return a.Negated();
        }

        public float LengthSquared()
        {
            return (dynamic)X * X + (dynamic)Y * Y + (dynamic)Z * Z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(LengthSquared());
        }

        public static float Dot(Vector3<T> a, Vector3<T> b)
        {
            return (dynamic)a.X * b.X + (dynamic)a.Y * b.Y + (dynamic)a.Z * b.Z;
        }

        public static float Dot(Vector3<T> a, Normal3<T> b)
        {
            return Dot(a, b.ToVector3());
        }

        public static float AbsDot(Vector3<T> a, Vector3<T> b)
        {
            return Math.Abs(Dot(a, b));
        }

        public static float AbsDot(Vector3<T> a, Normal3<T> b)
        {
            return Math.Abs(Dot(a, b));
        }

        public static Vector3<T> Cross(Vector3<T> a, Vector3<T> b)
        {
            return new Vector3<T>(
                (dynamic)a.Y * b.Z - (dynamic)a.Z * b.Y,
                (dynamic)a.Z * b.X - (dynamic)a.X * b.Z,
                (dynamic)a.X * b.Y - (dynamic)a.Y * b.X);
        }

        public Vector3<T> Normalized()
        {
            var l = Length();
            return new Vector3<T>((dynamic)X / l, (dynamic)Y / l, (dynamic)Z / l);
        }

        public Vector3<T> Negated()
        {
            return new Vector3<T>((dynamic)X * -1, (dynamic)Y * -1, (dynamic)Z * -1);
        }
    }
}
