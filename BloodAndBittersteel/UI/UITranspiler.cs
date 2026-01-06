using HarmonyLib;
using SandBox.GauntletUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;

namespace BloodAndBittersteel.UI
{
    [HarmonyPatch(typeof(GauntletPartyScreen), "TaleWorlds.Core.IGameStateListener.OnActivate")]
    public class UITranspiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions) 
            {
                if (instruction.opcode == OpCodes.Newobj 
                    && instruction.operand is ConstructorInfo constructorInfo && 
                    constructorInfo.DeclaringType == typeof(PartyVM))
                {
                    // Replace with BaBPartyVM constructor
                    var babConstructor = typeof(BaBPartyVM).GetConstructor(new Type[] { typeof(PartyScreenLogic) });
                    yield return new CodeInstruction(OpCodes.Newobj, babConstructor);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

    }
}



/*
 30	0076	ldarg.0
31	0077	ldarg.0
32	0078	ldfld	class [TaleWorlds.CampaignSystem]TaleWorlds.CampaignSystem.GameState.PartyState SandBox.GauntletUI.GauntletPartyScreen::_partyState
33	007D	callvirt	instance class [TaleWorlds.CampaignSystem]TaleWorlds.CampaignSystem.Party.PartyScreenLogic [TaleWorlds.CampaignSystem]TaleWorlds.CampaignSystem.GameState.PartyState::get_PartyScreenLogic()
34	0082	newobj	instance void [TaleWorlds.CampaignSystem.ViewModelCollection]TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyVM::.ctor(class [TaleWorlds.CampaignSystem]TaleWorlds.CampaignSystem.Party.PartyScreenLogic)
35	0087	stfld	class [TaleWorlds.CampaignSystem.ViewModelCollection]TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyVM SandBox.GauntletUI.GauntletPartyScreen::_dataSource

 
 v*/