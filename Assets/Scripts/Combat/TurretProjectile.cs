using Audio;
using UnityEngine;

namespace Combat
{
    public class TurretProjectile : Projectile
    {
        protected override void OnTriggerEnter(Collider other)
        {
            if (IsInLayerMask(other.gameObject, hitLayers))
            {
                if (other.CompareTag("Player"))
                {
                    _rangedAttack.ProcessCollision(other);
                }
        
                Debug.Log(other.gameObject.name);
            }
    
            if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                AudioManager.Instance.PlayTurretShotImpact(transform.position);
                Destroy(gameObject, 1f);
            }
        }
    }
}