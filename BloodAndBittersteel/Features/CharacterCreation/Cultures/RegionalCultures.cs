namespace BloodAndBittersteel.Features.CharacterCreation.Cultures;

public sealed class ValemanCulture : WesterosiCultureBase
{
    protected override CultureProfile Profile { get; } = new(
        "valeman",
        "{=bab_valeman_parent_retainer_desc}Your family served in the household of a Vale lord, managing his estates and training the men who guarded his mountain roads and passes.",
        "{=bab_valeman_parent_merchant_desc}Your family traded wool, grain, and mountain goods through the market towns and ports of the Vale.",
        "{=bab_valeman_parent_farmer_desc}Your family were free smallholders in one of the Vale's fertile valleys and answered their lord's call as part of the levy.",
        "{=bab_valeman_parent_blacksmith_desc}Your family kept a smithy in a Vale town, shoeing horses and forging tools, mail, and weapons for the local garrison.",
        "{=bab_valeman_parent_hunter_desc}Your family hunted the wooded slopes and high valleys, learning the hidden paths beneath the Mountains of the Moon.",
        "{=bab_valeman_youth_groom_desc}You tended the horses of a landed knight and accompanied him through steep passes and along the high roads of the Vale.",
        "{=bab_valeman_youth_cavalry_desc}You trained beside the mounted men-at-arms for which the Vale is famed, learning to keep your seat and couch a lance.",
        "{=bab_valeman_youth_guard_desc}You stood watch over a mountain stronghold, where a narrow path and a stout gate could hold back an army.",
        "{=bab_valeman_youth_bandit_title}ran with a mountain band.",
        "{=bab_valeman_youth_bandit_desc}You lived among outlaws in the high valleys, learning to move along goat tracks and strike from the rocks before vanishing into the mountains.",
        "{=bab_valeman_youth_infantry_desc}You drilled with the spearmen and swordsmen who held the Vale's passes when mounted knights could go no farther.",
        "{=bab_valeman_youth_skirmisher_desc}You learned to harry foes from broken ground, loosing missiles from slopes where heavier soldiers struggled to follow.");
}

public sealed class StormlanderCulture : WesterosiCultureBase
{
    protected override CultureProfile Profile { get; } = new(
        "stormlander",
        "{=bab_stormlander_parent_retainer_desc}Your family served a storm lord, overseeing his household and mustering the hard folk of the rainwood when banners were called.",
        "{=bab_stormlander_parent_merchant_desc}Your family dealt in timber, grain, and goods landed at the storm coast's guarded harbours.",
        "{=bab_stormlander_parent_farmer_desc}Your family worked stubborn soil beneath frequent storms and stood in the levy when raiders or rival lords threatened.",
        "{=bab_stormlander_parent_blacksmith_desc}Your family owned a town smithy and forged the sturdy tools and weapons demanded by life in the stormlands.",
        "{=bab_stormlander_parent_hunter_desc}Your family hunted beneath the dark boughs of the rainwood and knew how to endure its downpours and tangled paths.",
        "{=bab_stormlander_youth_groom_desc}You served a landed knight, caring for his destrier and carrying orders through driving rain and sodden roads.",
        "{=bab_stormlander_youth_cavalry_desc}You trained with mounted men-at-arms, learning to charge over rough ground even when storms turned the fields to mud.",
        "{=bab_stormlander_youth_guard_desc}You guarded a rain-beaten castle and learned to keep bowstrings dry, walls supplied, and watchfires burning.",
        "{=bab_stormlander_youth_bandit_title}ran with an outlaw band.",
        "{=bab_stormlander_youth_bandit_desc}You joined broken men hidden in the rainwood, surviving by poaching, ambush, and secret paths beneath the trees.",
        "{=bab_stormlander_youth_infantry_desc}You trained among the stormlands' foot, learning to hold a shield wall against raiders and rival hosts.",
        "{=bab_stormlander_youth_skirmisher_desc}You ranged ahead through rain and woodland, striking quickly with bow and javelin before slipping back into cover.");
}

public sealed class DornishCulture : WesterosiCultureBase
{
    protected override CultureProfile Profile { get; } = new(
        "dornish",
        "{=bab_dornish_parent_retainer_desc}Your family served a Dornish lord, administering his holdings and keeping riders ready to defend well, pass, and holdfast.",
        "{=bab_dornish_parent_merchant_desc}Your family traded wine, fruit, spices, and cloth along Dorne's roads and through its ports.",
        "{=bab_dornish_parent_farmer_desc}Your family tended irrigated fields and orchards, guarding every drop of water and answering the local lord's summons.",
        "{=bab_dornish_parent_blacksmith_desc}Your family ran a smithy in a Dornish town, making tools for field and vineyard as well as arms for guards and riders.",
        "{=bab_dornish_parent_hunter_desc}Your family hunted the stony hills and dry valleys, reading faint tracks and surviving far from any well.",
        "{=bab_dornish_youth_groom_desc}You tended a Dornish knight's swift horse and carried messages along dusty roads beneath the hot sun.",
        "{=bab_dornish_youth_cavalry_desc}You trained with light and heavy riders, learning to conserve your mount before closing suddenly with spear and sword.",
        "{=bab_dornish_youth_guard_desc}You guarded a desert holdfast, watching wells and gates and learning how stone walls could master heat and distance.",
        "{=bab_dornish_youth_bandit_title}rode with an outlaw band.",
        "{=bab_dornish_youth_bandit_desc}You rode with smugglers and outlaws who knew hidden springs, lonely tracks, and ways around a lord's patrols.",
        "{=bab_dornish_youth_infantry_desc}You drilled with spear and shield to hold narrow passes and punish any foe exhausted by the Dornish heat.",
        "{=bab_dornish_youth_skirmisher_desc}You learned to strike at range, withdraw across harsh ground, and draw heavier enemies into a fruitless pursuit.");
}

public sealed class ReachmanCulture : WesterosiCultureBase
{
    protected override CultureProfile Profile { get; } = new(
        "reachman",
        "{=bab_reachman_parent_retainer_desc}Your family served in a Reach lord's household, keeping his rich estates productive and his numerous levies prepared.",
        "{=bab_reachman_parent_merchant_desc}Your family traded grain, wine, fruit, and fine goods between the Reach's prosperous towns and river ports.",
        "{=bab_reachman_parent_farmer_desc}Your family were prosperous smallholders in the fertile Reach, raising abundant crops and serving in the local levy.",
        "{=bab_reachman_parent_blacksmith_desc}Your family owned a busy smithy, supplying farms with tools and knights and men-at-arms with weapons and horseshoes.",
        "{=bab_reachman_parent_hunter_desc}Your family hunted the Reach's woods and managed game for nearby estates while avoiding the wrath of noble foresters.",
        "{=bab_reachman_youth_groom_desc}You served a landed knight amid the Reach's tourneys and musters, tending costly horses and polished harness.",
        "{=bab_reachman_youth_cavalry_desc}You trained with the Reach's celebrated horsemen, learning the disciplined charge and the demanding use of the lance.",
        "{=bab_reachman_youth_guard_desc}You stood guard over a wealthy town or castle, protecting granaries, gates, and storehouses from thieves and enemies.",
        "{=bab_reachman_youth_bandit_title}ran with an outlaw band.",
        "{=bab_reachman_youth_bandit_desc}You hid among hedgerows and wooded riverbanks with poachers and broken men who preyed upon the Reach's busy roads.",
        "{=bab_reachman_youth_infantry_desc}You drilled in the dense ranks of spearmen and billmen who supported the Reach's splendid mounted knights.",
        "{=bab_reachman_youth_skirmisher_desc}You screened the host with bow and javelin, using fields, ditches, and hedges to cover your retreat.");
}

public sealed class WesterlanderCulture : WesterosiCultureBase
{
    protected override CultureProfile Profile { get; } = new(
        "westerlander",
        "{=bab_westerlander_parent_retainer_desc}Your family served a Westerlands lord, supervising mines, estates, and tenants while helping train his household levy.",
        "{=bab_westerlander_parent_merchant_desc}Your family traded ore, metalwork, wool, and luxury goods through the wealthy towns of the Westerlands.",
        "{=bab_westerlander_parent_farmer_desc}Your family held a modest farm among rocky hills and fertile valleys, owing service to a powerful local lord.",
        "{=bab_westerlander_parent_blacksmith_desc}Your family worked metal in a prosperous town, with fine ore and wealthy patrons bringing steady custom to the forge.",
        "{=bab_westerlander_parent_hunter_desc}Your family hunted the wooded hills, trapped valuable pelts, and learned the forgotten tracks between mines and keeps.",
        "{=bab_westerlander_youth_groom_desc}You cared for a wealthy knight's horses and observed the strict order of a great Westerlands household.",
        "{=bab_westerlander_youth_cavalry_desc}You trained with well-equipped cavalry, learning to trust in strong armour, close formation, and a decisive charge.",
        "{=bab_westerlander_youth_guard_desc}You guarded a mine, town, or hilltop castle where gold and iron demanded constant vigilance.",
        "{=bab_westerlander_youth_bandit_title}ran with an outlaw band.",
        "{=bab_westerlander_youth_bandit_desc}You joined outlaws who haunted mining roads and wooded hills, robbing rich traffic while evading a lord's patrols.",
        "{=bab_westerlander_youth_infantry_desc}You drilled with disciplined men-at-arms equipped from the wealth of the Westerlands' mines and armouries.",
        "{=bab_westerlander_youth_skirmisher_desc}You learned to scout rocky hills and loose missiles into enemy ranks before their heavier troops could close.");
}

public sealed class RiverlanderCulture : WesterosiCultureBase
{
    protected override CultureProfile Profile { get; } = new(
        "riverlander",
        "{=bab_riverlander_parent_retainer_desc}Your family served a river lord, settling disputes among tenants and mustering men to defend ford, ferry, and field.",
        "{=bab_riverlander_parent_merchant_desc}Your family carried grain, fish, timber, and wool along the river roads and waterways of the Trident.",
        "{=bab_riverlander_parent_farmer_desc}Your family farmed the rich but often-contested soil of the riverlands and learned to rebuild after passing armies.",
        "{=bab_riverlander_parent_blacksmith_desc}Your family owned a town forge, repairing ploughs in peace and making spearheads and horseshoes whenever war returned.",
        "{=bab_riverlander_parent_hunter_desc}Your family hunted beside rivers, marshes, and woods, becoming skilled trackers and boatmen.",
        "{=bab_riverlander_youth_groom_desc}You served a landed knight and carried his messages across ferries, muddy lanes, and the many branches of the Trident.",
        "{=bab_riverlander_youth_cavalry_desc}You trained with mounted men-at-arms, learning where horses could ford safely and where wet ground would ruin a charge.",
        "{=bab_riverlander_youth_guard_desc}You guarded a bridge, ferry, or riverside castle whose possession could decide an entire campaign.",
        "{=bab_riverlander_youth_bandit_title}ran with an outlaw band.",
        "{=bab_riverlander_youth_bandit_desc}You lived among broken men in willow woods and marshy islands, striking at travellers before escaping across the water.",
        "{=bab_riverlander_youth_infantry_desc}You trained with spear and shield, prepared to defend village and ford from yet another army crossing the riverlands.",
        "{=bab_riverlander_youth_skirmisher_desc}You learned to fight from riverbanks and thickets, harrying enemies across ground you knew better than they did.");
}

public sealed class IronIslanderCulture : WesterosiCultureBase
{
    protected override CultureProfile Profile { get; } = new(
        "ironislander",
        "{=bab_ironislander_parent_retainer_desc}Your family served an Ironborn lord or captain, overseeing his hall, ships, and fighting men when the banners were raised.",
        "{=bab_ironislander_parent_merchant_desc}Your family traded fish, iron, timber, and goods brought home by the longships of the Iron Islands.",
        "{=bab_ironislander_parent_farmer_desc}Your family scratched a living from thin soil and cold shores, working land and sea under a local lord.",
        "{=bab_ironislander_parent_blacksmith_desc}Your family worked an island forge, shaping local iron into tools, axes, mail, and fittings for ships.",
        "{=bab_ironislander_parent_hunter_desc}Your family hunted seabirds and seals along the coasts and knew every cove, cliff path, and treacherous current nearby.",
        "{=bab_ironislander_youth_groom_desc}You served in a captain's household, tending his few valuable horses while learning the harder duties expected aboard ship.",
        "{=bab_ironislander_youth_cavalry_desc}Though horses were scarce on the islands, you trained as one of the few mounted retainers maintained by a wealthy lord.",
        "{=bab_ironislander_youth_guard_desc}You stood watch over a harbour and sea-beaten keep, learning to spot unfamiliar sails beyond the headlands.",
        "{=bab_ironislander_youth_bandit_title}sailed with reavers.",
        "{=bab_ironislander_youth_bandit_desc}You went to sea with a hard captain, learning boarding, coastal raiding, and how to vanish beyond the horizon with your spoils.",
        "{=bab_ironislander_youth_infantry_desc}You trained to fight on deck and ashore with axe, spear, and shield in the close press of a landing.",
        "{=bab_ironislander_youth_skirmisher_desc}You learned to cast missiles from a rolling deck and screen a raiding party as it withdrew toward the ships.");
}

public sealed class NorthmanCulture : WesterosiCultureBase
{
    protected override CultureProfile Profile { get; } = new(
        "northman",
        "{=bab_northman_parent_retainer_desc}Your family served a northern lord, keeping his hall and lands through long winters and mustering his folk when called.",
        "{=bab_northman_parent_merchant_desc}Your family traded wool, timber, hides, and preserved food between scattered northern holds and market towns.",
        "{=bab_northman_parent_farmer_desc}Your family worked a northern holding, storing what the short summers yielded and serving in the levy when required.",
        "{=bab_northman_parent_blacksmith_desc}Your family kept a forge whose tools, nails, weapons, and horseshoes were essential to an isolated northern community.",
        "{=bab_northman_parent_hunter_desc}Your family ranged through deep forests and snowy hills, trapping fur and bringing down game through the lean months.",
        "{=bab_northman_youth_groom_desc}You tended a northern lord's horses and carried his messages over long roads through forest, snow, and cold rain.",
        "{=bab_northman_youth_cavalry_desc}You trained with mounted retainers, learning to fight from a hardy horse bred for distance and difficult ground.",
        "{=bab_northman_youth_guard_desc}You guarded a timber hall or ancient stone keep and learned the patience required for lonely watches in bitter weather.",
        "{=bab_northman_youth_bandit_title}ran with a band of broken men.",
        "{=bab_northman_youth_bandit_desc}You survived with poachers and deserters in the vast northern woods, where distance and winter hid you from pursuit.",
        "{=bab_northman_youth_infantry_desc}You drilled with shield, spear, and heavy blade in the stubborn ranks that form the strength of a northern host.",
        "{=bab_northman_youth_skirmisher_desc}You learned to scout through forest and snow, striking from cover before leading enemies onto hostile ground.");
}
