using UnityEngine;

namespace VoxelWorld.Terrain
{
	public class VoxelBlock
	{
		private static readonly byte[] Bits = { 1, 2, 4, 8, 16, 32, 64, 128 };

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
		/// The vertices to use to form the two triangles of a face, in the same order as Faces
		/// </summary>
		public static readonly byte[][] Triangles =
		{
			new byte[] {6, 1, 5, 6, 2, 1},
			new byte[] {0, 3, 7, 4, 0, 7},
			new byte[] {6, 4, 7, 6, 5, 4},
			new byte[] {0, 1, 2, 3, 0, 2},
			new byte[] {7, 2, 6, 7, 3, 2},
			new byte[] {5, 0, 4, 5, 1, 0}
		};

		public Vector3Int Position { get; set; }

		/// <summary>
		/// Bit field of visible faces, in the same order as Faces
		/// </summary>
		public byte VisibleFaces { get; private set; }

		public VoxelBlockTypes BlockType = VoxelBlockTypes.Stone;

		public void UpdateVisibility()
		{
			this.VisibleFaces = 0;

			for(int i = 0; i < Faces.Length; i++)
			{
				bool visible = VoxelTerrain.ActiveTerrain.GetBlockAt(this.Position + Faces[i]) == null;

				if (!visible) continue;

				this.VisibleFaces |= Bits[i];
			}
		}

		public bool IsFaceVisible(int faceIndex) => (this.VisibleFaces | Bits[faceIndex]) == this.VisibleFaces;
	}
}