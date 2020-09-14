using System;
using UnityEngine;

namespace VoxelWorld2.Chunks
{
	public class ChunkController
	{
		private readonly ChunkModel model;

		/// <summary>
		/// Cached position of the chunk (in chunk space)
		/// </summary>
		private Vector3Int position;

		public ChunkController() : this(new ChunkModel{Size = 1, Blocks = new byte[1], Position = new[] { 0, 0, 0 } }) { }

		public ChunkController(ChunkModel model)
		{
			this.model = model;
			this.position = new Vector3Int(this.model.Position[0], this.model.Position[1], this.model.Position[2]);
		}

		public byte GetSize() => this.model.Size;

		public void SetSize(byte size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException(nameof(size));

			if (this.model.Size == size)
				return;

			int oldBufferSize = this.model.Size * this.model.Size * this.model.Size;
			int newBufferSize = size * size * size;

			byte[] newArray = new byte[newBufferSize];
			Array.Copy(this.model.Blocks, newArray, Math.Min(oldBufferSize, newBufferSize));
			this.model.Blocks = newArray;
			this.model.Size = size;
		}

		public Vector3Int GetPosition() => this.position;

		public void SetPosition(Vector3Int position)
		{
			if (this.position == position) return;

			this.position = position;
			this.model.Position = new[] {this.position.x, this.position.y, this.position.z};
		}

		public byte GetBlockAt(Vector3Int position)
		{
			int offset = GetBlockArrayOffset(position, this.model.Size);
			return this.model.Blocks[offset];
		}

		public void SetBlockAt(Vector3Int position, byte blockId)
		{
			int offset = GetBlockArrayOffset(position, this.model.Size);
			this.model.Blocks[offset] = blockId;
		}

		/// <summary>
		/// Get the offset of a 3D position in a 1D array
		/// </summary>
		/// <param name="blockPos"></param>
		/// <param name="chunkSize"></param>
		/// <returns></returns>
		private static int GetBlockArrayOffset(Vector3Int blockPos, int chunkSize) =>
			blockPos.y * chunkSize * chunkSize + blockPos.z * chunkSize + blockPos.x;
	}
}