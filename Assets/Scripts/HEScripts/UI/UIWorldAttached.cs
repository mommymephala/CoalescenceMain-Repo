using UnityEngine;

namespace HEScripts.UI
{
    public class UIWorldAttached : MonoBehaviour
    {
        [SerializeField] Transform m_AttachedTo;
        [SerializeField] Vector3 m_BaseOffset;

        private void Update()
        {
            if (!m_AttachedTo)
            {
                enabled = false;
                return;
            }

            transform.position = UnityEngine.Camera.main.WorldToScreenPoint(m_AttachedTo.position + m_BaseOffset);
        }

        public void Attach(Transform attachedTo) 
        {
            m_AttachedTo = attachedTo;
            enabled = true;
        }
    }
}