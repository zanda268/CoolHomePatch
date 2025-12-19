using Il2Cpp;
using Il2CppTLD.PDID;
using Il2CppVLB;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace CoolHomePatch
{
    /// <inheritdoc/>
	public class Main : MelonMod
	{
        /// <inheritdoc/>
        public override void OnInitializeMelon()
        {
            
        }

		public override void OnSceneWasInitialized(int buildIndex, string sceneName)
		{

		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			if (!IsScenePlayable()) return;

			if (sceneName.EndsWith("_SANDBOX"))
			{

				MelonLogger.Msg($"Scene: {sceneName}");
				//SafehouseCustomization+ items seem to load a bit after the scene and after initial items. I believe this is the reason why SC+ doesn't assign them GUIDs in their project.
				MelonCoroutines.Start(new FireGUIDCoroutine().WaitForSceneToLoad());
			}
		}

		public static bool IsScenePlayable()
		{
			return !(string.IsNullOrEmpty(GameManager.m_ActiveScene) || GameManager.m_ActiveScene.Contains("MainMenu") || GameManager.m_ActiveScene == "Boot" || GameManager.m_ActiveScene == "Empty");
		}

	}

	internal class FireGUIDCoroutine : MonoBehaviour
	{
		internal System.Collections.IEnumerator WaitForSceneToLoad()
		{
			MelonLogger.Msg("--------------------------------");

			yield return new WaitForSeconds(1);

			Fire[] allGOs = GameObject.FindObjectsOfType<Fire>();

			foreach (Fire fire in allGOs)
			{
				MelonLogger.Msg($"FireGUIDCoroutine: {fire.name} at {fire.transform.position}");

				ObjectGuid og = fire.GetComponent<ObjectGuid>();

				if (og != null && og.PDID != null)
				{
					MelonLogger.Msg($"Fire already registered under '{og.PDID}'");
					continue;
				}

				//SafehouseCustomization+ Patch
				MelonLogger.Msg($"ObjectGUID or PDID is null. Attempting to patch.");
				og = fire.GetOrAddComponent<ObjectGuid>();

				//Generate seed from position
				Vector3 v = fire.transform.position;
				int seed = Mathf.CeilToInt(v.x * v.z + v.y * 10000f);

				//Generate GUID from seed
				var r = new System.Random(seed);
				var guid = new byte[16];
				r.NextBytes(guid);
				Guid newGuid = new Guid(guid);

				PdidTable.RuntimeAddOrReplace(og, newGuid.ToString());

				MelonLogger.Msg($"Added GUID {og.PDID} to object {fire.name} at {fire.transform.position}");
			}

			MelonLogger.Msg("--------------------------------");

		}
	}
}
