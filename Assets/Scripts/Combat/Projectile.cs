using UnityEngine;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 10f;
        public float lifetime = 5f;
        public LayerMask hitLayers;

        private Rigidbody _rb;
        private RangedAttack _rangedAttack;

        public void Initialize(RangedAttack rangedAttack)
        {
            _rangedAttack = rangedAttack;
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.velocity = transform.forward * speed;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsInLayerMask(other.gameObject, hitLayers))
            {
                if (other.CompareTag("Player"))
                {
                    _rangedAttack.ProcessCollision(other);
                }

                Destroy(gameObject);
            }
        }

        private static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
        {
            return (layerMask.value & (1 << obj.layer)) != 0;
        }
    }
}