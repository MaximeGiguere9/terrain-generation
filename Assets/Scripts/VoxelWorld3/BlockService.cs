using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using VoxelWorld2.Blocks;

namespace VoxelWorld3
{
	public class BlockService
	{
		#region Block Geometry Order

		/// <summary>
		/// Position of each block vertex
		/// </summary>
		private static readonly Vector3Int[] VertexOrder =
		{
			new Vector3Int(0, 0, 0),
			new Vector3Int(1, 0, 0),
			new Vector3Int(1, 0, 1),
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 1, 0),
			new Vector3Int(1, 1, 0),
			new Vector3Int(1, 1, 1),
			new Vector3Int(0, 1, 1)
		};

		/// <summary>
		/// The 6 face normals of the block
		/// </summary>
		private static readonly Vector3Int[] FaceOrder =
		{
			Vector3Int.right,
			Vector3Int.left,
			Vector3Int.up,
			Vector3Int.down,
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 0, -1)
		};

		/// <summary>
		/// The 4 vertices forming a face of a block.
		/// The first dimension corresponds to the index of the corresponding face, and the second the indexes of its vertices.
		/// Face index order matches <see cref="Faces"/>.
		/// Vertex index order matches <see cref="Vertices"/>.
		/// </summary>
		private static readonly byte[][] FaceVertexOrder =
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
		private static readonly byte[] FaceTriangleOrder = { 0, 1, 2, 0, 2, 3 };

		#endregion

		private static BlockService _instance;
		public static BlockService Instance => _instance ?? (_instance = new BlockService());

		private static Dictionary<byte, BlockModel> _blocks;

		private BlockService()
		{
			LoadBlocks("blocks");
		}

		public void LoadBlocks(string path)
		{
			if (_blocks != null) return;

			TextAsset res = Resources.Load(path) as TextAsset;
			BlocksConfig config = JsonUtility.FromJson<BlocksConfig>(res.text);
			_blocks = config.Blocks.ToDictionary(b => b.Id, b => b);
		}

		/// <summary>
		/// The 6 face normals of the block
		/// </summary>
		public Vector3Int[] GetFaceOrder() => FaceOrder;

		public byte[] GetFaceTriangleOrder() => FaceTriangleOrder;

		/// <summary>
		/// Shorthand for obtaining vertices of a face in world position
		/// </summary>
		/// <param name="worldPosition">world position of the block</param>
		/// <param name="faceIndex">face index of the block</param>
		/// <returns></returns>
		public Vector3[] GetFaceVertices(Vector3 worldPosition, int faceIndex)
		{
			var verts = VertexOrder;
			var faceVerts = FaceVertexOrder;

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
			int ti = _blocks[blockId].TextureIndexes[faceIndex];
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

		[CanBeNull]
		public BlockModel GetBlockModel(byte blockId)
		{
			return _blocks.TryGetValue(blockId, out BlockModel value) ? value : null;
		}
	}
}