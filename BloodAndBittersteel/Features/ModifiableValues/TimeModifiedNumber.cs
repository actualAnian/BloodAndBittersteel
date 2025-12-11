using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace BloodAndBittersteel.Features.ModifiableValues
{
    public class TimedModifierNumber
    {
        [SaveableField(1)]
        public List<TimedModifier> _modifiers = new();

        [SaveableProperty(2)] 
        public float BaseValue { get; set; }
        public TimedModifierNumber(float baseValue = 0)
        {
            BaseValue = baseValue;
        }
        public void AddModifier(TimedModifier modifier)
        {
            _modifiers.RemoveAll(m => m.Id == modifier.Id);
            _modifiers.Add(modifier);
        }
        public void RemoveModifier(string id)
        {
            _modifiers.RemoveAll(m => m.Id == id);
        }
        private void Cleanup()
        {
            _modifiers.RemoveAll(m => m.IsExpired);
        }

        public ExplainedNumber GetExplained()
        {
            Cleanup();
            var result = new ExplainedNumber(BaseValue, true, new("base value"));

            foreach (var m in _modifiers)
                result.Add(m.Factor, new(m.Description));
            return result;
        }
        public float CurrentValue => GetExplained().ResultNumber;
    }

}
