using OpenTK;

namespace pbrt.core.geometry
{
    public class Transform
    {
        public Matrix4 M { get; private set; }
        public Matrix4 MInv { get; private set; }

        public static Transform Identity => new Transform(Matrix4.Identity, Matrix4.Identity);

        public Transform()
            : this(Matrix4.Identity)
        {
        }

        public Transform(Matrix4 matrix)
        {
            M = matrix;
            MInv = matrix.Inverted();
        }

        public Transform(Matrix4 matrix, Matrix4 matrixInv)
        {
            M = matrix;
            MInv = matrixInv;
        }

        public static Transform Translate(Vector3<float> delta)
        {
            return Translate(delta.X, delta.Y, delta.Z);
        }

        public static Transform Translate(float x, float y, float z)
        {
            var m = new Matrix4(
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1);

            var mInv = new Matrix4(
                1, 0, 0, -x,
                0, 1, 0, -y,
                0, 0, 1, -z,
                0, 0, 0, 1);

            return new Transform(m, mInv);
        }

        public static Transform Scale(float x, float y, float z)
        {
            var m = new Matrix4(
                x, 0, 0, 0,
                0, y, 0, 0,
                0, 0, z, 0,
                0, 0, 0, 1);

            var mInv = new Matrix4(
                1 / x, 0, 0, 0,
                0, 1 / y, 0, 0,
                0, 0, 1 / z, 0,
                0, 0, 0, 1);

            return new Transform(m, mInv);
        }

        public static Transform LookAt(Point3<float> pos, Point3<float> target, Vector3<float> up)
        {
            Matrix4 cameraToWorld = new Matrix4();

            // The last column is the origin of camera space, ie. its world space position
            cameraToWorld.M14 = pos.X;
            cameraToWorld.M24 = pos.Y;
            cameraToWorld.M34 = pos.Z;
            cameraToWorld.M44 = 1;

            // Compute the basis vectors
            var dir = (target - pos).Normalized(); // The Z-axis looks forward
            var left = Vector3<float>.Cross(up.Normalized(), dir);
            up = Vector3<float>.Cross(dir, left); // Correct the up vector

            cameraToWorld.M11 = left.X;
            cameraToWorld.M21 = left.Y;
            cameraToWorld.M31 = left.Z;
            cameraToWorld.M41 = 0;

            cameraToWorld.M12 = up.X;
            cameraToWorld.M22 = up.Y;
            cameraToWorld.M32 = up.Z;
            cameraToWorld.M42 = 0;

            cameraToWorld.M13 = dir.X;
            cameraToWorld.M23 = dir.Y;
            cameraToWorld.M33 = dir.Z;
            cameraToWorld.M43 = 0;

            return new Transform(cameraToWorld.Inverted(), cameraToWorld);
        }

        public Transform Inverse()
        {
            return new Transform(MInv, M);
        }

        public static Transform operator *(Transform a, Transform b)
        {
            return new Transform(a.M * b.M);
        }

        public static Point3<float> operator *(Transform t, Point3<float> p)
        {
            float x = p.X, y = p.Y, z = p.Z;

            float px = t.M.M11 * x + t.M.M12 * y + t.M.M13 * z +t.M.M14;
            float py = t.M.M21 * x + t.M.M22 * y + t.M.M23 * z +t.M.M24;
            float pz = t.M.M31 * x + t.M.M32 * y + t.M.M33 * z +t.M.M34;
            float pw = t.M.M41 * x + t.M.M42 * y + t.M.M43 * z +t.M.M44;

            if (pw == 1)
                return new Point3<float>(px, py, pz);

            return new Point3<float>(px / pw, py / pw, pz / pw);
        }

        public static Vector3<float> operator *(Transform t, Vector3<float> p)
        {
            float x = p.X, y = p.Y, z = p.Z;

            float px = t.M.M11 * x + t.M.M12 * y + t.M.M13 * z;
            float py = t.M.M21 * x + t.M.M22 * y + t.M.M23 * z;
            float pz = t.M.M31 * x + t.M.M32 * y + t.M.M33 * z;

            return new Vector3<float>(px, py, pz);
        }

        public static Normal3<float> operator *(Transform t, Normal3<float> n)
        {
            // TODO handle scaling
            return new Normal3<float>(t * n.ToVector3());
        }

        public static Ray operator *(Transform t, Ray r)
        {
            var o = t * r.O;
            var d = t * r.D;
            var tMax = r.Tmax;

            return new Ray(o, d, tMax, r.Time/*, r.medium*/);
        }

        public static RayDifferential operator *(Transform t, RayDifferential rd)
        {
            Ray tr = t * new Ray(rd);
            
            return new RayDifferential(tr.O, tr.D, tr.Tmax, tr.Time/*, tr.medium*/)
            {
                HasDifferentials = rd.HasDifferentials,
                RxO = t * rd.RxO,
                RyO = t * rd.RyO,
                RxD = t * rd.RxD,
                RyD = t * rd.RyD
            };
        }

        public static Bounds3<float> operator *(Transform t, Bounds3<float> p)
        {
            return null;
        }

        public static SurfaceInteraction operator *(Transform t, SurfaceInteraction inter)
        {
            var p = t * inter.P;
            var n = (t * inter.N).Normalized();
            var wo = (t * inter.Wo).Normalized();

            var dpdu = t * inter.DpDu;
            var dpdv = t * inter.DpDv;
            var dndu = t * inter.DnDu;
            var dndv = t * inter.DnDv;

            // TODO need to transform shading data if no default geometry

            return new SurfaceInteraction(p, inter.PError, inter.Uv, wo, dpdu, dpdv, dndu, dndv, inter.Time, inter.Shape)
            {
                Bsdf = inter.Bsdf,
                Primitive = inter.Primitive
            };
        }
    }
}
