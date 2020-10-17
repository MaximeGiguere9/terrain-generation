using UnityEngine;
using Utils;
using VoxelWorld3.Blocks;

namespace VoxelWorld3.World
{
	public class BlockTargeterBehaviour : MonoBehaviour
	{
		[SerializeField] private Transform cameraTransform;
		[SerializeField] private float maxTargetRange = 4;

		private void Update()
		{
			BlockTargeter.HitPoint targetHit = BlockTargeter.Target(
				this.cameraTransform.position,
				this.cameraTransform.forward,
				this.maxTargetRange,
				pos => WorldService.Instance.GetBlockAt(pos),
				BlockService.Instance
			);

			if (targetHit == null) return;

			if (Input.GetButtonDown("Fire1"))
			{
				WorldService.Instance.SetBlockAt(targetHit.Position, 0);
			}
			else if (Input.GetButtonDown("Fire2"))
			{
				WorldService.Instance.SetBlockAt(targetHit.Position + targetHit.Normal, 6);
			}
		}

	}
}