using System.Collections.Generic;

namespace BloodAndBittersteel.Features.CharacterCreation;

public interface ICharacterCreationCulture
{
    string CultureId { get; }
    IEnumerable<NarrativeOption> GetParentOptions();
    IEnumerable<NarrativeOption> GetChildhoodOptions();
    IEnumerable<NarrativeOption> GetEducationOptions();
    IEnumerable<NarrativeOption> GetYouthOptions();
    IEnumerable<NarrativeOption> GetAdulthoodOptions();
}
