using UnityEngine;
using System.Collections.Generic;
using Avrahamy.Meshes;

namespace SpaceshipGenerator {
    public abstract class SpaceshipGenerator : ScriptableObject {
        public const float MAX_RADIUS = 0.9f;

        public abstract void Generate(EditableMesh mesh);

        /// <summary>
        /// Split points in a cross section that should extrude to a hard edge.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="angleThreshold"></param>
        /// <returns>The indices in the original list that were duplicated.</returns>
        protected static List<int> SplitByAngle(List<Vector2> points, float angleThreshold) {
            var splits = new List<int>();
            var n = points.Count;
            for (int i = 0; i < n; i++) {
                // Calculate the angle between each set of 3 points.
                var p0 = points[(i - 1 + n) % n];
                var p1 = points[i];
                var p2 = points[(i + 1) % n];
                var l0 = p0 - p1;
                if (l0.sqrMagnitude < 0.001f) {
                    // This is already a hard edge.
                    continue;
                }
                var l1 = p2 - p1;
                if (l1.sqrMagnitude < 0.001f) {
                    // This is already a hard edge.
                    continue;
                }
                var angle = Vector2.Angle(l0, l1);
                if (angle > angleThreshold) continue;
                // Should split the point.
                splits.Add(i);
            }
            if (splits.Count == 0) return splits;

            // Duplicate the split points.
            for (int i = n - 1; i >= 0; i--) {
                if (splits.Contains(i)) {
                    points.Insert(i, points[i]);
                }
            }
            return splits;
        }

        /// <summary>
        /// Flips the facing side of the triangles in the range.
        /// </summary>
        /// <param name="triangles"></param>
        /// <param name="fromIndex"></param>
        /// <param name="toIndex"></param>
        protected static void FlipTriangles(int[] triangles, int fromIndex = 0, int toIndex = -1) {
            Debug.Assert(fromIndex % 3 == 0, $"FlipTriangles got fromIndex={fromIndex}. Index must divide by 3!");
            if (toIndex < 0) {
                toIndex = triangles.Length - 1;
            }
            for (int i = fromIndex; i <= toIndex; i += 3) {
                (triangles[i], triangles[i + 1]) = (triangles[i + 1], triangles[i]);
            }
        }

        /// <summary>
        /// Assuming the center is (0,0,0), clamps the points to a sphere with
        /// the given radius.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="radius"></param>
        protected static void ClampToRadius(Vector3[] points, float radius) {
            var sqrRadius = radius * radius;
            for (var i = 0; i < points.Length; i++) {
                var point = points[i];
                if (point.sqrMagnitude > sqrRadius) {
                    points[i] = point.normalized * radius;
                }
            }
        }

        /// <summary>
        /// Assuming the center is (0,0,0), clamps the points to a sphere with
        /// the given radius.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="radius"></param>
        protected static void ClampToRadius(List<Vector3> points, float radius) {
            var sqrRadius = radius * radius;
            for (var i = 0; i < points.Count; i++) {
                var point = points[i];
                if (point.sqrMagnitude > sqrRadius) {
                    points[i] = point.normalized * radius;
                }
            }
        }
    }
}
