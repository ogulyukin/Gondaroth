using System;
using RPG.Core;
using UnityEngine;

namespace RPG.Spells
{
    public class Spell : MonoBehaviour
    {
        public Core.Spells spellType;
        public float duration;
        public UnitTypes caster;
    }
}
