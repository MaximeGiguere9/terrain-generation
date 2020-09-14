namespace VoxelWorld2.Chunks
{
	[System.Serializable]
	public class ChunkModel
	{
		/// <summary>
		/// Cubic size of the chunk
		/// </summary>
		public byte Size;
		/// <summary>
		/// xyz components of the chunk position in chunk space
		/// </summary>
		public int[] Position;
		/// <summary>
		/// Buffer containing ids of blocks in the chunk.
		/// Length should be Size^3
		/// </summary>
		public byte[] Blocks;
	}
}