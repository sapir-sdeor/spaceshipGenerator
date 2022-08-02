using UnityEngine;
using System.Collections.Generic;
using Avrahamy.Meshes;

namespace SpaceshipGenerator {
    [CreateAssetMenu(menuName = "Spaceship Generator/Plane", fileName = "PlaneGenerator")]
    public class PlaneGenerator : SpaceshipGenerator {
        public int divisions = 100;

        public override void Generate(EditableMesh mesh) {
            var points = new List<Vector3>();
            var uvs = new List<Vector2>();
            var triangles = new List<int>();
            for (int i = 0; i <= divisions; i++) {
                for (int j = 0; j <= divisions; j++) {
                    if (i > 0 && j > 0) {
                        triangles.Add(points.Count);
                        triangles.Add(points.Count - 1);
                        triangles.Add(points.Count - divisions - 2);
                        triangles.Add(points.Count);
                        triangles.Add(points.Count - divisions - 2);
                        triangles.Add(points.Count - divisions - 1);
                    }
                    var point = new Vector2((float)i / divisions, (float)j / divisions);
                    points.Add(point * 2f - Vector2.one);
                    uvs.Add(point);
                }
            }
            mesh.SetPoints(points.ToArray(), triangles.ToArray());

            mesh.Mesh.uv = uvs.ToArray();
        }
    }
}
