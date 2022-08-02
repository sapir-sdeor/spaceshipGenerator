using System.Collections.Generic;
using Avrahamy.Meshes;
using BitStrap;
using UnityEngine;

namespace SpaceshipGenerator
{
	public class OurSpaceShipGenerator : SpaceshipGenerator
	{
		[SerializeField] private FloatRange angleFromWingToSnot, shipHeight;
		private const float TailLength = 0.2f;

		public override void Generate(EditableMesh mesh)
		{
			float angle = angleFromWingToSnot.RandomInside(),
				height = MAX_RADIUS * shipHeight.RandomInside();
			var widestPartZValue = MAX_RADIUS * Mathf.Sin(angle);
			var widestPartXValue = MAX_RADIUS * Mathf.Cos(angle);
			var points = new List<Vector3>
			{
				//Base
				new Vector3(-MAX_RADIUS, 0, 0),
				new Vector3(widestPartXValue, 0, widestPartZValue),
				new Vector3(widestPartXValue, 0, -widestPartZValue),

				new Vector3(-MAX_RADIUS, 0, 0),
				//Widest part
				new Vector3(widestPartXValue, 0, widestPartZValue),
				new Vector3(widestPartXValue, height * 0.25f, widestPartZValue * 0.9f),
				new Vector3(widestPartXValue, height / 2, widestPartZValue * 0.75f),
				new Vector3(widestPartXValue, height * 0.75f, widestPartZValue * 0.5f),
				new Vector3(widestPartXValue, height, 0),
				new Vector3(widestPartXValue, height * 0.75f, -widestPartZValue * 0.5f),
				new Vector3(widestPartXValue, height / 2, -widestPartZValue * 0.75f),
				new Vector3(widestPartXValue, height * 0.25f, -widestPartZValue * 0.9f),
				new Vector3(widestPartXValue, 0, -widestPartZValue),

				//Back vertex
				new Vector3(widestPartXValue + TailLength, 0, 0),
				new Vector3(widestPartXValue + TailLength, 0, 0)
			};
			var triangles = new List<int>
			{
				//Base
				2, 1, 0,
				2, 14, 1
			};
			for (int j = 4; j <= 11; j++)
			{
				//Front
				triangles.Add(3);
				triangles.Add(j);
				triangles.Add(j + 1);
				//Back
				triangles.Add(j + 1);
				triangles.Add(j);
				triangles.Add(13);
			}

			var uvs = new List<Vector2>
			{
				new Vector2(0.75f, 0),
				new Vector2(0.5f, 0.5f),
				new Vector2(1f, 0.5f),
				new Vector2(0.25f, 0),
			};
			for (int i = 8; i >= 0; i--)
				uvs.Add(new Vector2(i / 16f, 0.5f));

			var uvsTails = new List<Vector2>
			{
				new Vector2(0.25f, 1),
				new Vector2(0.75f, 1f),
			};
			uvs.AddRange(uvsTails);
			mesh.SetPoints(points.ToArray(), triangles.ToArray());
			mesh.Mesh.uv = uvs.ToArray();
		}
	}
}