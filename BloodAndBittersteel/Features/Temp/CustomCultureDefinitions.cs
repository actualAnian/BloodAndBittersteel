using System.Collections.Generic;

namespace MedievalEurope15thCenturyMain.CharacterCreation
{
	/// <summary>
	/// Custom culture ids registered for character creation (must match Culture id in SPCultures XML).
	/// These reuse Vlandia family/youth options and equipment roster suffixes.
	/// </summary>
	internal static class CustomCultureDefinitions
	{
		public static readonly HashSet<string> Ids = new HashSet<string>
		{
            "crownlander",
		};
	}
}
