using Enemy;
using UnityEngine;

namespace Combat
{
    public class Turret : MonoBehaviour
    {
        [SerializeField] private Transform turretPivot;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private RangedAttack rangedAttack;
        [SerializeField] private EnemySensesController sensesController;

        private float _fireTimer;
        private bool _isDisabled;

        private void Update()
        {
            if (_isDisabled) return;

            if (sensesController.IsPlayerInSight)
            {
                Vector3 direction = (sensesController.PlayerTransform.position - turretPivot.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                turretPivot.rotation = Quaternion.Slerp(turretPivot.rotation, lookRotation, Time.deltaTime * rotationSpeed);

                _fireTimer += Time.deltaTime;
                if (_fireTimer >= fireRate)
                {
                    _fireTimer = 0f;
                    Fire();
                }
            }
        }

        private void Fire()
        {
            if (rangedAttack.projectilePrefab && rangedAttack.firePoint)
            {
                GameObject projectileInstance = Instantiate(rangedAttack.projectilePrefab, rangedAttack.firePoint.position, rangedAttack.firePoint.rotation);
                var projectile = projectileInstance.GetComponent<Projectile>();
                projectile.Initialize(rangedAttack);
            }
        }

        public void DisableTurret()
        {
            _isDisabled = true;
        }

        public void EnableTurret()
        {
            _isDisabled = false;
        }
    }
}