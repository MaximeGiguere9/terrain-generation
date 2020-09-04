using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.Terrain
{
	public class VoxelBlock
	{
		public static readonly Vector3 Size = Vector3Int.one;

		/// <summary>
		/// The 8 corners of the block in the right order
		/// </summary>
		public static readonly Vector3Int[] Vertices =
		{
			new Vector3Int(0, 0, 0),
			new Vector3Int(1, 0, 0),
			new Vector3Int(1, 0, 1),
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 1, 0),
			new Vector3Int(1, 1, 0),
			new Vector3Int(1, 1, 1),
			new Vector3Int(0, 1, 1),
		};

		/// <summary>
		/// The 6 face normals of the block
		/// </summary>
		public static readonly Vector3Int[] Faces =
		{
			Vector3Int.right,
			Vector3Int.left,
			Vector3Int.up,
			Vector3Int.down,
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 0, -1)
		};

		/// <summary>
		/// The 4 vertices (corners) forming a face of a block.
		/// The first dimension corresponds to the faces, and the second their vertices.
		/// Face index order matches <see cref="Faces"/>.
		/// Vertex index order matches <see cref="Vertices"/>.
		/// </summary>
		public static readonly byte[][] FaceVertices =
		{
			new byte[] {1, 2, 6, 5},
			new byte[] {3, 0, 4, 7},
			new byte[] {4, 5, 6, 7},
			new byte[] {3, 2, 1, 0},
			new byte[] {2, 3, 7, 6},
			new byte[] {0, 1, 5, 4}
		};

		/// <summary>
		/// Order of vertices used to make the two triangles of a face
		/// </summary>
		public static readonly byte[] FaceTriangles = {0, 1, 2, 0, 2, 3};

		/// <summary>
		/// Shorthand for obtaining vertices of a face in world position
		/// </summary>
		/// <param name="position">world position of the block</param>
		/// <param name="faceId">face index of the block (must match <see cref="Faces"/>)</param>
		/// <returns></returns>
		public static Vector3[] GetFaceVertices(Vector3 position, int faceId)
		{
			return new[]
			{
				position + Vertices[FaceVertices[faceId][0]],
				position + Vertices[FaceVertices[faceId][1]],
				position + Vertices[FaceVertices[faceId][2]],
				position + Vertices[FaceVertices[faceId][3]]
			};
		}
		
		public static Vector2[] GetFaceUVs(byte blockId, int faceIndex)
		{
			int ti = blocks[blockId].textureIds[faceIndex];
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

		public static bool IsOpaque(byte blockId)
		{
			return blocks.TryGetValue(blockId, out BlockTypeConfig value) && value.isOpaque;
		}

		public static bool IsFaceVisible(Vector3Int position, int faceIndex)
		{
			return !IsOpaque(VoxelTerrain.ActiveTerrain.GetBlockAt(position + Faces[faceIndex]));
		}

		private class BlockTypeConfig
		{
			public byte blockId;
			public string name;
			public int[] textureIds;
			public bool isOpaque;
		}

		private static readonly Dictionary<byte, BlockTypeConfig> blocks = new Dictionary<byte, BlockTypeConfig>
		{
			{
				1,
				new BlockTypeConfig
					{blockId = 1, name = "stone", textureIds = new[] {241, 241, 241, 241, 241, 241}, isOpaque = true}
			},
			{
				2,
				new BlockTypeConfig
					{blockId = 2, name = "bedrock", textureIds = new[] {225, 225, 225, 225, 225, 225}, isOpaque = true}
			},
			{
				3,
				new BlockTypeConfig
					{blockId = 3, name = "dirt", textureIds = new[] {242, 242, 242, 242, 242, 242}, isOpaque = true}
			},
			{
				4,
				new BlockTypeConfig
					{blockId = 4, name = "grass", textureIds = new[] {243, 243, 240, 242, 243, 243}, isOpaque = true}
			},
			{
				5,
				new BlockTypeConfig
					{blockId = 5, name = "leaves", textureIds = new[] {196, 196, 196, 196, 196, 196}, isOpaque = false}
			},
			{
				6,
				new BlockTypeConfig
					{blockId = 6, name = "brick", textureIds = new[] {247, 247, 247, 247, 247, 247}, isOpaque = true}
			},
			{
				7,
				new BlockTypeConfig
					{blockId = 7, name = "water", textureIds = new [] {61, 61, 61, 61, 61, 61}, isOpaque = true}
			},
			{
				8,
				new BlockTypeConfig
					{blockId = 8, name = "snow", textureIds = new [] {178, 178, 178, 178, 178, 178}, isOpaque = true}
			},
			{
				9,
				new BlockTypeConfig
					{blockId = 9, name = "sand", textureIds = new [] {226, 226, 226, 226, 226, 226}, isOpaque = true}
			}
		};
	}
}