using System;
using UnityEngine;
using VoxelWorld3.Blocks;

namespace Utils
{
	public static class BlockTargeter
	{
		public static readonly Vector3 BlockSize = Vector3Int.one;

		public class HitPoint
		{
			public readonly Vector3Int Position;
			public readonly Vector3Int Normal;

			public HitPoint(Vector3Int position, Vector3Int normal)
			{
				this.Position = position;
				this.Normal = normal;
			}
		}

		public static bool DebugMode { get; set; } = false;

		public static HitPoint Target(Vector3 position, Vector3 direction, float maxDistance, Func<Vector3Int, byte?> blockGetter, IBlockShapeProvider blockShapeProvider)
		{
			/*
			 * March through a series of ray-box intersections to find the closest block that can be targeted.
			 *  - Find the exit intersection of the ray with the block containing the initial position
			 *  - The point is on a face, the normal of that face indicates the next block to check
			 *  - Execute points 1 and 2 on the new blocks in sequence until either a targetable block is found or the max distance is reached
			 */

			DrawTargetRay(position, direction, maxDistance, Color.cyan);

			Vector3Int blockPosition = Vector3Int.FloorToInt(position);
			HitPoint hitPoint = null;

			// (will exit when max distance is reached or block is found)
			while (GetDistanceToBlock(position, blockPosition) <= maxDistance)
			{
				DrawBlockOutline(blockPosition, Color.blue, blockShapeProvider);

				//get normal of face through which ray exits block
				Vector3Int? faceNormal = FindRayBoxExitNormal(position, direction, blockPosition, blockShapeProvider);

				if (!faceNormal.HasValue)
					throw new InvalidOperationException($"Ray from {position} in direction {direction} does not intersect box at {blockPosition}");

				//march to next block in trajectory
				Vector3Int exitFace = faceNormal.Value;
				blockPosition += exitFace;

				byte? blockId = blockGetter(blockPosition);

				//block is empty
				if (!blockId.HasValue || blockId <= 0)
					continue;

				//found block, collision normal is inverse of current block exit point (next block entry point)
				hitPoint = new HitPoint(blockPosition, exitFace * -1);

				DrawBlockOutline(hitPoint.Position, Color.cyan, blockShapeProvider);
				DrawBlockNormal(blockPosition, exitFace * -1, Color.cyan);

				break;
			}

			return hitPoint;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="blockPosition"></param>
		/// <returns></returns>
		private static float GetDistanceToBlock(Vector3 origin, Vector3Int blockPosition)
		{
			return (blockPosition + BlockSize / 2f - origin).magnitude;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rayOrigin"></param>
		/// <param name="rayDirection"></param>
		/// <param name="blockPosition"></param>
		/// <returns>The normal of the face through which the ray exists the box, or null if the ray does not intersect the box</returns>
		private static Vector3Int? FindRayBoxExitNormal(Vector3 rayOrigin, Vector3 rayDirection, Vector3Int blockPosition, IBlockShapeProvider blockShapeProvider)
		{
			var faces = BlockService.Instance.GetFaceOrder();

			for (int i = 0; i < faces.Length; i++)
			{
				//find intersection for individual face plane
				Vector3? intersection = GetRayFaceIntersection(rayOrigin, rayDirection, blockPosition, i, blockShapeProvider);

				//no intersection
				if (!intersection.HasValue)
					continue;

				Vector3 result = intersection.Value;

				//intersection outside face bounds
				if (result.x < 0 || result.x > 1 || result.y < 0 || result.y > 1)
					continue;

				Vector3Int faceNormal = faces[i];

				//if larger than 0, ray exits box, otherwise ray enters box
				float colinearity = Vector3.Dot(rayDirection.normalized, faceNormal);

				//found entry face, need exit face to obtain next block position
				if (colinearity <= 0)
					continue;

				//found exit face (unique)
				return faceNormal;
			}

			return null;
		}

		/// <summary>
		/// Solves intersection between a ray and a block's face using the inverse matrix method
		/// </summary>
		/// <param name="rayOrigin"></param>
		/// <param name="rayDirection"></param>
		/// <param name="blockPosition"></param>
		/// <param name="faceIndex"></param>
		/// <returns></returns>
		private static Vector3? GetRayFaceIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3Int blockPosition, int faceIndex, IBlockShapeProvider blockShapeProvider)
		{
			var verts = blockShapeProvider.GetVertexOrder();
			var faceVerts = blockShapeProvider.GetFaceVertexOrder();

			//face plane equation components
			Vector3 u = verts[faceVerts[faceIndex][1]] -
						verts[faceVerts[faceIndex][0]];

			Vector3 v = verts[faceVerts[faceIndex][2]] -
						verts[faceVerts[faceIndex][1]];

			Vector3 p0 = verts[faceVerts[faceIndex][0]] + blockPosition;

			//solving (x,y,z) intersection with linear algebra
			//rayOrigin + t*dir = planeOrigin + x*u + y*v <=> x*u + y*v - t*dir = rayOrigin - planeOrigin
			Matrix4x4 matrix = new Matrix4x4();
			matrix.SetColumn(0, u);
			matrix.SetColumn(1, v);
			matrix.SetColumn(2, -rayDirection);
			matrix.SetColumn(3, new Vector4(0, 0, 0, 1));

			Matrix4x4 inverse = matrix.inverse;

			if (inverse == Matrix4x4.zero)
				return null; //inverse does not exist

			return inverse.MultiplyVector(rayOrigin - p0);
		}

		/// <summary>
		/// Debug
		/// </summary>
		/// <param name="position"></param>
		/// <param name="color"></param>
		private static void DrawBlockOutline(Vector3Int position, Color color, IBlockShapeProvider blockShapeProvider)
		{
			if (!DebugMode) return;

			var verts = blockShapeProvider.GetVertexOrder();
			var faceVerts = blockShapeProvider.GetFaceVertexOrder();

			foreach (byte[] vertexIndexes in faceVerts)
			{
				for (int i = 0; i < vertexIndexes.Length - 1; i++)
				{
					Vector3 start = position + verts[vertexIndexes[i]];
					Vector3 end = position + verts[vertexIndexes[i + 1]];

					Debug.DrawLine(start, end, color);
				}
			}
		}

		/// <summary>
		/// Debug
		/// </summary>
		/// <param name="position"></param>
		/// <param name="faceNormal"></param>
		/// <param name="color"></param>
		private static void DrawBlockNormal(Vector3 position, Vector3 faceNormal, Color color)
		{
			if (!DebugMode) return;

			Debug.DrawRay(position + BlockSize / 2, faceNormal / 2, color);
		}

		/// <summary>
		/// Debug
		/// </summary>
		/// <param name="position"></param>
		/// <param name="direction"></param>
		/// <param name="maxDistance"></param>
		/// <param name="color"></param>
		private static void DrawTargetRay(Vector3 position, Vector3 direction, float maxDistance, Color color)
		{
			if (!DebugMode) return;

			Debug.DrawLine(position, position + direction * maxDistance, color);
		}

	}
}