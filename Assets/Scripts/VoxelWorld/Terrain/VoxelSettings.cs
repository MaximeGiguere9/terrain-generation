using UnityEngine;

namespace VoxelWorld.Terrain
{
	[CreateAssetMenu(fileName = "VoxelSettings", menuName = "Game Settings/Voxel World Settings", order = 100)]
	public class VoxelSettings : ScriptableObject
	{
		private static VoxelSettings _instance;
		public static VoxelSettings Instance => _instance == null
			? _instance = Resources.Load<VoxelSettings>("VoxelSettings")
			: _instance;


		[Header("World Settings")]

		public GameObject ChunkPrefab;

		[Tooltip("Cubic size of chunks. Default is 16x16x16")]
		public int ChunkSize = 16;

		[Tooltip("World dimensions in finite generation")]
		public Vector3Int WorldSize;

		[Tooltip("Reference value for height in infinite generation")]
		public int BaselineHeight = 32;

		[Tooltip("If terrain height is below this value, block up to this value will be filled by water")]
		public int WaterLevel = 32;

		[Tooltip("Scale of the biome map")]
		public float BiomeScale = 1;

		[Header("Noise Settings")]

		public int Seed;

		public float Scale = 0.005f;

		[Tooltip("Noise sample coordinates get offset by this amount")]
		public Vector3 Offset;

		[Tooltip("Number of noise layers")]
		[Range(1, 8)]
		public int Octaves = 3;

		[Tooltip("Decreases amplitude of octaves. amp=per^oct")]
		[Range(0, 1)]
		public float Persistence = 0.5f;

		[Tooltip("Increases the frequency of octaves. freq=lac^oct")]
		[Range(1, 16)]
		public float Lacunarity = 2f;

		[Tooltip("Noise threshold as a function of height. " +
		         "Noise sampled at a y value corresponding to a percentage of the terrain's y size " +
		         "must be above the specified threshold to spawn a block.")]
		public AnimationCurve Density;
	}
}