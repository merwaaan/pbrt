using System;

namespace pbrt.core.geometry
{
    public class Normal3<T>
    {
        public T X;
        public T Y;
        public T Z;

        public static Normal3<T> Zero => new Normal3<T>((dynamic)0, (dynamic)0, (dynamic)0);

        public Normal3()
        {
        }

        public Normal3(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Normal3(Vector3<T> v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public float LengthSquared()
        {
            return (dynamic)X * X + (dynamic)Y * Y + (dynamic)Z * Z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(LengthSquared());
        }

        public Normal3<T> Normalized()
        {
            var l = Length();
            return new Normal3<T>((dynamic)X / l, (dynamic)Y / l, (dynamic)Z / l);
        }

        public Vector3<T> ToVector3()
        {
            return new Vector3<T>(X, Y, Z);
        }
    }
}
