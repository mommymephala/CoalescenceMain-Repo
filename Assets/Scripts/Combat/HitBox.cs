using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Combat
{
    public class HitBox : MonoBehaviour
    {
        [SerializeField] private Vector3 m_Center;
        [SerializeField] private Vector3 m_Size = Vector3.one;
        [SerializeField] private LayerMask m_LayerMask;

        // --------------------------------------------------------------------

        public bool CheckForPlayer(out Damageable playerDamageable)
        {
            playerDamageable = null;

            var scaledCenter = new Vector3(m_Center.x * transform.lossyScale.x, m_Center.y * transform.lossyScale.y, m_Center.z * transform.lossyScale.z);
            var scaledSize = new Vector3(m_Size.x * transform.lossyScale.x, m_Size.y * transform.lossyScale.y, m_Size.z * transform.lossyScale.z);

            DebugUtils.DrawBox(transform.position + scaledCenter, transform.rotation, scaledSize, Color.red, 10);
            var results = Physics.OverlapBox(transform.position + scaledCenter, scaledSize * 0.5f, transform.rotation, m_LayerMask, QueryTriggerInteraction.Collide);

            foreach (Collider result in results)
            {
                if (result.CompareTag("PlayerDamageable") && result.TryGetComponent(out Damageable d))
                {
                    playerDamageable = d;
                    return true;
                }
            }

            return false;
        }

        // --------------------------------------------------------------------

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.red;

                var scaledCenter = new Vector3(m_Center.x * transform.lossyScale.x, m_Center.y * transform.lossyScale.y, m_Center.z * transform.lossyScale.z);
                var scaledSize = new Vector3(m_Size.x * transform.lossyScale.x, m_Size.y * transform.lossyScale.y, m_Size.z * transform.lossyScale.z);

                Gizmos.matrix = Matrix4x4.Rotate(transform.rotation);
                Gizmos.DrawWireCube(transform.position + scaledCenter, scaledSize);
                Gizmos.matrix = Matrix4x4.identity;

                Gizmos.color = Color.white;
            }
        }
    }
}