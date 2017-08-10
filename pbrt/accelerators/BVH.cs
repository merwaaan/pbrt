using pbrt.core;
using pbrt.core.geometry;
using System.Collections.Generic;
using System.Linq;
using System;

namespace pbrt.accelerators
{
    // Bounding Volume Hierarchy
    // ("Equal count" variant)
    public class BVH : Aggregate
    {
        private class PrimitiveInfo
        {
            public readonly int Index;
            public Bounds3<float> Bounds;
            public Point3<float> Centroid;

            public PrimitiveInfo(int id, Bounds3<float> bounds)
            {
                Index = id;
                Bounds = bounds;
                Centroid = bounds.Min * 0.5f + bounds.Max * 0.5f;
            }
        }

        private class BVHBuildNode
        {
            public BVHBuildNode[] Children = new BVHBuildNode[2];
            public Bounds3<float> Bounds;
            public int SplitAxis;

            public int FirstPrimitiveOffset;
            public int PrimitiveCount;

            public void InitLeaf(int first, int n, Bounds3<float> bounds)
            {
                FirstPrimitiveOffset = first;
                PrimitiveCount = n;
                Bounds = bounds;
            }

            public void InitInterior(int axis, BVHBuildNode child1, BVHBuildNode child2)
            {
                Children[0] = child1;
                Children[1] = child2;
                PrimitiveCount = 0;
                Bounds = child1.Bounds.Union(child2.Bounds);
                SplitAxis = axis;
            }
        }

        public class BVHLinearNode
        {
            public Bounds3<float> Bounds;
            public int PrimitivesOffset; // For leafs
            public int SecondChildOffset; // For interior nodes
            public int PrimitiveCount;
            public int Axis;
        }

        public const int MaxPrimitivesPerNode = 3;

        private List<Primitive> allPrimitives;
        private BVHLinearNode[] nodes;

        public override Bounds3<float> WorldBounds => throw new NotImplementedException();

        public BVH(List<Primitive> primitives)
        {
            allPrimitives = new List<Primitive>(primitives);

            if (allPrimitives.Any())
            {
                var primitiveInfos = allPrimitives
                    .Select((p, i) => new PrimitiveInfo(i, p.WorldBounds))
                    .ToList();

                int nodeCount = 0;
                List<Primitive> orderedPrimitives = new List<Primitive>();
                var root = RecursiveBuild(primitiveInfos, 0, allPrimitives.Count(), ref nodeCount, ref orderedPrimitives);

                // Keep the ordered primitive list
                allPrimitives = orderedPrimitives;

                // Flatten the nodes into a linear list

                nodes = new BVHLinearNode[nodeCount];
                for (var i = 0; i < nodeCount; ++i)
                    nodes[i] = new BVHLinearNode();

                int offset = 0;
                FlattenBuildTree(root, ref offset);
            }
        }

        private BVHBuildNode RecursiveBuild(List<PrimitiveInfo> primitiveInfos, int start, int end, ref int nodeCount, ref List<Primitive> orderedPrimitives)
        {
            var node = new BVHBuildNode();
            ++nodeCount;

            // Compute the bounds of all the primitives below this node
            Bounds3<float> bounds = new Bounds3<float>();
            for (var i = start; i < end; ++i)
                bounds = bounds.Union(primitiveInfos[i].Bounds);

            var primitiveCount = end - start;

            // Create leaf node
            if (primitiveCount == 1)
            {
                var firstPrimitiveOffset = orderedPrimitives.Count();

                for (var i = start; i < end; ++i)
                    orderedPrimitives.Add(allPrimitives[primitiveInfos[i].Index]);

                node.InitLeaf(firstPrimitiveOffset, primitiveCount, bounds);
            }
            // Create interior node
            else
            {
                // Choose the axis to split (with the longest extent)
                var centroidBounds = new Bounds3<float>();
                for (var i = start; i < end; ++i)
                    centroidBounds = centroidBounds.Union(primitiveInfos[i].Centroid);
                var dim = centroidBounds.MaximumExtent();

                var mid = (start + end) / 2;

                // Juste create a leaf node if the bounds have zero volume
                // (all centroids are at the same position)
                if (centroidBounds.Max[dim] == centroidBounds.Min[dim])
                {
                    var firstPrimitiveOffset = orderedPrimitives.Count();

                    for (var i = start; i < end; ++i)
                        orderedPrimitives.Add(allPrimitives[primitiveInfos[i].Index]);

                    node.InitLeaf(firstPrimitiveOffset, primitiveCount, bounds);
                }
                else
                {
                    // Sort primitives in the current range along the split dimension

                    var orderedRange = primitiveInfos
                        .GetRange(start, end - start)
                        .OrderBy(p => p.Centroid[dim]);

                    primitiveInfos = primitiveInfos.GetRange(0, start)
                        .Concat(orderedRange)
                        .Concat(primitiveInfos.GetRange(end, primitiveInfos.Count - end))
                        .ToList();

                    // Recursively split the rest of the tree
                    node.InitInterior(
                        dim,
                        RecursiveBuild(primitiveInfos, start, mid, ref nodeCount, ref orderedPrimitives),
                        RecursiveBuild(primitiveInfos, mid, end, ref nodeCount, ref orderedPrimitives));
                }
            }

            return node;
        }

        private int FlattenBuildTree(BVHBuildNode node, ref int offset)
        {
            var linearNode = nodes[offset];
            linearNode.Bounds = node.Bounds;

            var myOffset = offset++;

            // Leaf node
            if (node.PrimitiveCount > 0)
            {
                linearNode.PrimitivesOffset = node.FirstPrimitiveOffset;
                linearNode.PrimitiveCount = node.PrimitiveCount;
            }
            // Interior node
            else
            {
                linearNode.Axis = node.SplitAxis;
                linearNode.PrimitiveCount = 0;

                FlattenBuildTree(node.Children[0], ref offset);

                linearNode.SecondChildOffset = FlattenBuildTree(node.Children[1], ref offset);
            }

            return myOffset;
        }

        public override bool Intersect(Ray ray, out SurfaceInteraction inter)
        {
            var hit = false;
            inter = null;

            var invDir = new Vector3<float>(1.0f / ray.D.X, 1.0f / ray.D.Y, 1.0f / ray.D.Z);
            bool[] dirIsNeg = { invDir.X < 0, invDir.Y < 0, invDir.Z < 0 };

            int toVisitOffset = 0, currentNodeIndex = 0;
            int[] nodesToVisit = new int[64];
            while (true)
            {
                var node = nodes[currentNodeIndex];

                if (node.Bounds.IntersectP(ray, out float t0, out float t1))
                {
                    // Leaf node: intersect ray with primitives
                    if (node.PrimitiveCount > 0)
                    {
                        for (var i = 0; i < node.PrimitiveCount; ++i)
                            if (allPrimitives[node.PrimitivesOffset + i].Intersect(ray, out inter))
                                hit = true;

                        if (toVisitOffset == 0)
                            break;
                        currentNodeIndex = nodesToVisit[--toVisitOffset];
                    }
                    // Interior node: process children
                    else
                    {
                        if (dirIsNeg[node.Axis])
                        {
                            nodesToVisit[toVisitOffset++] = currentNodeIndex + 1;
                            currentNodeIndex = node.SecondChildOffset;
                        }
                        else
                        {
                            nodesToVisit[toVisitOffset++] = node.SecondChildOffset;
                            currentNodeIndex = currentNodeIndex + 1;
                        }
                    }
                }
                else
                {
                    if (toVisitOffset == 0)
                        break;
                    currentNodeIndex = nodesToVisit[--toVisitOffset];
                }
            }

            return hit;
        }

        public override bool IntersectP(Ray ray)
        {
            return Intersect(ray, out SurfaceInteraction inter);
        }

        public override void ComputeScatteringFunctions(SurfaceInteraction inter, bool allowMultipleLobes)
        {
            throw new NotImplementedException();
        }
    }
}
