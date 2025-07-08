using UnityEngine;

namespace InsaneSystems.Radar
{
	public class SimplePlayer : MonoBehaviour
	{
		[SerializeField] float movingSpeed = 2f;
		[SerializeField] bool is2D;
		[SerializeField] Transform cameraTransform; 

		public void Update()
		{
			float x = Input.GetAxis("Horizontal");
			float y = Input.GetAxis("Vertical");
		
			float increaseSpeed = Input.GetKey(KeyCode.LeftShift) ? 2.5f : 1f;

			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");

			if (is2D)
			{
				transform.position += (transform.up * y + transform.right * x) * Time.deltaTime * movingSpeed * increaseSpeed;

				transform.localEulerAngles += new Vector3(0, 0, -mouseX);
			}
			else
			{
				transform.position += (transform.forward * y + transform.right * x) * Time.deltaTime * movingSpeed * increaseSpeed;

				transform.localEulerAngles += new Vector3(-mouseY, mouseX);
			}
		}

		public void LateUpdate()
		{
			if (cameraTransform && is2D)
			{
				cameraTransform.eulerAngles = Vector3.zero;
			}
		}
	}
}