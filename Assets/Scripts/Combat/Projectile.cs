using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        public float Speed = 10f;
        public float Lifetime = 5f;
        public int Damage = 10;
        public LayerMask HitLayers;

        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * Speed;

            // Set collision detection mode to Continuous
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            Destroy(gameObject, Lifetime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((HitLayers.value & (1 << other.gameObject.layer)) > 0)
            {
                if (other.TryGetComponent(out Damageable damageable))
                {
                    // Assuming Damageable has a Damage method
                    damageable.Damage(Damage, transform.position, transform.forward);
                }
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((HitLayers.value & (1 << collision.gameObject.layer)) > 0)
            {
                if (collision.gameObject.TryGetComponent(out Damageable damageable))
                {
                    // Assuming Damageable has a Damage method
                    damageable.Damage(Damage, transform.position, transform.forward);
                }
                Destroy(gameObject);
            }
            else
            {
                // Destroy the projectile if it hits any solid object
                Destroy(gameObject);
            }
        }
    }
}