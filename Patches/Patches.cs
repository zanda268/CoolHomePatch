using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolHomePatch.Patches
{
	[HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.ExitMeshPlacement))]
	internal static class ExitMeshPlacement
	{
		internal static void Postfix(ref DecorationItem __instance)
		{
			MelonCoroutines.Start(new FireGUIDCoroutine().WaitForSceneToLoad());
		}
	}
}
