using UnityEngine;
using UnityEngine.UI;

namespace InsaneSystems.Radar.UI
{
	public class RadarDrawer : MonoBehaviour
	{
		[Tooltip("Add here Canvas, where placed this radar UI. It also can be loaded automatically.")]
		[SerializeField] Canvas selfCanvas;
		[Tooltip("Bring here RectTransform, which contains all radar UI Elements.")]
		[SerializeField] RectTransform mainRadarWindow;
		[Tooltip("Bring here objectsPanel parent RectTransform.")]
		[SerializeField] RectTransform radarPanelParent;
		[Tooltip("Bring here RectTransform, which will contain all radar icons objects.")]
		[SerializeField] RectTransform objectsPanel;
		[Tooltip("Bring here RectTransform, which will present center map icon (like player icon).")]
		[SerializeField] RectTransform directionArrow;

		Vector2 startPanelCenter; 

		public Canvas SelfCanvas
		{
			get { return selfCanvas; }
		}

		void Start()
		{
			startPanelCenter = objectsPanel.localPosition;

			if (!selfCanvas)
				selfCanvas = GetComponentInParent<Canvas>();
		}

		void Update()
		{
			if (RadarSystem.sceneSingleton.centerObject)
			{
				if (RadarSystem.sceneSingleton.settings.rotateRadar)
				{
					UpdateRadarRotation();

					if (directionArrow.localEulerAngles.z != 0)
						directionArrow.localEulerAngles = new Vector3(0, 0, 0);
				}
				else
				{
					if (radarPanelParent.localEulerAngles.z != 0)
						radarPanelParent.localEulerAngles = new Vector3(0, 0, 0);
					directionArrow.localEulerAngles = new Vector3(0, 0, -RadarSystem.sceneSingleton.centerObject.GetEulerRotation());
				}

				objectsPanel.localPosition = startPanelCenter + RadarSystem.sceneSingleton.centerObject.GetOffsetFromStart() * RadarSystem.sceneSingleton.settings.radarScale; 
			}
		}

		void UpdateRadarRotation()
		{
			radarPanelParent.localEulerAngles = new Vector3(0, 0, RadarSystem.sceneSingleton.centerObject.GetEulerRotation());
		}

		public void DestroyIcon(Image icon)
		{
			Destroy(icon.gameObject);
		}

		public void UpdateIconForObject(RadarObject radarObject, bool isFirstDraw = false)
		{
			if (!radarObject.SelfIconTransform)
			{
				var spawnedIcon = Instantiate(RadarSystem.sceneSingleton.settings.radarIconTemplate, objectsPanel);
				var iconImage = spawnedIcon.GetComponent<Image>();
				var iconTransform = spawnedIcon.GetComponent<RectTransform>();

				iconImage.sprite = radarObject.IconSprite;
				iconImage.color = radarObject.CustomColor;
				radarObject.SetSelfIcon(iconTransform);
				iconTransform.SetAsFirstSibling();
				iconTransform.sizeDelta *= RadarSystem.sceneSingleton.settings.iconsScale;

				if (RadarSystem.sceneSingleton.settings.showHintsForRadarIcons)
				{
					spawnedIcon.AddComponent<IconHintData>().selfRadarObject = radarObject;
					iconImage.raycastTarget = true;
				}

				isFirstDraw = true;
			}

			if (!radarObject.IsPinging)
			{
				if (radarObject.SelfIconTransform.gameObject.activeSelf != radarObject.IsVisibleOnRadar)
					radarObject.SelfIconTransform.gameObject.SetActive(radarObject.IsVisibleOnRadar);

				if (radarObject.IsVisibleOnRadar && radarObject.SelfIconTransform.gameObject.activeSelf != radarObject.gameObject.activeSelf)
					radarObject.SelfIconTransform.gameObject.SetActive(radarObject.gameObject.activeSelf);
			}

			if (!isFirstDraw && radarObject.IsStatic) // && !radarObject.ShouldBeVisibleAllTime also needed " || !IsPointOnRadar(radarObject.SelfIconTransform.anchoredPosition)", but now it works not very good, because after point moves out of the radar, this condition always will be true and prevents movement of object. Russian: данная проблема будет решена в будущем апдейте. Необходимо делать проверку так, чтобы точка могла возвращаться обратно.
				return;

			SetIconPosition(radarObject);
			UpdateIconStickedPositionAndRotation(radarObject); // prevents flickering
		}

		void SetIconPosition(RadarObject radarObject)
		{
			Vector2 centerObjectPosition = RadarSystem.sceneSingleton.centerObject.GetPosition();
			Vector2 radarObjectOffsetFromCenterObject = radarObject.GetPosition() - centerObjectPosition;
			Vector2 actualRadarTransformOffset = RadarSystem.sceneSingleton.centerObject.GetOffsetFromStart();

			// Russian: У нас есть смещение объекта относительно игрока. И оно корректно. Но у нас так же смещён трансформ радара. Чтобы исключить
			// двойное смещение, вычитаем смещение радара.
			// English: Using the offset locally by player, and it is right. But we also have ofsetted transform of the radar. 
			// To exclude double-offset, subtract the radar offset.

			radarObject.SelfIconTransform.localPosition = startPanelCenter + (radarObjectOffsetFromCenterObject - actualRadarTransformOffset) * RadarSystem.sceneSingleton.settings.radarScale; // todo maybe move to anchoredPosition? Looks like it works same.
			radarObject.RealPositionOnRadar = radarObject.SelfIconTransform.position; 
			radarObject.LocalPositionOnRadar = radarObject.SelfIconTransform.localPosition; 
		}

		public void UpdateIconStickedPositionAndRotation(RadarObject radarObject)
		{
			if (!radarObject || !radarObject.SelfIconTransform)
				return;

			if (radarObject.KeepIconRotation)
			{
				var up = radarObject.SelfIconTransform.position + Vector3.forward;
				var newRotation = SelfCanvas ? SelfCanvas.transform.rotation : Quaternion.LookRotation(up - radarObject.SelfIconTransform.position);
				
				if (radarObject.SelfIconTransform.rotation != newRotation)
					radarObject.SelfIconTransform.rotation = newRotation;
			}

			if (!radarObject.ShouldBeVisibleAllTime)
				return;

			SetIconPosition(radarObject);

			bool isRadarCircle = RadarSystem.sceneSingleton.settings.radarShape == RadarSettings.RadarShape.Circle;
			var rect = new Rect(mainRadarWindow.position.x, mainRadarWindow.position.y, mainRadarWindow.sizeDelta.x, mainRadarWindow.sizeDelta.y);
			var radarCenter = new Vector3(rect.center.x, rect.center.y, 0);
			
			var iconSize = radarObject.SelfIconTransform.sizeDelta;

			float maxDistance = 0;

			if (isRadarCircle)
				maxDistance = (mainRadarWindow.sizeDelta.x - iconSize.x) / 2f; 
			else
				maxDistance = Mathf.Max(mainRadarWindow.sizeDelta.x, mainRadarWindow.sizeDelta.y) / 2f;

			var realRadarCenter = (Vector2)objectsPanel.localPosition; // we calculating local radar center based on offset of objects panel from minimap center
			realRadarCenter.x = -realRadarCenter.x; // inversion is needed to get real position from center, not offset of objectsPanel.
			realRadarCenter.y = -realRadarCenter.y;

			Vector2 onRadarPosition = radarObject.LocalPositionOnRadar;

			Vector2 normalizedPosition = (onRadarPosition - realRadarCenter).normalized;
			Vector2 finalIconPosition = onRadarPosition;

			if (isRadarCircle)
			{
				if (Vector2.Distance(onRadarPosition, realRadarCenter) > Mathf.Abs(maxDistance))
					finalIconPosition = realRadarCenter + normalizedPosition * maxDistance;
			}
			else
			{
				if (RadarSystem.sceneSingleton.settings.rotateRadar)
				{
					/* Russian: Идея в том, чтобы привести позицию этой иконки к локальным координатам mainRadarWindow, и там посчитать ограничения
					 * движения по рамкам, а затем вернуть "ограниченную" рамками позицию к локальным координатам её реального родителя, который
					 * по принципу работы радара может вращаться и двигаться, поэтому считать что-либо относительно него сложно.
					 * 
					 * English: Key idea here to get clamped pos in mainRadarWindow local space and return this coords to local space of icon parent.
					 */

					var selfObjectInMainRadarWindow = mainRadarWindow.InverseTransformPoint(radarObject.SelfIconTransform.position);

					var finalIconPosition3 = new Vector3();
					finalIconPosition3.x = Mathf.Clamp(selfObjectInMainRadarWindow.x, iconSize.x, mainRadarWindow.sizeDelta.x - iconSize.x);
					finalIconPosition3.y = Mathf.Clamp(selfObjectInMainRadarWindow.y, iconSize.y, mainRadarWindow.sizeDelta.y - iconSize.y);

					finalIconPosition3 = mainRadarWindow.TransformPoint(finalIconPosition3);
					finalIconPosition = radarObject.SelfIconTransform.parent.InverseTransformPoint(finalIconPosition3);
				}
				else // возможно, это устарело, и способ выше подойдёт для обоих вариантов.
				{
					/* Russian: Идея в том, чтобы взять локальную позицию иконки на радаре, посчитать её позицию относительно
					 * текущего центра радара, и затем вписать в квадрат по размеру миникарты, после чего обновить локальную позицию, которая будет
					 * уже относительно центра радара.
					 * 
					 * English: Key idea here to calculate local icon position relative to radar center and clamp it to minimap size.
					 */

					var halfSizeX = mainRadarWindow.sizeDelta.x / 2f;
					var halfSizeY = mainRadarWindow.sizeDelta.y / 2f;

					finalIconPosition.x = Mathf.Clamp(onRadarPosition.x - realRadarCenter.x, -halfSizeX + iconSize.x, halfSizeX - iconSize.x);
					finalIconPosition.y = Mathf.Clamp(onRadarPosition.y - realRadarCenter.y, -halfSizeY + iconSize.y, halfSizeY - iconSize.y);
					finalIconPosition += realRadarCenter;
				}
			}

			radarObject.SelfIconTransform.localPosition = finalIconPosition;
		}

		bool IsPointOnRadar(Vector2 point, bool isRadarCircle = false)
		{
			if (isRadarCircle)
			{
				var rect = new Rect(mainRadarWindow.position.x, mainRadarWindow.position.y, mainRadarWindow.sizeDelta.x, mainRadarWindow.sizeDelta.y);

				var radarCenter = new Vector3(rect.center.x, rect.center.y, 0);
				float maxDistance = mainRadarWindow.sizeDelta.x / 2f;

				return Vector2.Distance(point, radarCenter) <= Mathf.Abs(maxDistance);
			}

			return point.x >= 0 && point.x <= objectsPanel.rect.width && point.y >= 0 && point.y <= objectsPanel.rect.height;
		}
	}
}