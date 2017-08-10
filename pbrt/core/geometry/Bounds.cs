using System;
using System.Collections.Generic;

namespace pbrt.core.geometry
{
    public class Bounds2<T>
    {
        public Point2<T> Min { get; protected set; }
        public Point2<T> Max { get; protected set; }

        public Bounds2(Point2<T> p)
        {
            Min = new Point2<T>(p);
            Max = new Point2<T>(p);
        }

        public Bounds2(Point2<T> p1, Point2<T> p2)
        {
            Min = new Point2<T>(Math.Min((dynamic)p1.X, (dynamic)p2.X), Math.Min((dynamic)p1.Y, (dynamic)p2.Y));
            Max = new Point2<T>(Math.Max((dynamic)p1.X, (dynamic)p2.X), Math.Max((dynamic)p1.Y, (dynamic)p2.Y));
        }

        public override string ToString()
        {
            return $"{Min} / {Max}";
        }

        public float Width()
        {
            return (dynamic)Max.X - Min.X;
        }

        public float Height()
        {
            return (dynamic)Max.Y - Min.Y;
        }

        public Vector2<T> Diagonal()
        {
            return Max - Min;
        }

        public IEnumerable<Point2<int>> IteratePoints()
        {
            dynamic yMin = Min.Y, xMin = Min.X, yMax = Max.Y, xMax = Max.X;

            for (var y = (int)yMin; y < (int)yMax; ++y)
                for (var x = (int)xMin; x < (int)xMax; ++x)
                    yield return new Point2<int>(x, y);
        }

        public static Bounds2<T> Union(Bounds2<T> b, Vector2<T> p)
        {
            return new Bounds2<T>(
                new Point2<T>(
                    Math.Min((dynamic)b.Min.X, (dynamic)p.X),
                    Math.Min((dynamic)b.Min.Y, (dynamic)p.Y)),
                new Point2<T>(
                    Math.Max((dynamic)b.Max.X, (dynamic)p.X),
                    Math.Max((dynamic)b.Max.Y, (dynamic)p.Y)));
        }

        public static Bounds2<T> Union(Bounds2<T> b1, Bounds2<T> b2)
        {
            return new Bounds2<T>(
                new Point2<T>(
                    Math.Min((dynamic)b1.Min.X, (dynamic)b2.Min.X),
                    Math.Min((dynamic)b1.Min.Y, (dynamic)b2.Min.Y)),
                new Point2<T>(
                    Math.Max((dynamic)b1.Max.X, (dynamic)b2.Max.X),
                    Math.Max((dynamic)b1.Max.Y, (dynamic)b2.Max.Y)));
        }

        public static Bounds2<T> Intersect(Bounds2<T> b1, Bounds2<T> b2)
        {
            return new Bounds2<T>(
                new Point2<T>(
                    Math.Max((dynamic)b1.Min.X, (dynamic)b2.Min.X),
                    Math.Max((dynamic)b1.Min.Y, (dynamic)b2.Min.Y)),
                new Point2<T>(
                    Math.Min((dynamic)b1.Max.X, (dynamic)b2.Max.X),
                    Math.Min((dynamic)b1.Max.Y, (dynamic)b2.Max.Y)));
        }

        public static bool Overlaps(Bounds2<T> b1, Bounds2<T> b2)
        {
            return
                (dynamic)b1.Max.X >= b2.Min.X && (dynamic)b1.Min.X <= b2.Max.X &&
                (dynamic)b1.Max.Y >= b2.Min.Y && (dynamic)b1.Min.Y <= b2.Max.Y;
        }

        public static Bounds2<T> Expand(Bounds2<T> b, float delta)
        {
            return new Bounds2<T>(b.Min - (dynamic)delta * Vector2<T>.One, b.Max + (dynamic)delta * Vector2<T>.One);
        }
    }

    public class Bounds3<T>
    {
        public Point3<T> Min { get; protected set; }
        public Point3<T> Max { get; protected set; }

        public Bounds3()
        {
            Min = new Point3<T>((dynamic)float.PositiveInfinity);
            Max = new Point3<T>((dynamic)float.NegativeInfinity);
        }

        public Bounds3(Point3<T> p)
        {
            Min = new Point3<T>(p);
            Max = new Point3<T>(p);
        }

        public Bounds3(Point3<T> p1, Point3<T> p2)
        {
            Min = new Point3<T>(Math.Min((dynamic)p1.X, (dynamic)p2.X), Math.Min((dynamic)p1.Y, (dynamic)p2.Y), Math.Min((dynamic)p1.Z, (dynamic)p2.Z));
            Max = new Point3<T>(Math.Max((dynamic)p1.X, (dynamic)p2.X), Math.Max((dynamic)p1.Y, (dynamic)p2.Y), Math.Max((dynamic)p1.Z, (dynamic)p2.Z));
        }

        public Vector3<T> Diagonal()
        {
            return Max - Min;
        }

        public int MaximumExtent()
        {
            var d = Diagonal();

            if ((dynamic)d.X > d.Y && (dynamic)d.X > d.Z)
                return 0;

            if ((dynamic)d.Y > d.Z)
                return 1;

            return 2;
        }

        public bool IntersectP(Ray ray, out float t0, out float t1)
        {
            t0 = 0;
            t1 = ray.Tmax;

            for (var i = 0; i < 3; ++i)
            {
                var invRayDir = 1 / ray.D[i];
                var tNear = ((dynamic)Min[i] - ray.O[i]) * invRayDir;
                var tFar = ((dynamic)Max[i] - ray.O[i]) * invRayDir;

                if (tNear > tFar)
                {
                    var tmp = tNear;
                    tNear = tFar;
                    tFar = tmp;
                }

                t0 = tNear > t0 ? tNear : t0;
                t1 = tFar < t1 ? tFar : t1;

                if (t0 > t1)
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"{Min} / {Max}";
        }

        public Bounds3<T> Union(Point3<T> p)
        {
            return new Bounds3<T>(
                new Point3<T>(
                    Math.Min((dynamic)Min.X, (dynamic)p.X),
                    Math.Min((dynamic)Min.Y, (dynamic)p.Y),
                    Math.Min((dynamic)Min.Z, (dynamic)p.Z)),
                new Point3<T>(
                    Math.Max((dynamic)Max.X, (dynamic)p.X),
                    Math.Max((dynamic)Max.Y, (dynamic)p.Y),
                    Math.Max((dynamic)Max.Z, (dynamic)p.Z)));
        }

        public Bounds3<T> Union(Bounds3<T> b)
        {
            return new Bounds3<T>(
                new Point3<T>(
                    Math.Min((dynamic)Min.X, (dynamic)b.Min.X),
                    Math.Min((dynamic)Min.Y, (dynamic)b.Min.Y),
                    Math.Min((dynamic)Min.Z, (dynamic)b.Min.Z)),
                new Point3<T>(
                    Math.Max((dynamic)Max.X, (dynamic)b.Max.X),
                    Math.Max((dynamic)Max.Y, (dynamic)b.Max.Y),
                    Math.Max((dynamic)Max.Z, (dynamic)b.Max.Z)));
        }

        public static Bounds3<T> Intersect(Bounds3<T> b1, Bounds3<T> b2)
        {
            return new Bounds3<T>(
                new Point3<T>(
                    Math.Max((dynamic)b1.Min.X, (dynamic)b2.Min.X),
                    Math.Max((dynamic)b1.Min.Y, (dynamic)b2.Min.Y),
                    Math.Max((dynamic)b1.Min.Z, (dynamic)b2.Min.Z)),
                new Point3<T>(
                    Math.Min((dynamic)b1.Max.X, (dynamic)b2.Max.X),
                    Math.Min((dynamic)b1.Max.Y, (dynamic)b2.Max.Y),
                    Math.Min((dynamic)b1.Max.Z, (dynamic)b2.Max.Z)));
        }

        public static bool Overlaps(Bounds3<T> b1, Bounds3<T> b2)
        {
            return
                (dynamic)b1.Max.X >= b2.Min.X && (dynamic)b1.Min.X <= b2.Max.X &&
                (dynamic)b1.Max.Y >= b2.Min.Y && (dynamic)b1.Min.Y <= b2.Max.Y;
        }
    }
}
