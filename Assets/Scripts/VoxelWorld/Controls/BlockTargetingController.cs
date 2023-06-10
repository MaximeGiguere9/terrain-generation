using UnityEngine;
using VoxelWorld.World;

namespace VoxelWorld.Controls
{
	public class BlockTargetingController : MonoBehaviour
	{
		[SerializeField] private Transform cameraTransform;
		[SerializeField] private float maxTargetRange = 4;
		[SerializeField] private GameObject targetingIndicator;

		private readonly IFirstPersonInputHandler inputHandler = new UnityInputHandler();
		private IBlockTargeter blockTargeter;

		private Vector3Int hitPosition;
		private Vector3Int hitNormal;

		private void Awake()
		{
			blockTargeter = new RayBoxIntersectionTargeter(pos => WorldService.Instance.GetBlockAt(pos));
		}

		private void Update()
		{
			bool hit = blockTargeter.Target(
				this.cameraTransform.position,
				this.cameraTransform.forward,
				this.maxTargetRange,
				ref this.hitPosition,
				ref this.hitNormal
			);

			if (!hit)
			{
				this.targetingIndicator.SetActive(false);
				return;
			}

			// the hit position is the position of the block (min pos in world coords)
			// move the targeting indicator to the middle of the block face hit and activate it
			Vector3 centerBlockPosition = hitPosition + Vector3.one / 2;
			this.targetingIndicator.transform.position = centerBlockPosition + (Vector3) hitNormal* 0.51f;
			this.targetingIndicator.transform.LookAt(centerBlockPosition);
			this.targetingIndicator.SetActive(true);

			if (inputHandler.GetBreakBlock())
			{
				// break block
				WorldService.Instance.SetBlockAt(hitPosition, 0);
			}
			else if (inputHandler.GetPlaceBlock())
			{
				// place block
				WorldService.Instance.SetBlockAt(hitPosition + hitNormal, 6);
			}
		}
	}
}