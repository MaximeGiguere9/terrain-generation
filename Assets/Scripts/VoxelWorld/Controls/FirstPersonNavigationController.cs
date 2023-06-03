using UnityEngine;

namespace VoxelWorld.Controls
{
	public class FirstPersonNavigationController : MonoBehaviour
	{
		[SerializeField] private Transform cameraTransform;
		[SerializeField] private CharacterController characterController;
		[SerializeField] private float aimSensitivity = 100;
		[SerializeField] private float maxPitch = 80;
		[SerializeField] private float movementSpeed = 10;

		private IFirstPersonInputHandler inputHandler = new UnityInputHandler();

		private float pitch;
		public float Pitch
		{
			get => pitch;
			set => pitch = Mathf.Clamp(value, -maxPitch, maxPitch);
		}

		private float yaw;
		public float Yaw
		{
			get => yaw;
			set => yaw = value % 360;
		}

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			Yaw += inputHandler.GetAimX() * aimSensitivity * Time.deltaTime;
			Pitch += inputHandler.GetAimY() * aimSensitivity * Time.deltaTime;

			Vector3 movementInput = Quaternion.AngleAxis(Yaw, Vector3.up) *
									new Vector3(inputHandler.GetMoveX(), 0, inputHandler.GetMoveY());

			movementInput.y = inputHandler.GetJump() ? 1 : inputHandler.GetCrouch() ? -1 : 0;

			characterController.transform.localEulerAngles = new Vector3(0, Yaw, 0);
			cameraTransform.localEulerAngles = new Vector3(-Pitch, 0, 0);

			characterController.Move(movementInput * movementSpeed * Time.deltaTime * (inputHandler.GetSprint() ? 5 : 1));
		}
	}
}
