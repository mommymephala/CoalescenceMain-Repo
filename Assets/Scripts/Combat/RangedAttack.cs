using UnityEngine;

namespace Combat
{
    public class RangedAttack : AttackBase
    {
        public GameObject ProjectilePrefab;
        public Transform FirePoint;
        public float FireRate = 1f;
        public float AttackDuration = 1f; // Add this property
        private float nextFireTime;

        public override void StartAttack()
        {
            if (Time.time >= nextFireTime)
            {
                FireProjectile();
                nextFireTime = Time.time + 1f / FireRate;
            }
        }

        private void FireProjectile()
        {
            if (ProjectilePrefab && FirePoint)
            {
                Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);
            }
        }

        public override void OnAttackNotStarted() { }
    }
}