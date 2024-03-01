using UnityEngine;

namespace HEScripts.Graphics
{
    public class ShaderGlobalFloatSetter : MonoBehaviour
    {
        [SerializeField] private string m_Property;
        [SerializeField] protected float m_Value;

        protected int m_PropertyHash = 0;

        private void Awake()
        {
            m_PropertyHash = Shader.PropertyToID(m_Property);
        }

        private void OnEnable()
        {
            Shader.SetGlobalFloat(m_PropertyHash, m_Value);
        }

        private void Update()
        {
            Shader.SetGlobalFloat(m_PropertyHash, m_Value);
        }
    }
}
