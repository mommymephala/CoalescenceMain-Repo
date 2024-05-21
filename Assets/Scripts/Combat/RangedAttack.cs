using UnityEngine;

namespace Combat
{
    public class RangedAttack : AttackBase
    {
        public GameObject projectilePrefab;
        public AttackType attackType;
        public Transform firePoint;
        public float fireRate;
        private float _nextFireTime;

        public override void StartAttack()
        {
            if (Time.time >= _nextFireTime)
            {
                FireProjectile();
                _nextFireTime = Time.time + 1f / fireRate;
            }
        }

        private void FireProjectile()
        {
            if (projectilePrefab && firePoint)
            {
                GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                var projectile = projectileInstance.GetComponent<Projectile>();
                projectile.Initialize(this);
            }
        }

        public void ProcessCollision(Collider other)
        {
            var damageable = other.GetComponentInChildren<Damageable>();

            if (damageable)
            {
                AttackImpact impact = attackType.GetImpact(damageable.Type);
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