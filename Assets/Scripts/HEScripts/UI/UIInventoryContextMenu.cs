using HEScripts.Inventory;
using HEScripts.Items;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HEScripts.UI
{
    public class UIInventoryContextMenu : MonoBehaviour
    {
        public Button UseButton;
        public Button EquipButton;
        public Button ExamineButton;
        public Button CombineButton;
        public Button DropButton;
        public Button MoveButton;

        public UnityEvent OnClose;

        private IUIInput m_Input;
        private Button m_DefaultButton;

        private void Awake()
        {
            m_Input = GetComponentInParent<IUIInput>();

            UseButton.onClick.AddListener(OnOptionSelected);
            EquipButton.onClick.AddListener(OnOptionSelected);
            ExamineButton.onClick.AddListener(OnOptionSelected);
            CombineButton.onClick.AddListener(OnOptionSelected);
            DropButton.onClick.AddListener(OnOptionSelected);
            MoveButton.onClick.AddListener(OnOptionSelected);
        }

        // --------------------------------------------------------------------

        private void OnOptionSelected()
        {
            m_Input.Flush(); // Flush so an object is not immediately selected after this
            gameObject.SetActive(false);
        }

        // --------------------------------------------------------------------

        public bool Show(ItemData item)
        {
            Fill(item);
            if (!m_DefaultButton)
            {
                return false;
            }

            SelectDefault();
            gameObject.SetActive(true);

            return true;
        }

        // --------------------------------------------------------------------

        private void OnDisable()
        {
            OnClose?.Invoke();
        }

        // --------------------------------------------------------------------

        private void Fill(ItemData item)
        {
            m_DefaultButton = null;

            UseButton.gameObject.SetActive(false);
            EquipButton.gameObject.SetActive(false);
            ExamineButton.gameObject.SetActive(false);
            CombineButton.gameObject.SetActive(false);
            DropButton.gameObject.SetActive(false);
            MoveButton.gameObject.SetActive(false);

            if (item.inventoryAction.HasFlag(InventoryMainAction.Use))
            {
                UseButton.gameObject.SetActive(true);
                m_DefaultButton = UseButton;
            }
            
            if(item.inventoryAction.HasFlag(InventoryMainAction.Equip))
            {
                var equipable = item as EquipableItemData;
                if (equipable.Slot != EquipmentSlot.None)
                {
                    EquipButton.gameObject.SetActive(true);
                    if (!m_DefaultButton) m_DefaultButton = EquipButton;
                }
            }

            if (item.flags.HasFlag(ItemFlags.Combinable))
            {
                CombineButton.gameObject.SetActive(true);
                if (!m_DefaultButton) m_DefaultButton = CombineButton;
            }

            if (item.flags.HasFlag(ItemFlags.Examinable))
            {
                ExamineButton.gameObject.SetActive(true);
                if (!m_DefaultButton) m_DefaultButton = ExamineButton;
            }

            if (item.flags.HasFlag(ItemFlags.Droppable))
            {
                DropButton.gameObject.SetActive(true);
                if (!m_DefaultButton) m_DefaultButton = DropButton;
            }
            
            if (item.flags.HasFlag(ItemFlags.Movable))
            {
                MoveButton.gameObject.SetActive(true);
                if (!m_DefaultButton) m_DefaultButton = MoveButton;
            }


            FixNavigation();
        }

        // --------------------------------------------------------------------

        private void FixNavigation() 
        {
            Navigation navigation = new Navigation();
            navigation.mode = Navigation.Mode.Explicit;

            Button[] buttons = GetComponentsInChildren<Button>();
            int lastIndex = buttons.Length - 1;
            for (int i = 0; i < buttons.Length; ++i)
            {
                navigation.selectOnUp = i == 0 ? buttons[lastIndex] : buttons[i - 1];
                navigation.selectOnDown = i == lastIndex ? buttons[0] : buttons[i + 1];
                buttons[i].navigation = navigation;
            }
        }

        // --------------------------------------------------------------------

        private void SelectDefault()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(m_DefaultButton.gameObject);
        }
    }
}