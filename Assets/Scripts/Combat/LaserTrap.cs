using UnityEngine;

namespace Combat
{
    public class LaserTrap : MonoBehaviour
    {
        [SerializeField] private LineRenderer laserRenderer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private Color activeColor = Color.red;
        [SerializeField] private Color deactivatedColor = Color.green;
        [SerializeField] private float laserLength = 10f;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float explosionRadius = 5f;
        [SerializeField] private float explosionDamage = 100f;

        private bool _isActive = true;

        private void Start()
        {
            SetLaserColor(activeColor);
        }

        private void Update()
        {
            if (_isActive)
            {
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, laserLength, playerLayer))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Explode();
                        hit.collider.GetComponent<Health>().Kill();
                    }
                }
            }
        }

        public void Deactivate()
        {
            _isActive = false;
            SetLaserColor(deactivatedColor);
        }

        public void Explode()
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            var colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider nearbyObject in colliders)
            {
                var health = nearbyObject.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(explosionDamage);
                }
            }

            Destroy(gameObject);
        }

        private void SetLaserColor(Color color)
        {
            laserRenderer.startColor = color;
            laserRenderer.endColor = color;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}