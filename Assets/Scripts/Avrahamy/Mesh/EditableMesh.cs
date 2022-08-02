using UnityEngine;
using Avrahamy.Math;

namespace Avrahamy.Meshes {
    public enum SpaceType {
        XYZ = 0,
        XY = 1,
        XZ = 2,
    };

    public class EditableMesh : UniqueMesh {
        [SerializeField] protected SpaceType space = SpaceType.XY;

        /// <summary>
        /// Triangulates the points by flattening them to 2D. The 2D space is
        /// defined in the 'space' variable.
        /// </summary>
        public void SetPoints(Vector3[] points) {
            var vec2Points = space == SpaceType.XY ? points.ToVector2XY() : points.ToVector2XZ();
            // Triangulate points for mesh.
            var indices = Triangulator.Triangulate(vec2Points);
            SetPoints(points, indices);
        }

        /// <summary>
        /// Triangulates the points by flattening them to 2D. The 2D space is
        /// defined in the 'space' variable.
        /// </summary>
        public void SetPoints(Vector2[] vec2Points) {
            // Triangulate points for mesh.
            var indices = Triangulator.Triangulate(vec2Points);
            var points = space == SpaceType.XY ? vec2Points.ToVector3XY() : vec2Points.ToVector3XZ();
            SetPoints(points, indices);
        }

        public void SetPoints(Vector3[] vertices, int[] triangles) {
            // Reset mesh.
            var sharedMesh = Mesh;
            sharedMesh.Clear();
            sharedMesh.vertices = vertices;
            sharedMesh.triangles = triangles;
            MeshFilter.sharedMesh.RecalculateBounds();
            MeshFilter.sharedMesh.RecalculateNormals();
        }
    }
}
