using System.Linq;
using UnityEngine;

namespace VoxelWorld.Blocks
{
	public class BlockService : IBlockService
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

		private static readonly int[][] VertexOrderArr =
		{
			new[] {0, 0, 0},
			new[] {1, 0, 0},
			new[] {1, 0, 1},
			new[] {0, 0, 1},
			new[] {0, 1, 0},
			new[] {1, 1, 0},
			new[] {1, 1, 1},
			new[] {0, 1, 1}
		};

		/// <summary>
		/// The 6 face normals of the block
		/// </summary>
		public static readonly Vector3Int[] FaceOrder =
		{
			Vector3Int.right,
			Vector3Int.left,
			Vector3Int.up,
			Vector3Int.down,
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 0, -1)
		};

		public static readonly int[][] FaceOrderArr =
		{
			new[] {1, 0, 0},
			new[] {-1, 0, 0},
			new[] {0, 1, 0},
			new[] {0, -1, 0},
			new[] {0, 0, 1},
			new[] {0, 0, -1}
		};

		/// <summary>
		/// The 4 vertices forming a face of a block.
		/// The first dimension corresponds to the index of the corresponding face, and the second the indexes of its vertices.
		/// Face index order matches <see cref="Faces"/>.
		/// Vertex index order matches <see cref="Vertices"/>.
		/// </summary>
		public static readonly byte[][] FaceVertexOrder =
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
		public static readonly byte[] FaceTriangleOrder = { 0, 1, 2, 0, 2, 3 };

		#endregion

		private static BlockService _instance;
		public static BlockService Instance => _instance ?? (_instance = new BlockService());

		private static BlockModel[] _blocks;

		private BlockService()
		{
			LoadBlocks("blocks");
		}

		public void LoadBlocks(string path)
		{
			if (_blocks != null) return;

			TextAsset res = Resources.Load(path) as TextAsset;
			BlocksConfig config = JsonUtility.FromJson<BlocksConfig>(res.text);

			_blocks = new BlockModel[config.Blocks.Max(b => b.Id) + 1];
			foreach (var block in config.Blocks)
			{
				_blocks[block.Id] = block;
			}
		}

		public Vector3Int[] GetVertexOrder() => VertexOrder;

		/// <summary>
		/// The 6 face normals of the block
		/// </summary>
		public Vector3Int[] GetFaceOrder() => FaceOrder;

		public byte[][] GetFaceVertexOrder() => FaceVertexOrder;

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

		public Vector3[] GetFaceVerticesArrayBuffer()
		{
			return new Vector3[4];
		}

		public void GetFaceVertices(in Vector3 worldPosition, in int faceIndex, ref Vector3[] arrayBuffer)
		{
			arrayBuffer[0] = worldPosition + VertexOrder[FaceVertexOrder[faceIndex][0]];
			arrayBuffer[1] = worldPosition + VertexOrder[FaceVertexOrder[faceIndex][1]];
			arrayBuffer[2] = worldPosition + VertexOrder[FaceVertexOrder[faceIndex][2]];
			arrayBuffer[3] = worldPosition + VertexOrder[FaceVertexOrder[faceIndex][3]];
		}

		public void GetFaceVertices(in int x, in int y, in int z, in int faceIndex, ref Vector3[] arrayBuffer)
		{
			arrayBuffer[0].x = x + VertexOrderArr[FaceVertexOrder[faceIndex][0]][0];
			arrayBuffer[0].y = y + VertexOrderArr[FaceVertexOrder[faceIndex][0]][1];
			arrayBuffer[0].z = z + VertexOrderArr[FaceVertexOrder[faceIndex][0]][2];

			arrayBuffer[1].x = x + VertexOrderArr[FaceVertexOrder[faceIndex][1]][0];
			arrayBuffer[1].y = y + VertexOrderArr[FaceVertexOrder[faceIndex][1]][1];
			arrayBuffer[1].z = z + VertexOrderArr[FaceVertexOrder[faceIndex][1]][2];

			arrayBuffer[2].x = x + VertexOrderArr[FaceVertexOrder[faceIndex][2]][0];
			arrayBuffer[2].y = y + VertexOrderArr[FaceVertexOrder[faceIndex][2]][1];
			arrayBuffer[2].z = z + VertexOrderArr[FaceVertexOrder[faceIndex][2]][2];

			arrayBuffer[3].x = x + VertexOrderArr[FaceVertexOrder[faceIndex][3]][0];
			arrayBuffer[3].y = y + VertexOrderArr[FaceVertexOrder[faceIndex][3]][1];
			arrayBuffer[3].z = z + VertexOrderArr[FaceVertexOrder[faceIndex][3]][2];
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

		public Vector2[] GetFaceUVsArrayBuffer()
		{
			return new[]
			{
				new Vector2(),
				new Vector2(),
				new Vector2(),
				new Vector2()
			};
		}

		public void GetFaceUVs(byte blockId, int faceIndex, ref Vector2[] arrayBuffer)
		{
			int ti = _blocks[blockId].TextureIndexes[faceIndex];
			int x = ti % 16;
			int y = ti / 16;

			arrayBuffer[0].x = x / 16f;
			arrayBuffer[0].y = y / 16f;

			arrayBuffer[1].x = (x + 1) / 16f;
			arrayBuffer[1].y = y / 16f;

			arrayBuffer[2].x = (x + 1) / 16f;
			arrayBuffer[2].y = (y + 1) / 16f;

			arrayBuffer[3].x = x / 16f;
			arrayBuffer[3].y = (y + 1) / 16f;
		}

		public BlockModel GetBlockModel(int blockId)
		{
			return _blocks[blockId];
		}

		public Vector3 GetBlockSize()
		{
			return Vector3Int.one;
		}
	}
}