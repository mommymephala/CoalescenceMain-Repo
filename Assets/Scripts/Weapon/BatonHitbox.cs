using Interfaces;
using UnityEngine;

namespace Weapon
{
    public class BatonHitbox : MonoBehaviour
    {
        [SerializeField] private float damageAmount;
        [SerializeField] private float extraDamage;
    
        private bool _isChargedAttack;

        public void SetChargedAttack(bool charged)
        {
            _isChargedAttack = charged;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                var totalDamage = _isChargedAttack ? damageAmount + extraDamage : damageAmount;
                damageable.TakeDamage(totalDamage, _isChargedAttack);
                Debug.Log("Damage applied to enemy: " + totalDamage);

                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log($"Collider {other.name} is not enemy.");
            }
        }
    }
}
