using HEScripts.Interaction;
using UnityEngine;

namespace HEScripts.UI
{
    public class UIExamineItemRenderer : MonoBehaviour
    {
        public Transform PreviewObject;
        public UnityEngine.Camera Camera;
        public InteractionRaycastDetector InteractionDetector;

        [SerializeField] private float m_RenderTextureScale = 1f;
        [SerializeField] private FilterMode m_RenderTextureFilterMode = FilterMode.Bilinear;

        private RenderTexture m_Texture;

        public RenderTexture Texture => m_Texture;
        
        private void Awake()
        {
            m_Texture = new RenderTexture((int)(Screen.width * m_RenderTextureScale), (int)(Screen.height * m_RenderTextureScale), 16);
            m_Texture.filterMode = m_RenderTextureFilterMode;
            
            Camera.targetTexture = m_Texture;
        }
    }
}
