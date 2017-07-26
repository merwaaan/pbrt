using System;
using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.shapes
{
    public class Sphere : Shape
    {
        public readonly float Radius;

        public Sphere(Transform objectToWorld, float radius)
            : this(objectToWorld, objectToWorld.Inverse(), radius)
        {
        }

        public Sphere(Transform objectToWorld, Transform worldToObject, float radius)
            : base(objectToWorld, worldToObject)
        {
            Radius = radius;
        }

        public override Bounds3<float> ObjectBounds()
        {
            return new Bounds3<float>(
                new Point3<float>(-Radius, -Radius, -Radius),
                new Point3<float>(+Radius, +Radius, +Radius));
        }

        public override bool Intersect(Ray ray, out float t, out SurfaceInteraction inter)
        {
            t = 0.0f;
            inter = null;

            // Transform the ray from world space to object space
            var rayObj = WorldToObject * ray;

            // The implicit representation of the sphere is:
            //
            //            x² + y² + z² - r² = 0
            //
            // Subtituting the parametric representation of the ray:
            //
            //  (ox + t dx)² + (oy + t dy)² + (oz + t dz)² = r²
            //
            // Or, with the following coefficients:
            //
            //              a t² + b t + c = 0

            var a = rayObj.D.X * rayObj.D.X + rayObj.D.Y * rayObj.D.Y + rayObj.D.Z * rayObj.D.Z;
            var b = 2 * (rayObj.D.X * rayObj.O.X + rayObj.D.Y * rayObj.O.Y + rayObj.D.Z * rayObj.O.Z);
            var c = rayObj.O.X * rayObj.O.X + rayObj.O.Y * rayObj.O.Y + rayObj.O.Z * rayObj.O.Z - Radius * Radius;

            // Solve the quadration equation for t
            if (!Quadratic(a, b, c, out float t0, out float t1))
                return false;

            // Look for the nearest intersection

            if (t0 > rayObj.Tmax || t1 <= 0)
                return false;

            float tHit = t0;
            if (tHit <= 0)
            {
                tHit = t1;
                if (tHit > rayObj.Tmax)
                    return false;
            }
            t = tHit;

            // Initialize the interaction parameters

            var hitPos = rayObj.At(tHit);

            var hitPhi = Math.Atan2(hitPos.Y, hitPos.X); // Transform the hit point to polar coordinates
            if (hitPhi < 0) hitPhi += 2 * Math.PI;
            var u = hitPhi / (2 * Math.PI);
            var theta = Math.Acos((hitPos.Z / Radius));
            var v = theta / Math.PI;

            var zRadius = (float)Math.Sqrt(hitPos.X * hitPos.X + hitPos.Y * hitPos.Y);
            var invZRadius = 1.0f / zRadius;
            var cosPhi = hitPos.X * invZRadius;
            var sinPhi = hitPos.Y * invZRadius;
            var dpdu = new Vector3<float>(-2.0f * (float)Math.PI * hitPos.Y, 2.0f * (float)Math.PI * hitPos.X, 0);
            var dpdv = new Vector3<float>(hitPos.Z * cosPhi, hitPos.Z * sinPhi, -Radius * (float)Math.Sin(theta)) * -MathUtils.Pi;

            inter = ObjectToWorld * new SurfaceInteraction(
                hitPos, Vector3<float>.Zero, Point2<float>.Zero, -ray.D,
                dpdu, dpdv, Normal3<float>.Zero, Normal3<float>.Zero,
                0, this);

            return true;
        }

        private static bool Quadratic(float a, float b, float c, out float t0, out float t1)
        {
            t0 = t1 = 0.0f;

            float delta = b * b - 4 * a * c;
            if (delta < 0)
                return false;

            if (delta == 0.0f)
            {
                t0 = -b / (2 * a);
            }
            else
            {
                var sqrt = (float)Math.Sqrt(delta);
                t0 = (-b + sqrt) / (2 * a);
                t1 = (-b - sqrt) / (2 * a);

                if (t1 < t0)
                {
                    var tmp = t1;
                    t1 = t0;
                    t0 = tmp;
                }
            }

            return true;
        }

        public override float Area()
        {
            return (float)(4 * Math.PI * Radius * Radius);
        }
    }
}
