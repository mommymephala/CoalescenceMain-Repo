using UnityEngine;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 10f;
        public float lifetime = 5f;
        public AttackType attackType;
        public LayerMask hitLayers;

        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.velocity = transform.forward * speed;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter(Collider other)
        {
            ProcessCollision(other);
            Debug.Log(other.name);
        }

        // private void OnCollisionEnter(Collision collision)
        // {
        //     ProcessCollision(collision.collider);
        // }

        private void ProcessCollision(Collider other)
        {
            if (IsInLayerMask(other.gameObject, hitLayers))
            {
                var damageable = other.GetComponentInChildren<Damageable>();
        
                if (damageable)
                {
                    AttackImpact impact = attackType.GetImpact(damageable.Type);
                    if (impact != null)
                    {
                        ApplyDamage(damageable, impact, other.transform.position, transform.forward);
                    }
                }

                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
        {
            return (layerMask.value & (1 << obj.layer)) != 0;
        }

        private static void ApplyDamage(Damageable damageable, AttackImpact impact, Vector3 impactPoint, Vector3 impactDir)
        {
            // Check and apply pre-damage effects if any
            if (impact.PreDamageEffects != null)
            {
                foreach (AttackEffect effect in impact.PreDamageEffects)
                {
                    effect.Apply(new AttackInfo { Damageable = damageable, ImpactPoint = impactPoint, ImpactDir = impactDir });
                }
            }

            // Apply damage
            damageable.Damage(impact.Damage, impactPoint, impactDir);

            // Check and apply post-damage effects if any
            if (impact.PostDamageEffects != null)
            {
                foreach (AttackEffect effect in impact.PostDamageEffects)
                {
                    effect.Apply(new AttackInfo { Damageable = damageable, ImpactPoint = impactPoint, ImpactDir = impactDir });
                }
            }
        }
    }
}