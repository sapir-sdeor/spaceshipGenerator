using UnityEngine;
using System.Collections.Generic;
using Avrahamy.Meshes;
using BitStrap;

namespace SpaceshipGenerator {
    [CreateAssetMenu(menuName = "Spaceship Generator/Box", fileName = "BoxSpaceshipGenerator")]
    public class BoxSpaceshipGenerator : SpaceshipGenerator {
        public const float MIN_SIZE = 0.1f;
        private const int FACE_SIZE = 8;
        private const int LAST_FACE_OFFSET = 8 + 8 + 4;

        [SerializeField] FloatRange topSizeRange;
        [SerializeField] FloatRange sidesSizeRange;
        [SerializeField] bool autoClampToMaxRadius = true;

        protected void OnValidate() {
            if (topSizeRange.Min < MIN_SIZE) {
                Debug.LogError($"Size must be greater than {MIN_SIZE}");
                topSizeRange.Min = MIN_SIZE;
                topSizeRange.Max = Mathf.Max(topSizeRange.Max, MIN_SIZE);
            }
            if (sidesSizeRange.Min < MIN_SIZE) {
                Debug.LogError($"Size must be greater than {MIN_SIZE}");
                sidesSizeRange.Min = MIN_SIZE;
                sidesSizeRange.Max = Mathf.Max(sidesSizeRange.Max, MIN_SIZE);
            }
        }

        public override void Generate(EditableMesh mesh) {
            var topHalfSize = topSizeRange.RandomInside() / 2;
            var sidesHalfSize = sidesSizeRange.RandomInside() / 2;

            // The first face.
            const int POINTS_COUNT = 4 * 2 + 4 * 2 * 2;
            var points = new List<Vector3>(POINTS_COUNT) {
                new Vector3(sidesHalfSize, topHalfSize, -MAX_RADIUS),
                new Vector3(sidesHalfSize, -topHalfSize, -MAX_RADIUS),
                new Vector3(-sidesHalfSize, -topHalfSize, -MAX_RADIUS),
                new Vector3(-sidesHalfSize, topHalfSize, -MAX_RADIUS),
            };
            var uvs = new List<Vector2>(POINTS_COUNT) {
                new Vector2(1f, 0.25f),
                new Vector2(1f, 0f),
                new Vector2(0.75f, 0f),
                new Vector2(0.75f, 0.25f),
            };

            // Duplicate the first face and split its vertices so they can create
            // hard edges.
            points.Add(points[0]);
            uvs.Add(new Vector2(0f, 0.25f));
            for (int i = 1; i < 4; i++) {
                points.Add(points[i]);
                points.Add(points[i]);
                var uv = new Vector2(0.25f * i, 0.25f);
                uvs.Add(uv);
                uvs.Add(uv);
            }
            points.Add(points[0]);
            uvs.Add(new Vector2(1f, 0.25f));

            // Create the other end of the top and side faces.
            for (int i = 4; i < 4 + 8; i++) {
                var point = points[i];
                point.z = MAX_RADIUS;
                points.Add(point);
                var uv = uvs[i];
                uv.y = 0.75f;
                uvs.Add(uv);
            }

            // Add the last face.
            for (int i = 0; i < 4; i++) {
                var point = points[i];
                point.z = MAX_RADIUS;
                points.Add(point);
                var uv = uvs[i];
                uv.y = 1f - uv.y;
                uvs.Add(uv);
            }

            // There are 4 faces, each face has 2 triangles and each triangle has
            // the indices of 3 vertices.
            var triangles = new [] {
                // First face.
                0, 1, 3,
                1, 2, 3,
                // Right side.
                5, 4, 5 + FACE_SIZE,
                4, 4 + FACE_SIZE, 5 + FACE_SIZE,
                // Bottom side.
                7, 6, 7 + FACE_SIZE,
                6, 6 + FACE_SIZE, 7 + FACE_SIZE,
                // Left side.
                9, 8, 9 + FACE_SIZE,
                8, 8 + FACE_SIZE, 9 + FACE_SIZE,
                // Top side.
                11, 10, 11 + FACE_SIZE,
                10, 10 + FACE_SIZE, 11 + FACE_SIZE,
                // Last face (flipped compared to the first face).
                1 + LAST_FACE_OFFSET, 0 + LAST_FACE_OFFSET, 3 + LAST_FACE_OFFSET,
                2 + LAST_FACE_OFFSET, 1 + LAST_FACE_OFFSET, 3 + LAST_FACE_OFFSET,
            };

            if (autoClampToMaxRadius) {
                ClampToRadius(points, MAX_RADIUS);
            }

            mesh.SetPoints(points.ToArray(), triangles);

            mesh.Mesh.uv = uvs.ToArray();
        }
    }
}
