using UnityEngine;
using UnityEditor;

namespace InsaneSystems.Radar
{
	[CustomPropertyDrawer(typeof(RadarObjectCenterAttribute))]
	public class RadarObjectCenterAttributeDrawer : PropertyDrawer
	{
		bool isCenterObjectOnScene;
		string centerObjectName = "";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = 20;

			if (property.boolValue != true)
			{
				RadarObject[] radarObjects = GameObject.FindObjectsOfType<RadarObject>();

				for (int i = 0; i < radarObjects.Length; i++)
					if (radarObjects[i].IsCenter)
					{
						isCenterObjectOnScene = true;
						centerObjectName = radarObjects[i].gameObject.name;
						break;
					}
			}

			GUI.enabled = !isCenterObjectOnScene;
			property.boolValue = EditorGUI.Toggle(position, label, property.boolValue);
			GUI.enabled = true;

			position.y += position.height;
			position.height = 40;	

			if (isCenterObjectOnScene)
				EditorGUI.HelpBox(position, "Center object already exists on scene! Its name: " + centerObjectName, MessageType.Info); 
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (isCenterObjectOnScene)
				return 60;
			else
				return base.GetPropertyHeight(property, label);
		}
	}
}