using UnityEngine;

namespace InsaneSystems.Radar
{
	public class DemoHelper : MonoBehaviour
	{
		RadarSystem radarSystem;
		
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				if (!radarSystem)
					radarSystem = FindObjectOfType<RadarSystem>();

				radarSystem.settings.rotateRadar = !radarSystem.settings.rotateRadar;
			}

			if (Input.GetKeyDown(KeyCode.L))
			{
				if (Cursor.visible)
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false; 
				}
				else
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
			}
		}
	}
}