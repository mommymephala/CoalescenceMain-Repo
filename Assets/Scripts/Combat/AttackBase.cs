using UnityEngine;

namespace Combat
{
    public interface IAttack
    {
        void StartAttack();

        void OnAttackNotStarted();
    }

    public struct AttackInfo
    {
        public AttackBase Attack;
        public Damageable Damageable;
        public Vector3 ImpactDir;
        public Vector3 ImpactPoint;
    }

    public abstract class AttackBase : MonoBehaviour, IAttack
    {
        [SerializeField] protected AttackType m_Attack;
        
        // --------------------------------------------------------------------
        protected virtual void Awake()
        {
        }

        // --------------------------------------------------------------------

        public abstract void StartAttack();

        // --------------------------------------------------------------------

        public void Process(AttackInfo info)
        {
            AttackImpact impact = m_Attack.GetImpact(info.Damageable.Type);
            if (impact != null)
            {
                if (impact.Filters != null)
                {
                    foreach (AttackFilter filter in impact.Filters)
                    {
                        if (!filter.Passes(info))
                            return;
                    }
                }

                if (impact.PreDamageEffects != null)
                {
                    foreach (AttackEffect effect in impact.PreDamageEffects)
                    {
                        effect.Apply(info);
                    }
                }

                info.Damageable.Damage(impact.Damage, info.ImpactPoint, info.ImpactDir);

                if (impact.PostDamageEffects != null)
                {
                    foreach (AttackEffect effect in impact.PostDamageEffects)
                    {
                        effect.Apply(info);
                    }
                }
            }
        }

        // --------------------------------------------------------------------

        public virtual void OnAttackNotStarted() { }
    }
}