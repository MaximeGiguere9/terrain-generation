using UnityEngine;

namespace VoxelWorld
{
	[CreateAssetMenu(fileName = "VoxelWorldSettings", menuName = "Game Settings/Voxel World Settings", order = 100)]
	public class VoxelWorldSettings : ScriptableObject
	{
		private static VoxelWorldSettings _instance;
		public static VoxelWorldSettings Instance => _instance == null
			? _instance = Resources.Load<VoxelWorldSettings>("VoxelWorldSettings")
			: _instance;

		public GameObject ChunkPrefab;
		public int ChunkSize;
	}
}