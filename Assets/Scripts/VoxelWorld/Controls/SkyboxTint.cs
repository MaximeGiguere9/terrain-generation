using UnityEngine;
using VoxelWorld.World;

namespace VoxelWorld.Controls
{
	public class SkyboxTint : MonoBehaviour
	{
		[SerializeField] private Camera mainCamera;

		public void Update()
		{
			Vector3 cameraPos = this.mainCamera.gameObject.transform.position;

			byte? blockId = WorldService.Instance.GetBlockAt(new Vector3Int(Mathf.FloorToInt(cameraPos.x), Mathf.FloorToInt(cameraPos.y), Mathf.FloorToInt(cameraPos.z)));

			if (blockId.HasValue && blockId.Value == 7)
			{
				RenderSettings.fog = true;
				RenderSettings.fogDensity = 0.1f;
				RenderSettings.fogColor = new Color(64 / 255f, 120 / 255f, 180 / 255f, 1);
			}
			else
			{
				RenderSettings.fog = false;
			}
		}

	}
}