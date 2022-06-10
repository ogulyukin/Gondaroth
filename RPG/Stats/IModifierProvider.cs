using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifier(MainStats stat);
        public IEnumerable<float> GetPercentageModifier(MainStats stat);
    }
}
