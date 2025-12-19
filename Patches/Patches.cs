using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolHomePatch.Patches
{
	[HarmonyPatch(typeof(ConsoleManager), nameof(ConsoleManager.Initialize))]
	internal class AddCommands
	{
		internal static void Postfix()
		{
			if (!uConsole.CommandIsRegistered("chpdebug"))
			{
				uConsole.RegisterCommand("chpdebug", new Action(CHPUtils.CONSOLE_CHPDebug));
				uConsole.RegisterCommand("chpdebug2", new Action(CHPUtils.CONSOLE_CHPDebug2));
			}
		}
	}

	[HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.ExitMeshPlacement))]
	internal static class ExitMeshPlacement
	{
		internal static void Postfix(ref DecorationItem __instance)
		{
			MelonCoroutines.Start(new FireGUIDCoroutine().WaitForSceneToLoad());
		}
	}


}
