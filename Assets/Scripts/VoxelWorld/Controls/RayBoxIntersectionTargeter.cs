using System;
using UnityEngine;
using VoxelWorld.Blocks;

namespace VoxelWorld.Controls
{
	public class RayBoxIntersectionTargeter : IBlockTargeter
	{
		public static bool DebugMode { get; set; } = false;

		private readonly Func<Vector3Int, byte?> blockGetter;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="blockGetter">function that, when given a world position, returns a block at that position</param>
		/// <param name="blockService">object that provides information about the shape of the blocks</param>
		public RayBoxIntersectionTargeter(Func<Vector3Int, byte?> blockGetter) 
		{
			this.blockGetter = blockGetter;
		}

		public bool Target(in Vector3 position, in Vector3 direction, in float maxDistance, 
			ref Vector3Int hitPosition, ref Vector3Int hitNormal)
		{
			/*
			 * March through a series of ray-box intersections to find the closest block that can be targeted.
			 *  - Find the exit intersection of the ray with the block containing the initial position
			 *  - The point is on a face, the normal of that face indicates the next block to check
			 *  - Execute points 1 and 2 on the new blocks in sequence until either a targetable block is found or the max distance is reached
			 */

			DrawTargetRay(position, direction, maxDistance, Color.cyan);

			Vector3Int blockPosition = Vector3Int.FloorToInt(position);

			bool hit = false;

			// (will exit when max distance is reached or block is found)
			while (GetDistanceToBlock(position, blockPosition) <= maxDistance)
			{
				DrawBlockOutline(blockPosition, Color.blue);

				//get normal of face through which ray exits block
				Vector3Int? faceNormal = FindRayBoxExitNormal(position, direction, blockPosition);

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
				hitPosition = blockPosition;
				hitNormal = exitFace * -1;

				DrawBlockOutline(hitPosition, Color.cyan);
				DrawBlockNormal(blockPosition, exitFace * -1, Color.cyan);

				hit = true;
				break;
			}

			return hit;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="blockPosition"></param>
		/// <returns></returns>
		private float GetDistanceToBlock(in Vector3 origin, in Vector3Int blockPosition)
		{
			return (blockPosition + Vector3.one / 2f - origin).magnitude;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rayOrigin"></param>
		/// <param name="rayDirection"></param>
		/// <param name="blockPosition"></param>
		/// <returns>The normal of the face through which the ray exists the box, or null if the ray does not intersect the box</returns>
		private Vector3Int? FindRayBoxExitNormal(in Vector3 rayOrigin, in Vector3 rayDirection, in Vector3Int blockPosition)
		{
			var faces = BlockMeshModel.FaceNormals;

			for (int i = 0; i < faces.Length; i++)
			{
				//find intersection for individual face plane
				Vector3? intersection = GetRayFaceIntersection(rayOrigin, rayDirection, blockPosition, i);

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
		private Vector3? GetRayFaceIntersection(in Vector3 rayOrigin, in Vector3 rayDirection, in Vector3Int blockPosition, in int faceIndex)
		{
			var verts = BlockMeshModel.Vertices;
			var faceVerts = BlockMeshModel.FaceVertexOrder;

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
		private void DrawBlockOutline(in Vector3Int position, in Color color)
		{
			if (!DebugMode) return;

			var verts = BlockMeshModel.Vertices;
			var faceVerts = BlockMeshModel.FaceVertexOrder;

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
		private void DrawBlockNormal(in Vector3 position, in Vector3 faceNormal, in Color color)
		{
			if (!DebugMode) return;

			Debug.DrawRay(position + Vector3.one / 2f, faceNormal / 2f, color);
		}

		/// <summary>
		/// Debug
		/// </summary>
		/// <param name="position"></param>
		/// <param name="direction"></param>
		/// <param name="maxDistance"></param>
		/// <param name="color"></param>
		private void DrawTargetRay(in Vector3 position, in Vector3 direction, in float maxDistance, in Color color)
		{
			if (!DebugMode) return;

			Debug.DrawLine(position, position + direction * maxDistance, color);
		}

	}
}