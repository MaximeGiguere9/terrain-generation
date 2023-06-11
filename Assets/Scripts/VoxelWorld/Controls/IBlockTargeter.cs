using UnityEngine;

namespace VoxelWorld.Controls
{
	public interface IBlockTargeter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="position">position of the object that is trying to target a block</param>
		/// <param name="direction">direction in which the object that is trying to target a block is aiming</param>
		/// <param name="maxDistance">maximum distance for the target to connect</param>
		/// <param name="hitPosition">when a block is hit, the position of that block is assigned to this variable</param>
		/// <param name="hitNormal">when a block is hit, the normal of the face of the block that was hit is assigned to this variable</param>
		/// <returns>True if a block was hit, false otherwise</returns>
		bool Target(in Vector3 position, in Vector3 direction, in float maxDistance, 
			ref Vector3Int hitPosition, ref Vector3Int hitNormal);
	}
}