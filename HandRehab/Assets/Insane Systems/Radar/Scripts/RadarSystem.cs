using UnityEngine;

namespace InsaneSystems.Radar
{
	public class RadarSystem : MonoBehaviour
	{
		public static RadarSystem sceneSingleton { get; protected set; }

		[SerializeField] RadarSettings radarSettings;
		[SerializeField] UI.RadarDrawer radarDrawer;
		
		[SerializeField] [Range(0f, 1f)] float pauseBetweenUpdates = 0.1f;

		public RadarObject centerObject { get; set; }
		public RadarSettings settings { get { return radarSettings; } }
		public UI.Hint hint { get; protected set; }
		
		float timeToNextUpdate;

		public float pauseBetweenUpdatesValue { get { return pauseBetweenUpdates; } }
		public UI.RadarDrawer RadarDrawer {	get { return radarDrawer; }	}

		void Awake()
		{
			sceneSingleton = this;

			hint = FindObjectOfType<UI.Hint>();

			if (!hint)
				Debug.LogWarning("[RadarSystem] No Hint setted up into RadarSystem. Please, setup it, otherwise hints will be not shown.");
			
			if (!radarDrawer)
			{
				radarDrawer = FindObjectOfType<UI.RadarDrawer>();

				if (!radarDrawer)
				{
					Debug.LogWarning("[RadarSystem] No RadarDrawer setted up into RadarSystem. Please, setup it and restart scene.");
					enabled = false;
				}
			}
		}

		void Update()
		{
			UpdateActualRadarObjects();
		}

		void UpdateActualRadarObjects()
		{
			for (int i = RadarObject.allRadarObjects.Count - 1; i >= 0; i--)
			{
				if (!RadarObject.allRadarObjects[i])
				{
					RadarObject.allRadarObjects.RemoveAt(i);
					continue;
				}

				radarDrawer.UpdateIconStickedPositionAndRotation(RadarObject.allRadarObjects[i]);
			}

			if (timeToNextUpdate > 0)
			{
				timeToNextUpdate -= Time.deltaTime;
				return;
			}
			
			for (int i = RadarObject.allRadarObjects.Count - 1; i >= 0; i--)
			{
				if (!RadarObject.allRadarObjects[i])
				{
					RadarObject.allRadarObjects.RemoveAt(i);
					continue;
				}

				radarDrawer.UpdateIconForObject(RadarObject.allRadarObjects[i]);
			}

			timeToNextUpdate = pauseBetweenUpdates;
		}

		public void ShowHintFor(RadarObject radarObject)
		{
			if (!hint)
				return;

			hint.ShowForObject(radarObject);
		}
	}
}