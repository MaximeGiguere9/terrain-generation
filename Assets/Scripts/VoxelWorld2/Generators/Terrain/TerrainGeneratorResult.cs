using System;
using UnityEngine;
using Utils;
using VoxelWorld2.Generators.Common;

namespace VoxelWorld2.Generators.Terrain
{
	public class TerrainGeneratorResult : IBlockGeneratorResult
	{
		private Vector3Int offset;
		private Vector3Int size;
		private byte[] blocks;

		public TerrainGeneratorResult(Vector3Int offset, Vector3Int size)
		{
			if (!IsValidSize(size))
				throw new ArgumentException("Size must be positive", nameof(size));

			this.offset = offset;
			this.size = size;
			this.blocks = new byte[size.x * size.y * size.z];
		}

		public Vector3Int GetOffset() => this.offset;

		public Vector3Int GetSize() => this.size;

		public byte? GetBlockAt(in Vector3Int position)
		{
			int arrayIndex = GetArrayIndex(this.size, position);
			if(arrayIndex < 0 || arrayIndex >= this.blocks.Length)
				throw new ArgumentOutOfRangeException(nameof(position));
			return this.blocks[arrayIndex];
		}

		public void SetBlockAt(in Vector3Int position, byte blockId)
		{
			int arrayIndex = GetArrayIndex(this.size, position);
			if (arrayIndex < 0 || arrayIndex >= this.blocks.Length)
				throw new ArgumentOutOfRangeException(nameof(position));
			this.blocks[arrayIndex] = blockId;
		}

		public void Resize(in Vector3Int newSize)
		{
			if (newSize == this.size)
				return;

			if(!IsValidSize(newSize))
				throw new ArgumentException("Size must be positive", nameof(newSize));

			/*
			 Because of the order of indexes in the buffer, modifying the y value is relatively cheap.
			 You just add or peel off "layers". However, changing the x or z value requires recalculating 
			 offsets for every block, which requires a lot more operations. The main use case for resizing is 
			 changing the max height during terrain generation, so this is fine.
			 */
			if (newSize.x == this.size.x && newSize.z == this.size.z)
			{
				byte[] newBuffer = new byte[newSize.x * newSize.y * newSize.z];
				Array.Copy(this.blocks, newBuffer, Math.Min(this.blocks.Length, newBuffer.Length));
				this.blocks = newBuffer;
				this.size = newSize;
			}
			else
			{
				byte[] newBuffer = new byte[newSize.x * newSize.y * newSize.z];
				CoordinateIterator itr = new CoordinateIterator(Vector3Int.Min(this.size, newSize), Vector3Int.zero);

				for (int i = 0; i < newBuffer.Length; i++)
				{
					itr.MoveNext();
					newBuffer[i] = this.blocks[GetArrayIndex(this.size, itr.Current)];
				}

				this.blocks = newBuffer;
				this.size = newSize;
			}
		}

		private static int GetArrayIndex(Vector3Int size, Vector3Int position) =>
			position.y * size.x * size.z + position.z * size.x + position.x;

		private static bool IsValidSize(Vector3Int size) => 
			size.x > 0 && size.y > 0 && size.z > 0;
	}
}