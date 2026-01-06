using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;

namespace BloodAndBittersteel.UI
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
