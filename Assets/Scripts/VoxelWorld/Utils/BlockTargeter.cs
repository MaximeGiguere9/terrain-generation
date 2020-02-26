using UnityEngine;
using VoxelWorld.Terrain;

namespace VoxelWorld.Utils
{
	public class BlockTargeter
	{
		public class HitPoint
		{
			public readonly bool HasValue;
			public readonly Vector3Int Position;
			public readonly Vector3Int EntryFaceNormal;
			public readonly Vector3Int ExitFaceNormal;

			public HitPoint()
			{
				this.HasValue = false;
				this.Position = Vector3Int.zero;
				this.EntryFaceNormal = Vector3Int.zero;
				this.ExitFaceNormal = Vector3Int.zero;
			}

			public HitPoint(Vector3Int position, Vector3Int entryFaceNormal, Vector3Int exitFaceNormal)
			{
				this.HasValue = true;
				this.Position = position;
				this.EntryFaceNormal = entryFaceNormal;
				this.ExitFaceNormal = exitFaceNormal;
			}
		}

		public static HitPoint Target(Vector3 position, Vector3 direction, float maxDistance)
		{
			/*
			 * March through a series of ray-box intersections to find the closest block that can be targeted.
			 *  - Find the exit intersection of the ray with the block containing the initial position
			 *  - The point is on a face, the normal of that face indicates the next block to check
			 *  - Execute points 1 and 2 on the new blocks in sequence until either a targetable block is found or the max distance is reached
			 */

			Vector3Int? nextBlockPos = Vector3Int.FloorToInt(position);

			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			// (will exit when max distance is reached or block is found)
			while (nextBlockPos.HasValue)
			{
				Vector3Int blockPosition = nextBlockPos.Value;
				nextBlockPos = null;

				Vector3Int entryFace = Vector3Int.zero;
				Vector3Int exitFace = Vector3Int.zero;

				for(int i = 0; i < VoxelBlock.Faces.Length; i++)
				{

					//face plane equation
					Vector3 u = VoxelBlock.Vertices[VoxelBlock.FaceVertices[i][1]] -
					            VoxelBlock.Vertices[VoxelBlock.FaceVertices[i][0]];

					Vector3 v = VoxelBlock.Vertices[VoxelBlock.FaceVertices[i][2]] -
					            VoxelBlock.Vertices[VoxelBlock.FaceVertices[i][1]];

					Vector3 p0 = VoxelBlock.Vertices[VoxelBlock.FaceVertices[i][0]] + blockPosition;

					//solving intersection with linear algebra
					//rayOrigin + t*dir = planeOrigin + x*u + y*v <=> x*u + y*v - t*dir = rayOrigin - planeOrigin
					Matrix4x4 matrix = new Matrix4x4();
					matrix.SetColumn(0, u);
					matrix.SetColumn(1, v);
					matrix.SetColumn(2, -direction);
					matrix.SetColumn(3, new Vector4(0, 0, 0, 1));

					Matrix4x4 inverse = matrix.inverse;

					if (inverse == Matrix4x4.zero)
						continue; //inverse does not exist

					Vector3 result = inverse.MultiplyVector(position - p0);

					if (result.x < 0 || result.x > 1 || result.y < 0 || result.y > 1)
						continue; //intersection outside face bounds
					if (result.z > maxDistance)
						continue; //target outside reach

					//if larger than 0, ray exits box, otherwise ray enters box
					float colinearity = Vector3.Dot(direction.normalized, VoxelBlock.Faces[i]);

					if (colinearity <= 0)
					{
						entryFace = VoxelBlock.Faces[i];
						continue;
					}

					exitFace = VoxelBlock.Faces[i];
					nextBlockPos = blockPosition + VoxelBlock.Faces[i];

					Debug.DrawLine(VoxelBlock.Vertices[VoxelBlock.FaceVertices[i][0]] + blockPosition, VoxelBlock.Vertices[VoxelBlock.FaceVertices[i][2]] + blockPosition, Color.blue);
				}

				if (!nextBlockPos.HasValue) break; //no valid block found

				byte blockId = VoxelTerrain.ActiveTerrain.GetBlockAt(nextBlockPos.Value);

				if (blockId > 0) return new HitPoint(nextBlockPos.Value, entryFace, exitFace); //found block
			}

			return new HitPoint();
		}

		//BUG! entry/exit normals match the block before the targeted one (workaround: exit normal is the opposite of the correct entry normal due to symmetry)

	}
}