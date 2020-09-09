using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelWorld.Terrain;

namespace VoxelWorld2.Blocks
{
	public class BlockService
	{
		private Dictionary<byte, BlockModel> blocks;

		public void LoadBlocks(string path)
		{
			TextAsset res = Resources.Load(path) as TextAsset;
			BlocksConfig config = JsonUtility.FromJson<BlocksConfig>(res.text);
			this.blocks = config.Blocks.ToDictionary(b => b.Id, b => b);
		}

		/// <summary>
		/// Shorthand for obtaining vertices of a face in world position
		/// </summary>
		/// <param name="worldPosition">world position of the block</param>
		/// <param name="faceIndex">face index of the block</param>
		/// <returns></returns>
		public Vector3[] GetFaceVertices(Vector3 worldPosition, int faceIndex)
		{
			var verts = BlockProperties.Vertices;
			var faceVerts = BlockProperties.FaceVertices;

			return new[]
			{
				worldPosition + verts[faceVerts[faceIndex][0]],
				worldPosition + verts[faceVerts[faceIndex][1]],
				worldPosition + verts[faceVerts[faceIndex][2]],
				worldPosition + verts[faceVerts[faceIndex][3]]
			};
		}

		/// <summary>
		/// Get the 4 uv coordinates of a block face
		/// </summary>
		/// <param name="blockId"></param>
		/// <param name="faceIndex"></param>
		/// <returns></returns>
		public Vector2[] GetFaceUVs(byte blockId, int faceIndex)
		{
			int ti = this.blocks[blockId].TextureIndexes[faceIndex];
			int x = ti % 16;
			int y = ti / 16;
			return new[]
			{
				new Vector2(x / 16f, y / 16f),
				new Vector2((x + 1) / 16f, y / 16f),
				new Vector2((x + 1) / 16f, (y + 1) / 16f),
				new Vector2(x / 16f, (y + 1) / 16f)
			};
		}

		/// <summary>
		/// Whether a block with the given id is transparent or opaque.
		/// </summary>
		/// <param name="blockId"></param>
		/// <returns></returns>
		public bool IsTransparent(byte blockId)
		{
			return !this.blocks.TryGetValue(blockId, out BlockModel value) || value.Transparent;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="terrain"></param>
		/// <param name="position"></param>
		/// <param name="faceIndex"></param>
		/// <returns></returns>
		public bool IsFaceVisible(ref VoxelTerrain terrain, Vector3Int position, int faceIndex)
		{
			Vector3Int neighborPos = position + BlockProperties.Faces[faceIndex];
			byte block = terrain.GetBlockAt(position);
			byte neighbor = terrain.GetBlockAt(neighborPos);
			return this.IsTransparent(terrain.GetBlockAt(neighborPos)) && !(this.blocks[block].HideConnectingFaces && block == neighbor);
		}
	}
}