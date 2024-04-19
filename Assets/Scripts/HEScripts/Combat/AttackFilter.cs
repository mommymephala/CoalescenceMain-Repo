using UnityEngine;

namespace HEScripts.Combat
{
    public class AttackFilter : ScriptableObject
    {
        public virtual bool Passes(AttackInfo info) { return false; }
    }
}