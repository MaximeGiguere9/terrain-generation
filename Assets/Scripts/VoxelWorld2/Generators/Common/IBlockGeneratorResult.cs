﻿using UnityEngine;

namespace VoxelWorld2.Generators.Common
{
	/// <summary>
	/// Generate a 3D rectangular area to feed to the world, independent of chunk and world implementation
	/// </summary>
	public interface IBlockGeneratorResult
	{
		/// <summary>
		/// Start position of the generated area in the world coordinate system
		/// </summary>
		/// <returns></returns>
		Vector3Int GetOffset();

		/// <summary>
		/// Dimensions of the generated area
		/// </summary>
		/// <returns></returns>
		Vector3Int GetSize();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position">Position of the block in the local coordinate system</param>
		/// <returns></returns>
		byte? GetBlockAt(in Vector3Int position);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="blockId"></param>
		void SetBlockAt(in Vector3Int position, byte blockId);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newSize"></param>
		void Resize(in Vector3Int newSize);
	}
}