using System.Linq;
using UnityEngine;

namespace VoxelWorld.Blocks
{
	public class BlockService
	{
		public static string BLOCKS_CONFIG_RESOURCE_PATH = "blocks";

		private static BlockService _instance;
		public static BlockService Instance => _instance ??= new BlockService();

		private BlockModel[] blocks;

		private BlockService() 
		{
			LoadBlocks(BLOCKS_CONFIG_RESOURCE_PATH);
		}

		public void LoadBlocks(string path)
		{
			if (blocks != null) return;

			TextAsset res = Resources.Load(path) as TextAsset;
			BlocksConfig config = JsonUtility.FromJson<BlocksConfig>(res.text);

			blocks = new BlockModel[config.Blocks.Max(b => b.Id) + 1];
			foreach (var block in config.Blocks)
			{
				blocks[block.Id] = block;
			}
		}

		public ref BlockModel GetBlockModel(int blockId)
		{
			return ref blocks[blockId];
		}
	}
}