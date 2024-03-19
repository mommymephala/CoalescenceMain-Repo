using HorrorEngine;
using UnityEngine;

namespace HEScripts.Graphics
{
    public class ShaderGlobalFloatSetterOnUpdate : ShaderGlobalFloatSetter
    {
        private void Update()
        {
            Shader.SetGlobalFloat(m_PropertyHash, m_Value);
        }
    }
}