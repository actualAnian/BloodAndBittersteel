using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.Engine.GauntletUI;

namespace LanceSystem.UI.UIPatches
{
    internal class TempPatch
    {
        [HarmonyPatch(typeof(GauntletLayer), "LoadMovie", new Type[] { typeof(GauntletMovieIdentifier)})]
        public static class ReplaceUIPatch
        {
            public static void Prefix(GauntletMovieIdentifier identifier)
            {
                if (identifier.MovieName == "PartyScreen")
                {
                    var prop = typeof(GauntletMovieIdentifier)
                        .GetProperty(
                            "MovieName",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                        );
                    prop.SetValue(identifier, "BaBPartyScreen");
                }
                    //identifier.MovieName = "BaBPartyScreen";
            }
        }
    }
}
