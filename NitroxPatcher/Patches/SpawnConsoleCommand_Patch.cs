﻿using Harmony;
using NitroxClient.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NitroxPatcher.Patches
{
    public class SpawnConsoleCommand_Patch : NitroxPatch
    {
        public static readonly Type TARGET_CLASS = typeof(SpawnConsoleCommand);
        public static readonly MethodInfo TARGET_METHOD = TARGET_CLASS.GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(SpawnConsoleCommand __instance)
        {
            __instance.gameObject.AddComponent<Multiplayer>();
            return true;
        }

        public override void Patch(HarmonyInstance harmony)
        {
            this.PatchPrefix(harmony, TARGET_METHOD);
        }
    }
}
