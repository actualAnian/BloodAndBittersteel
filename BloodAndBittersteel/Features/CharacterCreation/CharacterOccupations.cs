namespace BloodAndBittersteel.Features.CharacterCreation;

public static class CharacterOccupations
{
    public const string Retainer = "retainer";
    public const string Bard = "bard";
    public const string Hunter = "hunter";
    public const string Farmer = "farmer";
    public const string Herder = "herder";
    public const string Healer = "healer";
    public const string Mercenary = "mercenary";
    public const string Infantry = "infantry";
    public const string Skirmisher = "skirmisher";
    public const string Kern = "kern";
    public const string Guard = "guard";
    public const string RetainerUrban = "retainer_urban";
    public const string MercenaryUrban = "mercenary_urban";
    public const string MerchantUrban = "merchant_urban";
    public const string VagabondUrban = "vagabond_urban";
    public const string ArtisanUrban = "artisan_urban";
    public const string PhysicianUrban = "physician_urban";
    public const string HealerUrban = "healer_urban";
    public const string BardUrban = "bard_urban";
    public const string Noble = "noble";
    public const string Merchant = "merchant";
    public const string Craftman = "craftman";
    public const string Cavalry = "cavalry";
    public const string Bandit = "bandit";

public static bool IsUrbanOccupation(string occupation)
    {
        return occupation switch
        {
            RetainerUrban or MercenaryUrban or MerchantUrban or VagabondUrban or ArtisanUrban or PhysicianUrban or HealerUrban => true,
            _ => occupation == BardUrban,
        };
    }
}
