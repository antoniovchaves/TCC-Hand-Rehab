using UnityEngine;
using UnityEngine.UI;

namespace InsaneSystems.Radar.UI
{
	public class RadarColorizer : MonoBehaviour
	{
		[SerializeField] Graphic[] interfaceElementsToColorize;

		void Start()
		{
			var interfaceColor = RadarSystem.sceneSingleton.settings.customPrimaryUIColor;

			for (int i = 0; i < interfaceElementsToColorize.Length; i++)
				interfaceElementsToColorize[i].color = interfaceColor;
		}
	}
}