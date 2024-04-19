using UnityEngine;

namespace HEScripts.Combat
{
    public class AttackEffect : ScriptableObject
    {
        public virtual void Apply(AttackInfo info) { }
    }
}