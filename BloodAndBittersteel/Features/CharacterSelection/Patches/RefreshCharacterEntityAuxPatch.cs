//using HarmonyLib;
//using LOTRAOM.Features.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Emit;
//using TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator;
//using TaleWorlds.MountAndBlade;

//namespace LOTRAOM.Features.CharacterSelection.Patches;

//[HarmonyPatch(typeof(BodyGeneratorView), "RefreshCharacterEntityAux")]
//public class RefreshCharacterEntityAuxPatch
//{
//    public static void Initialize() { }

//    public static MBActionSet GetActionSet(BodyGeneratorView bodyGeneratorView)
//    {
//        var race = bodyGeneratorView.BodyGen.Race;
//        var isFemale = bodyGeneratorView.BodyGen.IsFemale;

//        _logger?.LogDebug($"GetActionSet called for race={race}, isFemale={isFemale}");

//        try
//        {
//            var baseMonster = TaleWorlds.Core.FaceGen.GetBaseMonsterFromRace(race);

//            // Validate monster is not null
//            if (baseMonster == null)
//            {
//                _logger?.LogWarning($"GetBaseMonsterFromRace returned null for race {race}, falling back to human");
//                baseMonster = TaleWorlds.Core.FaceGen.GetBaseMonsterFromRace(0);
//            }
//            else
//            {
//                _logger?.LogDebug($"Got baseMonster: ActionSetCode={baseMonster.ActionSetCode}, FemaleActionSetCode={baseMonster.FemaleActionSetCode}");
//            }

//            var actionSet = MBGlobals.GetActionSetWithSuffix(baseMonster, isFemale, "_facegen");

//            // Validate action set is valid
//            if (!actionSet.IsValid)
//            {
//                _logger?.LogWarning($"Action set invalid for race {race} (monster={baseMonster?.StringId}), falling back to human");
//                var humanMonster = TaleWorlds.Core.FaceGen.GetBaseMonsterFromRace(0);
//                actionSet = MBGlobals.GetActionSetWithSuffix(humanMonster, isFemale, "_facegen");
//            }
//            else
//            {
//                _logger?.LogDebug($"Action set valid for race {race}");
//            }

//            return actionSet;
//        }
//        catch (Exception ex)
//        {
//            _logger?.LogError($"Exception getting action set for race {race}: {ex.Message}\n{ex.StackTrace}");
//            // Ultimate fallback to human
//            var humanMonster = TaleWorlds.Core.FaceGen.GetBaseMonsterFromRace(0);
//            return MBGlobals.GetActionSetWithSuffix(humanMonster, isFemale, "_facegen");
//        }
//    }

//    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
//    {
//        var newInstructions = new List<CodeInstruction>(instructions);
//        var insertionIndex = -1;

//        // Find where AgentVisualsData is instantiated
//        for (int i = 0; i < newInstructions.Count - 1; i++)
//        {
//            var instruction = newInstructions[i];
//            if (instruction.opcode == OpCodes.Newobj && instruction.operand is System.Reflection.ConstructorInfo ci && ci == AccessTools.Constructor(typeof(AgentVisualsData)))
//            {
//                insertionIndex = i + 1;
//                break;
//            }
//        }

//        if (insertionIndex < 0)
//        {
//            throw new ArgumentException("Cannot find AgentVisualsData constructor. Patch: RefreshCharacterEntityAuxPatch");
//        }

//        var insertedInstructions = new List<CodeInstruction>
//        {
//            // Load "this" (BodyGeneratorView) onto the stack
//            new CodeInstruction(OpCodes.Ldarg_0),
//            // Call our method to get the correct action set
//            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RefreshCharacterEntityAuxPatch), nameof(GetActionSet))),
//            // Call ActionSet method on AgentVisualsData
//            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(AgentVisualsData), nameof(AgentVisualsData.ActionSet)))
//        };

//        newInstructions.InsertRange(insertionIndex, insertedInstructions);
//        return newInstructions.AsEnumerable();
//    }
//}