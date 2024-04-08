/*using HEScripts.Combat;
using HorrorEngine;
using UnityEngine;

namespace Enemies
{
    public class DamageOverTimeTrigger : MonoBehaviour
    {
        public float damagePerSecond = 10f;
        private PlayerHealth _playerHealth;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (_playerHealth == null)
                {
                    _playerHealth = other.gameObject.GetComponent<PlayerHealth>();
                }

                if (_playerHealth != null)
                {
                    var damageAmount = damagePerSecond * Time.deltaTime;
                    _playerHealth.TakeDamage(damageAmount, false, false);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _playerHealth = null;
            }
        }
    }
}*/