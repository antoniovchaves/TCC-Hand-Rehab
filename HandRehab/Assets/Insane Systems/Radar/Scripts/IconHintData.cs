using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InsaneSystems.Radar
{
	public class IconHintData : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		[HideInInspector] public RadarObject selfRadarObject;

		public void OnPointerClick(PointerEventData pointerData)
		{
			if (RadarSystem.sceneSingleton.hint && RadarSystem.sceneSingleton.hint.shownForRadarObject == selfRadarObject)
				RadarSystem.sceneSingleton.hint.Hide();
			else 
				RadarSystem.sceneSingleton.ShowHintFor(selfRadarObject);
		}

		public void OnPointerEnter(PointerEventData pointerData)
		{
			if (RadarSystem.sceneSingleton.settings.showHintsOnlyByClick)
				return;

			RadarSystem.sceneSingleton.ShowHintFor(selfRadarObject);
		}

		public void OnPointerExit(PointerEventData pointerData)
		{
			if (RadarSystem.sceneSingleton.settings.showHintsOnlyByClick)
				return;

			if (RadarSystem.sceneSingleton.hint)
				RadarSystem.sceneSingleton.hint.Hide();
		}
	}
}