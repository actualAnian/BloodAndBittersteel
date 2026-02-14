using LanceSystem.LanceDataClasses;
using TaleWorlds.CampaignSystem;

namespace LanceSystem
{
    public class LanceEvents
    {
        private static readonly MbEvent<LanceData> _lanceDisbanded = new ();
        public static IMbEvent<LanceData> LanceDisbanded
        {
            get
            {
                return _lanceDisbanded;
            }
        }
        internal static void OnLanceDisbanded(LanceData lance)
        {
            _lanceDisbanded.Invoke(lance);
        }

    }
}
