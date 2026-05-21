using BloodAndBittersteel.Features.CharacterSelection.Patches;

namespace BloodAndBittersteel.Features.CharacterSelection;

public static class CharacterSelectionIoC
{
    public static void RegisterCharacterSelectionFeature()
    {
        //RefreshCharacterEntityAuxPatch.Initialize();
        BodyGeneratorViewPatch.Initialize();
    }
}