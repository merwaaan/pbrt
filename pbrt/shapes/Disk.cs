using System;
using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.shapes
{
    public class Disk : Shape
    {
        public readonly float Radius;
        public readonly float Height;

        public Disk(Transform objectToWorld, float radius, float height = 0)
            : this(objectToWorld, objectToWorld.Inverse(), radius, height)
        {
        }

        public Disk(Transform objectToWorld, Transform worldToObject, float radius, float height = 0)
            : base(objectToWorld, worldToObject)
        {
            Radius = radius;
            Height = height;
        }

        public override Bounds3<float> ObjectBounds()
        {
            return new Bounds3<float>(
                new Point3<float>(-Radius, -Radius, Height),
                new Point3<float>(+Radius, +Radius, Height));
        }

        public override bool Intersect(Ray ray, out float tHit, out SurfaceInteraction inter)
        {
            tHit = 0f;
            inter = null;

            // Transform the ray from world space to object space
            var rayObj = WorldToObject * ray;

            // Compute intersections with the disk plane
            //
            // We want t such that h = o_z + t d_z
            // Hence t = (h - o_z) / d_z

            if (rayObj.D.Z == 0)
                return false;

            tHit = (Height - rayObj.O.Z) / rayObj.D.Z;
            if (tHit <= 0 || tHit >= rayObj.Tmax)
                return false;

            // Check if the intersection point lies in the circle
            var hitPos = rayObj.At(tHit);
            var dist2 = hitPos.X * hitPos.X + hitPos.Y * hitPos.Y;
            if (dist2 > Radius * Radius)
                return false;

            // Refine the disk intersection point
            hitPos.Z = Height;

            // Compute the parametric representation of the intersection point

            var phi = Math.Atan2(hitPos.Y, hitPos.X);
            if (phi < 0)
                phi += 2 * MathUtils.Pi;

            var rHit = (float)Math.Sqrt(dist2);
            var dpdu = new Vector3<float>(-MathUtils.TwoPi * hitPos.Y, MathUtils.TwoPi * hitPos.X, 0);
            var dpdv = new Vector3<float>(hitPos.X, hitPos.Y, 0) * (-Radius / rHit);

            inter = ObjectToWorld * new SurfaceInteraction(
                hitPos, Vector3<float>.Zero, Point2<float>.Zero, -rayObj.D,
                dpdu, dpdv, Normal3<float>.Zero, Normal3<float>.Zero,
                rayObj.Time, this);

            return true;
        }

        public override float Area()
        {
            return MathUtils.Pi * Radius * Radius;
        }

        public override Interaction Sample(Point2<float> u)
        {
            var pDisk = MathUtils.ConcentricSampleDisk(u);
            var pObj = new Point3<float>(pDisk.X * Radius, pDisk.Y * Radius, Height);

            var inter = new Interaction(
                ObjectToWorld * pObj,
                ObjectToWorld * new Normal3<float>(0, 0, 1),
                Vector3<float>.Zero,
                Vector3<float>.Zero,
                0);

            /*if (reverseOrientation)
                inter.N *= 1;*/

            return inter;
        }

        public override Interaction Sample(Interaction inter, Point2<float> u)
        {
            throw new NotImplementedException();
        }
    }
}
