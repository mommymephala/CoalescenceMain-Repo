using UnityEngine;

namespace Combat
{
    public class LaserTrap : AttackBase
    {
        [SerializeField] private LineRenderer laserRenderer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private Color activeColor = Color.red;
        [SerializeField] private Color deactivatedColor = Color.green;
        [SerializeField] private float laserLength;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float explosionRadius;

        private float _maxDamage;
        private const float MinDamage = 10f;

        private bool _isActive = true;

        private readonly Collider[] _colliders = new Collider[10];

        protected override void Awake()
        {
            base.Awake();
            SetLaserColor(activeColor);

            if (m_Attack != null && m_Attack.Impacts.Length > 0)
            {
                _maxDamage = m_Attack.Impacts[0].Damage;
            }
            else
            {
                Debug.LogError("AttackType or its Impacts are not set properly.");
                _maxDamage = 100f; // Fallback value
            }
        }

        private void Start()
        {
            SetLaserColor(activeColor);

            // Set the width of the Line Renderer
            laserRenderer.startWidth = 0.01f;
            laserRenderer.endWidth = 0.01f;

            // Set the positions for the Line Renderer
            var positions = new Vector3[2];
            positions[0] = transform.position;
            positions[1] = transform.position + transform.forward * laserLength;
            laserRenderer.SetPositions(positions);
        }

        public override void StartAttack()
        {
            enabled = true;
        }

        private void Update()
        {
            if (!_isActive) return;
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, laserLength, playerLayer))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Explode();
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

            var numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, _colliders);
            for (var i = 0; i < numColliders; i++)
            {
                Collider nearbyObject = _colliders[i];
                var damageable = nearbyObject.GetComponent<Damageable>();
                if (damageable != null)
                {
                    Vector3 impactPoint = nearbyObject.ClosestPoint(transform.position);
                    Vector3 impactDir = (nearbyObject.transform.position - transform.position).normalized;
                    var distance = Vector3.Distance(transform.position, nearbyObject.transform.position);
                    var damage = CalculateDamage(distance);

                    // Apply calculated damage using a temporary AttackInfo
                    var attackInfo = new AttackInfo
                    {
                        Attack = this,
                        Damageable = damageable,
                        ImpactDir = impactDir,
                        ImpactPoint = impactPoint
                    };

                    var originalDamage = m_Attack.Impacts[0].Damage; // Store the original damage
                    m_Attack.Impacts[0].Damage = damage; // Temporarily set the damage

                    Process(attackInfo);

                    m_Attack.Impacts[0].Damage = originalDamage; // Restore the original damage
                }
            }

            gameObject.SetActive(false);
        }

        private int CalculateDamage(float distance)
        {
            // Linear interpolation between maxDamage and minDamage based on the distance
            var damage = Mathf.Lerp(_maxDamage, MinDamage, distance / explosionRadius);
            return Mathf.Clamp(Mathf.RoundToInt(damage), Mathf.RoundToInt(MinDamage), Mathf.RoundToInt(_maxDamage));
        }

        private void SetLaserColor(Color color)
        {
            laserRenderer.startColor = color;
            laserRenderer.endColor = color;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * laserLength);
        }

        public override void OnAttackNotStarted()
        {
            // Optional: implement if needed
        }
    }
}