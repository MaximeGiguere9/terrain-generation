using UnityEngine;

namespace Utils
{
	public class FirstPersonController : MonoBehaviour
	{
		[SerializeField] private Transform cameraTransform;
		[SerializeField] private CharacterController characterController;
		[SerializeField] private float sensitivity = 100;
		[SerializeField] private float maxPitch = 80;
		[SerializeField] private float movementSpeed = 10;

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

			this.characterController.Move(movementInput * this.movementSpeed * Time.deltaTime * (Input.GetButton("Fire2") ? 5 : 1));
		}
	}
}
