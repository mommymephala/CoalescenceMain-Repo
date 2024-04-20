using UnityEngine;

namespace Combat
{
    public class AttackFilter : ScriptableObject
    {
        public virtual bool Passes(AttackInfo info) { return false; }
    }
}