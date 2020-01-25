using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
	[SerializeField] private GameObject blockPrefab;
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

		for (int y = 0; y < size.y; y++)
		{
			for (int z = 0; z < size.z; z++)
			{
				for (int x = 0; x < size.x; x++)
				{
					float amplitude = 1;
					float frequency = 1;
					float noise = 0;

					for (int i = 0; i < this.octaves; i++)
					{
						float val = SimplePerlin3D.Sample(x * scale * frequency + octaveOffsets[i].x, y * scale * frequency + octaveOffsets[i].y, z * scale * frequency + octaveOffsets[i].z) * 2 - 1;
						noise += val * amplitude;

						amplitude *= persistance;
						frequency *= lacunarity;
					}

					if (noise < minValue)
						minValue = noise;

					if (noise > maxValue)
						maxValue = noise;

					this.noiseMap[x, y, z] = noise;
				}
			}
		}

		for (int y = 0; y < size.y; y++)
		{
			for (int z = 0; z < size.z; z++)
			{
				for (int x = 0; x < size.x; x++)
				{
					this.noiseMap[x, y, z] = Mathf.InverseLerp(minValue, maxValue, this.noiseMap[x, y, z]);
					if (this.noiseMap[x, y, z] < this.density.Evaluate((float) y / size.y))
						Instantiate(this.blockPrefab, new Vector3(x, y, z), Quaternion.identity, transform);
				}
			}
		}
	}
}
