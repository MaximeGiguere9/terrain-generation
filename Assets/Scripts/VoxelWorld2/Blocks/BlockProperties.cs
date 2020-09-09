using UnityEngine;

namespace VoxelWorld2.Blocks
{
	public static class BlockProperties
	{
		/// <summary>
		/// Position of each block vertex
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
			new Vector3Int(0, 1, 1)
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
		/// The 4 vertices forming a face of a block.
		/// The first dimension corresponds to the index of the corresponding face, and the second the indexes of its vertices.
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
		public static readonly byte[] FaceTriangles = { 0, 1, 2, 0, 2, 3 };
	}
}