using HEScripts.Combat;
using HorrorEngine;
using UnityEngine;

namespace Enemies
{
    public class EnemyHitBox : MonoBehaviour
    {
        [SerializeField] private float damageAmount;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damageAmount, false, false);
                Debug.Log("Damage applied to player: " + damageAmount);

                gameObject.SetActive(false);
            }
        }
    }
}