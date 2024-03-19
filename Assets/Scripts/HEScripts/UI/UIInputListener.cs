using HEScripts.Systems;
using UnityEngine;

namespace HEScripts.UI
{
    public class UIInputListener : MonoBehaviour
    {
        private IUIInput m_Input;

        private UIInventory m_Inventory;
        // private UIMap m_Map;
        
        private void Update()
        {
            if (GameManager.Instance.IsPlaying)
            {
                if (m_Input == null)
                    m_Input = GetComponent<IUIInput>();

                if (m_Input != null)
                {
                    if (m_Input.IsToggleInventoryDown())
                    {
                        if (!m_Inventory)
                            m_Inventory = GetComponentInChildren<UIInventory>(true);

                        m_Inventory?.Show();
                    }

                    // if (m_Input.IsToggleMapDown())
                    // {
                    //     if (!m_Map)
                    //         m_Map = GetComponentInChildren<UIMap>(true);
                    //
                    //     m_Map?.Show();
                    // }
                }
            }
        }
    }
}