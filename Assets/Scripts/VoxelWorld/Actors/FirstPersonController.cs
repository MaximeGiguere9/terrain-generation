using UnityEngine;
using VoxelWorld.Terrain;

namespace VoxelWorld.Actors
{
	public class FirstPersonController : MonoBehaviour
	{
		[SerializeField] private Transform cameraTransform;
		[SerializeField] private CharacterController characterController;
		[SerializeField] private float sensitivity = 100;
		[SerializeField] private float maxPitch = 80;
		[SerializeField] private float movementSpeed = 5;
		[SerializeField] private float maxTargetRange = 3;

		private float pitch;
		public float Pitch
		{
			get => pitch;
			set => pitch = Mathf.Clamp(value, -this.maxPitch, this.maxPitch);
		}

		private float yaw;
		public float Yaw
		{
			get => this.yaw;
			set => this.yaw = value % 360;
		}

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			Move();
			Target();
		}

		private void Move()
		{
			this.Yaw += Input.GetAxis("Mouse X") * this.sensitivity * Time.deltaTime;
			this.Pitch += Input.GetAxis("Mouse Y") * this.sensitivity * Time.deltaTime;

			Vector3 movementInput = Quaternion.AngleAxis(this.Yaw, Vector3.up) *
			                        new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

			movementInput.y = Input.GetButton("Jump") ? 1 : Input.GetButton("Fire3") ? -1 : 0;

			this.characterController.transform.localEulerAngles = new Vector3(0, this.Yaw, 0);
			this.cameraTransform.localEulerAngles = new Vector3(-this.Pitch, 0, 0);

			this.characterController.Move(movementInput * this.movementSpeed * Time.deltaTime);
		}

		private void Target()
		{
			Ray ray = new Ray(this.cameraTransform.position, this.cameraTransform.forward);

			Debug.DrawRay(ray.origin, ray.direction * this.maxTargetRange, Color.blue);
			bool isHit = Physics.Raycast(ray, out RaycastHit hit, this.maxTargetRange);

			if (!isHit) return;

			Debug.DrawRay(hit.point, hit.normal, Color.blue);
			Vector3Int blockPosition = Vector3Int.FloorToInt(hit.point - hit.normal/2);

			if (Input.GetButtonDown("Fire1"))
				VoxelTerrain.ActiveTerrain.RemoveBlockAt(blockPosition);
		}
	}
}
