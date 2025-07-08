using UnityEngine;
using UnityEngine.UI;

namespace InsaneSystems.Radar.UI
{
	public class Hint : MonoBehaviour
	{ 
		public RadarObject shownForRadarObject { get; protected set; }
		
		[SerializeField] Text nameText;

		RectTransform rectTransform;

		void Start()
		{
			rectTransform = GetComponent<RectTransform>();

			Hide();
		}

		private void Update()
		{
			if (shownForRadarObject != null)
				rectTransform.anchoredPosition = shownForRadarObject.SelfIconTransform.position;
		}

		public void ShowForObject(RadarObject radarObject)
		{
			string objectName = radarObject.Label;

			if (objectName == "" && RadarSystem.sceneSingleton.settings.showGameObjectNameIfRadarObjectLabelEmpty)
				objectName = radarObject.name;

			if (objectName == "")
				return;

			gameObject.SetActive(true);

			shownForRadarObject = radarObject;
			rectTransform.anchoredPosition = radarObject.SelfIconTransform.position;
			nameText.text = objectName;
		}

		public void Hide()
		{
			shownForRadarObject = null;
			gameObject.SetActive(false);
		}
	}
}