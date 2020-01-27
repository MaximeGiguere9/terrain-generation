using System.Collections;
using Noise;
using UnityEngine;

namespace VoxelWorld
{
	public class VoxelTerrainGenerator : MonoBehaviour
	{
		[SerializeField] private Vector3Int size;
		[SerializeField] private int seed;
		[SerializeField] private float scale = 0.09f;

		[Tooltip("Noise sample coordinates get offset by this amount")]
		[SerializeField] private Vector3 offset;

		[Tooltip("Number of noise layers")]
		[SerializeField] private int octaves;

		[Tooltip("Decreases amplitude of octaves. amp=per^oct")]
		[Range(0,1)]
		[SerializeField] private float persistance = 0.5f;

		[Tooltip("Increases the frequency of octaves. freq=lac^oct")]
		[Range(1,16)]
		[SerializeField] private float lacunarity = 2f;

		[Tooltip("Noise threshold as a function of height. " +
		         "Noise sampled at a y value corresponding to a percentage of the terrain's y size " +
		         "must be above the specified threshold to spawn a block.")]
		[SerializeField] private AnimationCurve density;

		private readonly INoiseSampler noiseSampler = new SimplePerlin3D();

		private float[,,] noiseMap;

		private void Start()
		{
			System.Random rng = new System.Random(this.seed);
			Vector3[] octaveOffsets = new Vector3[octaves];
			for (int i = 0; i < octaves; i++)
				octaveOffsets[i] = new Vector3(rng.Next(-100000, 100000) + offset.x, rng.Next(-100000, 100000) + offset.y, rng.Next(-100000, 100000) + offset.z);

			float minValue = float.MaxValue;
			float maxValue = float.MinValue;

			this.noiseMap = new float[size.x, size.y, size.z];

			CoordinateIterator iterator = new CoordinateIterator(this.size, Vector3Int.zero);

			foreach (Vector3Int pos in iterator)
			{
				float amplitude = 1;
				float frequency = 1;
				float noise = 0;

				for (int i = 0; i < this.octaves; i++)
				{
					Vector3 samplePos = (Vector3) pos * scale * frequency + octaveOffsets[i];

					float val = this.noiseSampler.Sample(samplePos.x, samplePos.y, samplePos.z) * 2 - 1;
					noise += val * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noise < minValue)
					minValue = noise;

				if (noise > maxValue)
					maxValue = noise;

				this.noiseMap[pos.x, pos.y, pos.z] = noise;
			}

			iterator.Reset();

			foreach (Vector3Int pos in iterator)
			{
				int x = pos.x;
				int y = pos.y;
				int z = pos.z;

				this.noiseMap[x, y, z] = Mathf.InverseLerp(minValue, maxValue, this.noiseMap[x, y, z]);
				if (this.noiseMap[x, y, z] > this.density.Evaluate((float) y / size.y)) continue;

				Block block = new Block();
				block.Position = new Vector3Int(x,y,z);
				ChunkManager.AddBlock(block);
			}

			StartCoroutine(Redraw());
		}

		private IEnumerator Redraw()
		{
			yield return new WaitForEndOfFrame();
			ChunkManager.RedrawAll();
		}
	}
}
