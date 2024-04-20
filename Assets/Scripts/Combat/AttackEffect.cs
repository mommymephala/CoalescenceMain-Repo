using UnityEngine;

namespace Combat
{
    public class AttackEffect : ScriptableObject
    {
        public virtual void Apply(AttackInfo info) { }
    }
}