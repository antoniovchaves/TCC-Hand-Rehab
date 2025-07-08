using System;
using UnityEngine;

namespace InsaneSystems.Radar
{
	public class GhostFollowingPlayer : MonoBehaviour
	{
		[SerializeField] Transform playerTransform;

		Transform secondaryTransform;
		
		new Transform transform;

		void Start()
		{
			transform = GetComponent<Transform>();
		}

		void Update()
		{
			if (playerTransform && playerTransform.gameObject.activeSelf)
				SetPositionAsTransform(playerTransform);
			else if (secondaryTransform && secondaryTransform.gameObject.activeSelf)
				SetPositionAsTransform(secondaryTransform);
		}

		void SetPositionAsTransform(Transform otherTransform)
		{
			transform.position = otherTransform.position;
			transform.rotation = otherTransform.rotation;
		}

		public void SetPlayerTransform(Transform playerTransform)
		{
			this.playerTransform = playerTransform;
		}

		public void SetSecondaryTransform(Transform secondaryTransform)
		{
			this.secondaryTransform = secondaryTransform;
		}
	}
}