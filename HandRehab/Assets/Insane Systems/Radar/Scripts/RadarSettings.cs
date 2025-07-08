using UnityEngine;

namespace InsaneSystems.Radar
{
	[CreateAssetMenu(fileName = "RadarSettings", menuName = "Insane Systems/Radar/Settings")]
	public class RadarSettings : ScriptableObject
	{
		public enum RadarShape
		{
			Rectangle,
			Circle
		}

		[Tooltip("UI template of radar icon object. You can change this prefab settings to change some icon parameters, like size, pivot point, etc.")]
		public GameObject radarIconTemplate;
		[Tooltip("UI template of height arrow icon object. Height arrows drawn above or below radar icons, if this option activated in radar object settings.")]
		public GameObject heightArrowIconTemplate;
		[Tooltip("Radar zoom value. Default value is 1 - real scale on radar. Bigger values will increase scaling.")]
		[Range(0.1f, 8f)] public float radarScale = 1f;
		[Tooltip("If you need bigger icons on radar, increase this value.")]
		[Range(1f, 4f)] public float iconsScale = 1f;
		[Tooltip("If you're making an Action or RPG and this toggle checked, radar will rotate with player, otherwise only player icon will rotate.")]
		public bool rotateRadar;
		[Tooltip("Mark this if your game is 2D and radar will work true. If your game is 3D, left this toggle empty.")]
		public bool gameIs2D;
		public Color customPrimaryUIColor = Color.white;
		public RadarShape radarShape;

		[Header("Hints")]
		public bool showHintsForRadarIcons;
		public bool showGameObjectNameIfRadarObjectLabelEmpty = true;
		[HideInInspector] public bool showHintsOnlyByClick = false; // todo add it in future
	}
}