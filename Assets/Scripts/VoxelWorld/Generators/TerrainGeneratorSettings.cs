using UnityEngine;

namespace VoxelWorld.Generators
{
	[CreateAssetMenu(fileName = "TerrainGeneratorSettings", menuName = "Game Settings/Terrain Generator Settings", order = 100)]
	public class TerrainGeneratorSettings : ScriptableObject
	{
		private static TerrainGeneratorSettings _instance;
		public static TerrainGeneratorSettings Instance => _instance == null
			? _instance = Resources.Load<TerrainGeneratorSettings>("TerrainGeneratorSettings")
			: _instance;



		[Tooltip("Terrain generator to use")]
		public TerrainGeneratorType TerrainGeneratorType;

		public int Seed;

		public float Scale = 0.005f;

		[Tooltip("Noise sample coordinates get offset by this amount")]
		public Vector3 Offset;

		[Tooltip("Number of noise layers")]
		[Range(1, 8)]
		public int Octaves = 3;

		[Tooltip("Decreases amplitude of octaves. amp=per^oct")]
		[Range(0, 1)]
		public float Persistence = 0.25f;

		[Tooltip("Increases the frequency of octaves. freq=lac^oct")]
		[Range(1, 16)]
		public float Lacunarity = 4f;

		[Tooltip("Noise threshold as a function of height. " +
		         "Noise sampled at a y value corresponding to a percentage of the terrain's y size " +
		         "must be above the specified threshold to spawn a block.")]
		public AnimationCurve Density;

		[Tooltip("Reference value for the height map")]
		public int BaselineHeight = 32;

		[Tooltip("If terrain height is below this value, block up to this value will be filled by water")]
		public int WaterLevel = 25;

		[Tooltip("Scale of the biome map")]
		public float BiomeScale = 0.001f;

		public float CavernScale = 0.05f;
	}
}