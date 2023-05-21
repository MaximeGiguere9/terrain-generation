namespace VoxelWorld.Blocks
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
		/// How to render the block texture. See <see cref="RenderType"/>.
		/// </summary>
		public RenderType RenderType;
		/// <summary>
		/// When two blocks with the same id are next to each other,
		/// hide the face that connects them and render them as a single blob.
		/// </summary>
		public bool HideConnectingFaces;
		/// <summary>
		/// Whether or not his block should have collision.
		/// </summary>
		public bool Solid;
	}

	/// <summary>
	/// Used for deserialization
	/// </summary>
	[System.Serializable]
	public class BlocksConfig
	{
		public BlockModel[] Blocks;
	}

	public enum RenderType
	{
		/// <summary>
		/// Texture is entirely opaque and does not need to render blocks behind it
		/// </summary>
		Opaque = 0,
		/// <summary>
		/// Texture uses alpha cutout and needs to render blocks behind it
		/// </summary>
		Cutout = 1,
		/// <summary>
		/// Texture uses transparency and needs to render blocks behind it
		/// </summary>
		Transparent = 2
	}
}