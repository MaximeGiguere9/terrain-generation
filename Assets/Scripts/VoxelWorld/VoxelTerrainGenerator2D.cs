using Noise;
using System.Collections;
using UnityEngine;
using VoxelWorld.Terrain;
using VoxelWorld.Utils;

namespace VoxelWorld
{
	public class VoxelTerrainGenerator2D : MonoBehaviour
	{
		[SerializeField] private Vector3Int size;
		[SerializeField] private int seed;
		[SerializeField] private float scale = 0.09f;

		[Tooltip("Noise sample coordinates get offset by this amount")]
		[SerializeField] private Vector3 offset;

		[Tooltip("Number of noise layers")]
		[SerializeField] private int octaves;

		[Tooltip("Decreases amplitude of octaves. amp=per^oct")]
		[Range(0, 1)]
		[SerializeField] private float persistance = 0.5f;

		[Tooltip("Increases the frequency of octaves. freq=lac^oct")]
		[Range(1, 16)]
		[SerializeField] private float lacunarity = 2f;

		private readonly NoiseSampler noiseSampler = new SimplePerlin2D();

		private readonly  NoiseSampler varianceSampler = new SimplePerlin2D();

		private float[,] noiseMap;

		private void Start()
		{
			System.Random rng = new System.Random(this.seed);
			Vector3[] octaveOffsets = new Vector3[octaves];
			for (int i = 0; i < octaves; i++)
				octaveOffsets[i] = new Vector3(rng.Next(-100000, 100000) + offset.x, rng.Next(-100000, 100000) + offset.y, rng.Next(-100000, 100000) + offset.z);

			float minValue = float.MaxValue;
			float maxValue = float.MinValue;

			this.noiseMap = new float[size.x, size.z];

			CoordinateIterator iterator = new CoordinateIterator(new Vector3Int(this.size.x, 1, this.size.z), Vector3Int.zero);

			foreach (Vector3Int pos in iterator)
			{
				float amplitude = 1;
				float frequency = 1;
				float noise = 0;

				for (int i = 0; i < this.octaves; i++)
				{
					Vector3 samplePos = (Vector3)pos * scale * frequency + octaveOffsets[i];

					float val = this.noiseSampler.Sample(samplePos.x, samplePos.y, samplePos.z) * 2 - 1;
					noise += val * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noise < minValue)
					minValue = noise;

				if (noise > maxValue)
					maxValue = noise;

				this.noiseMap[pos.x, pos.z] = noise;
			}

			iterator.Reset();

			foreach (Vector3Int pos in iterator)
			{
				int x = pos.x;
				int z = pos.z;

				this.noiseMap[x, z] = Mathf.InverseLerp(minValue, maxValue, this.noiseMap[x, z]);

				float varianceMult = this.noiseMap[x, z];

				float variance = this.size.y * varianceMult;

				int height = Mathf.FloorToInt((this.noiseMap[x, z]*2 - 1) * variance + Mathf.Max(this.size.y - variance, 0));

				for (int y = 0; y < height; y++)
				{
					VoxelBlock block = new VoxelBlock();
					block.Position = new Vector3Int(x, y, z);
					VoxelTerrain.ActiveTerrain.AddBlock(block);
				}

			}

			StartCoroutine(Redraw());
		}

		private IEnumerator Redraw()
		{
			yield return new WaitForEndOfFrame();
			VoxelTerrain.ActiveTerrain.RedrawAll();
		}
	}
}
