using System.Collections.Generic;

namespace BloodAndBittersteel.Features.HelmetTilting
{
    internal class HelmetSwapManager
    {
        private static readonly string _path = PathHelper.BaBOutsideConfigPath + "helmet_swap.xml";
        private static HelmetSwapManager? _instance;
        public static HelmetSwapManager Instance => _instance ??= new HelmetSwapManager();
        public IReadOnlyList<HelmetSwap> Swaps { get; private set; } = new List<HelmetSwap>();
        public void LoadFromFile()
        {
            Swaps = HelmetSwapDeserializer.LoadFromFile(_path);
        }
    }
}
