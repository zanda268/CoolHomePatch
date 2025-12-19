using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolHomePatch
{
	internal class CHPUtils
	{
		internal static void CONSOLE_CHPDebug()
		{
			Ray ray = GameManager.GetMainCamera().ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out RaycastHit hit, 3f, (int)787009))
			{
				GameObject go = hit.collider.GetComponentInParent<GameObject>();

				if (go != null) MelonLogger.Msg($"SCPDebug: {go.name} is a {go.GetType().ToString()}");
			}
		}

		internal static void CONSOLE_CHPDebug2()
		{
			GameObject go = GetRealGameObjectUnderCrosshair();

			MelonLogger.Msg($"SCPDebug2: {go.name} is a {go.GetType().ToString()}");
		}

		public static GameObject? GetRealGameObjectUnderCrosshair()
		{
			PlayerManager pm = GameManager.GetPlayerManagerComponent();

			float maxPickupRange = GameManager.GetGlobalParameters().m_MaxPickupRange;
			float maxRange = pm.ComputeModifiedPickupRange(maxPickupRange);
			if (pm.GetControlMode() == PlayerControlMode.InFPCinematic)
			{
				maxRange = 50f;
			}

			Ray ray = GameManager.GetMainCamera().ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out RaycastHit hit, maxRange, (int)787009))
			{
				GameObject hitGo = GetRealParent(hit.transform);
				if (hitGo.name.StartsWith("ARC_"))
				{
					HUDMessage.AddMessage(Localization.Get("SCP_Action_NoValidObject"));
					return null;
				}
				foreach (Renderer r in hitGo.transform.GetComponentsInChildren<Renderer>())
				{
					if (r && r.isPartOfStaticBatch)
					{
						HUDMessage.AddMessage(Localization.Get("SCP_Action_IsPartOfStaticBatch"));
						return null;
					}
				}

				return hitGo;
			}
			else
			{
				HUDMessage.AddMessage(Localization.Get("SCP_Action_NoValidObject"));
				return null;
			}
		}

		public static GameObject GetRealParent(Transform t)
		{
			while (t.parent
				&& !WithinDistance(t.parent.position, Vector3.zero)
				&& t.parent.name.Contains("_")
				&& !t.parent.GetComponent<RadialObjectSpawner>())
			{
				t = t.parent;
			}
			return t.gameObject;
		}

		public static bool WithinDistance(Vector3 A, Vector3 B, float distance = 0.01f)
		{
			float dx = A.x - B.x;
			if (Mathf.Abs(dx) > distance) return false;

			float dy = A.y - B.y;
			if (Mathf.Abs(dy) > distance) return false;

			float dz = A.z - B.z;
			if (Mathf.Abs(dz) > distance) return false;

			return true;
		}
	}
}
