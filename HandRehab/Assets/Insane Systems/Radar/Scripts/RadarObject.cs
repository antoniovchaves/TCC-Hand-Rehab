using System;
using System.Collections.Generic;
using UnityEngine;

namespace InsaneSystems.Radar
{
	public class RadarObject : MonoBehaviour
	{
		public static readonly List<RadarObject> allRadarObjects = new List<RadarObject>();
		
		[Tooltip("Icon, which will be drawn on radar for this object.")]
		[SerializeField] Sprite icon;

		[Tooltip("If icon need custom color, you can change this parameter.")]
		[SerializeField] Color customColor = Color.white;

		[Tooltip("If you have turned on Show Hints toggle in radar settings, you can fill this field with object name, which should be shown on radar. If empty will be shown name of game object.")]
		[SerializeField] string label = "";

		[Tooltip("Turn on this, if you need to show arrows near object icon, if object above or below player.")]
		[SerializeField] bool showHeightArrows;

		[Tooltip("Turn on this on the object, which the radar should follow. For example, on a player.")]
		[SerializeField] [RadarObjectCenter] bool isCenter;

		[Tooltip("Turn on this, if this object will stay on one position during all game.")]
		[SerializeField] bool isStatic;

		[Tooltip("Turn on this, if this icon should be always visible on radar (for important marks, etc).")]
		[SerializeField] bool shouldBeVisibleAllTime;

		[Tooltip("Turn on this, if icon should keep its rotation on radar. It needed for asymmetrical or symbol icons. Used only for rotating radar.")]
		[SerializeField] bool keepIconRotation; 

		GameObject selfIconObject;

		new Transform transform;

		Vector2 startPositon, lastPosition;

		int pingCountsLeft;
		float pingDelay = 0.5f, currentPingDelay;

		bool isVisibleOnCurrentPing;

		Transform heightArrowsParent;

		public Sprite IconSprite { get { return icon; } }
		public Color CustomColor { get { return customColor; } }
		public string Label	{ get { return label; }	}
		public bool IsCenter { get { return isCenter; } }
		public bool ShouldBeVisibleAllTime { get { return shouldBeVisibleAllTime; } }
		public bool KeepIconRotation { get { return keepIconRotation; }	}
		public bool IsStatic {get { return isStatic; } }
		public bool ShowHeightArrows { get { return showHeightArrows; }	}

		public bool IsPinging { get; protected set; }
		public bool IsInitialized { get; protected set; }
		public bool IsVisibleOnRadar { get; set; }
		public RectTransform SelfIconTransform { get; protected set; }
		public Vector3 RealPositionOnRadar { get; set; }
		public Vector3 LocalPositionOnRadar { get; set; }

		public GameObject downArrowIcon { get; protected set; }
		public GameObject topArrowIcon { get; protected set; }

		void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			if (isCenter)
				RadarSystem.sceneSingleton.centerObject = this;
			
			IsInitialized = true;

			transform = GetComponent<Transform>();
			startPositon = GetPosition();
			lastPosition = startPositon;
			IsVisibleOnRadar = true;
			
			if (!isCenter)
				allRadarObjects.Add(this);
		}

		void Update()
		{
			HandlePing();

			if (ShowHeightArrows && RadarSystem.sceneSingleton.centerObject && topArrowIcon)
			{
				var centerObjectHeight = RadarSystem.sceneSingleton.centerObject.transform.position.y;
				var selfHeight = transform.position.y;

				if (Mathf.Abs(centerObjectHeight - selfHeight) < 1f)
				{
					HideHeightArrows();
				}
				else
				{
					if (centerObjectHeight < selfHeight)
						ShowTopHeightArrow();
					else
						ShowBottomHeightArrow();
				}

				var canvas = RadarSystem.sceneSingleton.RadarDrawer.SelfCanvas;

				heightArrowsParent.rotation = canvas ? canvas.transform.rotation : Quaternion.identity;
			}
		}

		void HandlePing()
		{
			if (!IsPinging || !selfIconObject || !IsVisibleOnRadar)
				return;

			if (currentPingDelay > 0)
			{
				currentPingDelay -= Time.deltaTime;
			}
			else
			{
				isVisibleOnCurrentPing = !isVisibleOnCurrentPing;

				selfIconObject.SetActive(isVisibleOnCurrentPing);

				if (isVisibleOnCurrentPing)
					pingCountsLeft--;

				currentPingDelay = pingDelay;

				if (pingCountsLeft == 0)
					IsPinging = false;
			}
		}

		public void SetSelfIcon(RectTransform transform)
		{
			SelfIconTransform = transform;
			selfIconObject = SelfIconTransform.gameObject;

			if (showHeightArrows)
				SetupHeightArrows();
		}

		public void SetupHeightArrows()
		{
			if (RadarSystem.sceneSingleton.settings.gameIs2D)
				return;

			heightArrowsParent = new GameObject("ArrowsParent").transform;
			heightArrowsParent.transform.SetParent(SelfIconTransform);
			heightArrowsParent.transform.localPosition = Vector3.zero;
			heightArrowsParent.transform.localScale = Vector3.one; // fix for parenting on scaled canvas

			topArrowIcon = Instantiate(RadarSystem.sceneSingleton.settings.heightArrowIconTemplate, heightArrowsParent);
			downArrowIcon = Instantiate(RadarSystem.sceneSingleton.settings.heightArrowIconTemplate, heightArrowsParent);

			var topIconRectTransform = topArrowIcon.GetComponent<RectTransform>();
			var downArrowIconRectTransform = downArrowIcon.GetComponent<RectTransform>();

			topIconRectTransform.localPosition = new Vector3(0, topIconRectTransform.sizeDelta.y / 2f, 0);
			downArrowIconRectTransform.localPosition = new Vector3(0, -downArrowIconRectTransform.sizeDelta.y / 2f, 0);

			downArrowIcon.transform.localEulerAngles = new Vector3(0, 0, 180);

			HideHeightArrows();
		}

		public void ShowTopHeightArrow()
		{
			if (RadarSystem.sceneSingleton.settings.gameIs2D)
				return;

			topArrowIcon.SetActive(true);
			downArrowIcon.SetActive(false);
		}

		public void ShowBottomHeightArrow()
		{
			if (RadarSystem.sceneSingleton.settings.gameIs2D)
				return;

			topArrowIcon.SetActive(false);
			downArrowIcon.SetActive(true);
		}

		public void HideHeightArrows()
		{
			if (RadarSystem.sceneSingleton.settings.gameIs2D)
				return;

			topArrowIcon.SetActive(false);
			downArrowIcon.SetActive(false);
		}

		public void UpdateColor(Color newColor)
		{
			if (SelfIconTransform)
				SelfIconTransform.GetComponent<UnityEngine.UI.Image>().color = newColor;
		}

		public void Ping(int times = 3, float delay = 0.25f)
		{
			currentPingDelay = 0;
			IsPinging = true;
			pingCountsLeft = times;
			pingDelay = delay;
			isVisibleOnCurrentPing = true;
		}

		public Vector2 GetPosition()
		{
			var secondCoord = RadarSystem.sceneSingleton.settings.gameIs2D ? transform.position.y : transform.position.z;
			return new Vector2(transform.position.x, secondCoord);
		}

		public Vector2 GetMoveDelta()
		{
			Vector2 delta = GetPosition() - lastPosition;
			lastPosition = GetPosition();

			return delta;
		}

		public Vector2 GetOffsetFromStart()	{ return startPositon - GetPosition(); }

		public float GetEulerRotation()
		{
			return RadarSystem.sceneSingleton.settings.gameIs2D ? -transform.localEulerAngles.z : transform.localEulerAngles.y;
		}

		public void OnDestroy() { Destruct(); }

		public void Destruct()
		{
			if (SelfIconTransform)
				Destroy(SelfIconTransform.gameObject);

			Destroy(this);
		}
	}

	public class RadarObjectCenterAttribute : PropertyAttribute { }
}