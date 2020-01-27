﻿using UnityEngine;

namespace VoxelWorld
{
	public class Block
	{
		public enum BlockTypes
		{
			Stone,
			Dirt,
			Grass
		}

		public static byte[] Bits = { 1, 2, 4, 8, 16, 32, 64, 128 };

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
		/// The four vertices forming a face, in the same orders as Vertices and Faces
		/// </summary>
		private static readonly byte[][] VertexVisibilityLookup =
		{
			new byte[] {1, 2, 5, 6},
			new byte[] {0, 3, 4, 7},
			new byte[] {4, 5, 6, 7},
			new byte[] {0, 1, 2, 3},
			new byte[] {2, 3, 6, 7},
			new byte[] {0, 1, 4, 5}
		};

		/// <summary>
		/// The vertices to use to form the two triangles of a face, in the same order as Faces
		/// </summary>
		public static readonly byte[][] TrianglesTable =
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
		public bool IsVisible => this.VisibleFaces != 0;

		public BlockTypes BlockType = BlockTypes.Stone;

		public void UpdateVisibility()
		{
			this.VisibleFaces = 0;

			for(int i = 0; i < Faces.Length; i++)
			{
				bool visible = ChunkManager.GetBlockAt(this.Position + Faces[i]) == null;

				if (!visible) continue;

				this.VisibleFaces |= Bits[i];
			}
		}
	}
}