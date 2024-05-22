using UnityEngine;

namespace Combat
{
    public class RangedAttack : AttackBase
    {
        public GameObject projectilePrefab;
        public Transform firePoint;
        public int numberOfProjectiles = 3;
        public float spreadAngle = 5f;
        
        public override void StartAttack()
        {
            FireProjectiles();
        }

        private void FireProjectiles()
        {
            if (projectilePrefab && firePoint)
            {
                for (var i = 0; i < numberOfProjectiles; i++)
                {
                    FireProjectile(i);
                }
            }
        }

        private void FireProjectile(int index)
        {
            GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            var projectile = projectileInstance.GetComponent<Projectile>();
            projectile.Initialize(this);

            if (numberOfProjectiles > 1)
            {
                var angleOffset = spreadAngle * ((float)index / (numberOfProjectiles - 1) - 0.5f);
                projectile.transform.Rotate(0, angleOffset, 0);
            }
        }

        public void ProcessCollision(Collider other)
        {
            var damageable = other.GetComponentInChildren<Damageable>();

            if (damageable)
            {
                AttackImpact impact = m_Attack.GetImpact(damageable.Type);
                if (impact != null)
                {
                    Vector3 impactPoint = other.transform.position;
                    Vector3 impactDir = other.transform.forward;
                    Process(new AttackInfo
                    {
                        Attack = this,
                        Damageable = damageable,
                        ImpactDir = impactDir,
                        ImpactPoint = impactPoint
                    });
                }
            }
        }
        
        public override void OnAttackNotStarted() { }
    }
}