namespace VoxelWorld2.Blocks
{
	[System.Serializable]
	public class BlockModel
	{
		public byte Id;
		public string Name;
		/// <summary>
		/// Index of textures in the atlas, for each face.
		/// </summary>
		public int[] TextureIndexes;
		/// <summary>
		/// If the block is transparent, it should not hide other blocks behind it.
		/// </summary>
		public bool Transparent;
		/// <summary>
		/// When two blocks with the same id are next to each other,
		/// hide the face that connects them and render them as a single blob.
		/// </summary>
		public bool HideConnectingFaces;
	}

	/// <summary>
	/// Used for deserialization
	/// </summary>
	[System.Serializable]
	public class BlocksConfig
	{
		public BlockModel[] Blocks;
	}
}