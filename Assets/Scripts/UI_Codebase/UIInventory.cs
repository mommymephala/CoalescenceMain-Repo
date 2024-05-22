using System;
using System.Collections;
using Extensions;
using FMODUnity;
using Inventory;
using Items;
using Pickups;
using Systems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI_Codebase
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private GameObject m_Content;
        [SerializeField] private UIInventoryContextMenu m_ContextMenu;
        [SerializeField] private UIInventoryItem[] m_ItemSlots;
        [SerializeField] private TMPro.TextMeshProUGUI m_ItemName;
        [SerializeField] private TMPro.TextMeshProUGUI m_ItemDesc;
        [SerializeField] private UIInventoryItem m_Equipped;
        [SerializeField] private UIInventoryItem m_EquippedSecondary;
        [SerializeField] private UIInventoryItem m_FirstItemForSwap;
        [SerializeField] private float m_ExpandingInteractionDelay = 1f;

        [Header("Audio")]
        [SerializeField] private EventReference m_ShowClip;
        [SerializeField] private EventReference m_UseClip;
        [SerializeField] private EventReference m_CantUseClip;
        [SerializeField] private EventReference m_NavigateClip;
        [SerializeField] private EventReference m_CloseClip;
        [SerializeField] private EventReference m_ExpandClip;

        private UIInventoryItem m_SelectedSlot;
        private UIInventoryItem m_CombiningSlot;

        private IUIInput m_Input;
        private bool m_Expanding;

        // --------------------------------------------------------------------

        protected void Awake()
        {
            m_Input = GetComponentInParent<IUIInput>();

            foreach (UIInventoryItem slot in m_ItemSlots)
            {
                RegisterItemCallbacks(slot);
            }

            // Register callbacks for equipped slots
            RegisterItemCallbacks(m_Equipped);
            RegisterItemCallbacks(m_EquippedSecondary);

            m_ContextMenu.UseButton.onClick.AddListener(OnUse);
            m_ContextMenu.EquipButton.onClick.AddListener(OnEquip);
            m_ContextMenu.CombineButton.onClick.AddListener(OnCombine);
            m_ContextMenu.ExamineButton.onClick.AddListener(OnExamine);
            m_ContextMenu.DropButton.onClick.AddListener(OnDrop);
            m_ContextMenu.MoveButton.onClick.AddListener(OnSwap);
            // m_ContextMenu.OnClose.AddListener(SelectDefault);
        }

        // --------------------------------------------------------------------

        private void RegisterItemCallbacks(UIInventoryItem slot)
        {
            var selectable = slot.GetComponent<UISelectableCallbacks>();
            selectable.OnSelected.AddListener(OnSlotSelected);

            var pointerEvents = slot.GetComponent<UIPointerClickEvents>();
            pointerEvents.OnClick.AddListener(() => OnSlotSelected(slot.gameObject));
            pointerEvents.OnDoubleClick.AddListener(OnSubmit);
        }

        // --------------------------------------------------------------------

        private void OnSlotSelected(GameObject obj)
        {
            var slot = obj.GetComponent<UIInventoryItem>();
            if (slot != null && slot.InventoryEntry != null && slot.InventoryEntry.Item != null)
            {
                m_SelectedSlot = slot;
                m_ItemName.text = slot.InventoryEntry.Item.Name;
                m_ItemDesc.text = slot.InventoryEntry.Item.description;
                UIManager.Get<UIAudio>().Play(m_NavigateClip);
                Debug.Log("burdayım");
                Debug.Log(m_NavigateClip);
            }
            else
            {
                ClearSelection();
            }

            StartCoroutine(SetSelectedGameObjectAfterFrame(obj));
        }

        private void ClearSelection()
        {
            m_SelectedSlot = null;
            m_ItemName.text = "";
            m_ItemDesc.text = "";
            
            if (m_CombiningSlot != null)
            {
                CancelCombine();
            }
            
            m_ContextMenu.gameObject.SetActive(false); // Ensure the context menu is closed if open
        }

        private static IEnumerator SetSelectedGameObjectAfterFrame(GameObject obj)
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(obj);
        }

        // --------------------------------------------------------------------

        private void Start()
        {
            gameObject.SetActive(false);
        }

        // --------------------------------------------------------------------

        private void Update()
        {
            if (m_Expanding)
                return;

            if (m_Input.IsCancelDown() || m_Input.IsToggleInventoryDown())
            {
                OnCancel();
            }

            if (Input.GetMouseButtonDown(1) && m_SelectedSlot != null)
            {
                ClearSelection();
            }

            if (m_SelectedSlot && m_SelectedSlot.InventoryEntry != null)
            {
                if (m_Input.IsConfirmDown())
                {
                    OnSubmit();
                }
            }
        }

        // --------------------------------------------------------------------

        public void Show()
        {
            PauseController.Instance.Pause();
            CursorController.Instance.SetInUI(true);
            
            m_SelectedSlot = null;
            m_Expanding = false;

            gameObject.SetActive(true); // Needs to happen before fill or animations wont play

            Fill();
            FillEquipped();
            // SelectDefault();

            m_ContextMenu.gameObject.SetActive(false);

            m_CombiningSlot = null;
            UIManager.Get<UIAudio>().Play(m_ShowClip);

            if (m_Expanding)
            {
                UIManager.Get<UIAudio>().Play(m_ExpandClip);
                this.InvokeActionUnscaled(EndExpansion, m_ExpandingInteractionDelay);
            }
        }

        // --------------------------------------------------------------------

        private void EndExpansion()
        {
            m_Expanding = false;
        }

        // --------------------------------------------------------------------

        private void Hide()
        {
            PauseController.Instance.Resume();
            CursorController.Instance.SetInUI(false);
            m_ContextMenu.gameObject.SetActive(false);
            gameObject.SetActive(false);

            UIManager.PopAction();
        }

        // --------------------------------------------------------------------

        private void OnCancel()
        {
            UIManager.Get<UIAudio>().Play(m_CloseClip);
            if (m_CombiningSlot)
            {
                CancelCombine();
            }
            
            else if (m_ContextMenu.isActiveAndEnabled)
            {
                m_ContextMenu.gameObject.SetActive(false);
            }
            
            else
            {
                Hide();
            }
        }

        // --------------------------------------------------------------------

        private void CancelCombine()
        {
            m_CombiningSlot.SetSelectionLocked(false);
            m_CombiningSlot = null;
        }

        // --------------------------------------------------------------------

        private void OnSubmit()
        {
            if (m_SelectedSlot == null || m_SelectedSlot.InventoryEntry == null || m_SelectedSlot.InventoryEntry.Item == null)
                return;

            ItemData item = m_SelectedSlot.InventoryEntry.Item;

            if (m_CombiningSlot && m_SelectedSlot != m_CombiningSlot)
            {
                Combine();
            }
            else if (!m_ContextMenu.Show(item))
            {
                OnUse();
            }
        }

        // --------------------------------------------------------------------

        private void OnUse()
        {
            if (GameManager.Instance.Inventory.Use(m_SelectedSlot.InventoryEntry))
            {
                UIManager.Get<UIAudio>().Play(m_UseClip);

                ItemData item = m_SelectedSlot.InventoryEntry.Item;
                if (item && item.flags.HasFlag(ItemFlags.UseOnInteractive))
                {
                    Hide();
                }
                else
                {
                    Fill();
                    FillEquipped();
                }
            }
            else
            {
                UIManager.Get<UIAudio>().Play(m_CantUseClip);
            }

            m_Input.Flush();
        }

        // --------------------------------------------------------------------

        private void OnEquip()
        {
            var item = m_SelectedSlot.InventoryEntry.Item as EquipableItemData;
            if (item != null) item.Equip(m_SelectedSlot.InventoryEntry);
            UIManager.Get<UIAudio>().Play(m_UseClip);

            Fill();
            FillEquipped();

            m_Input.Flush();
        }

        // --------------------------------------------------------------------

        private void OnCombine()
        {
            if (m_CombiningSlot)
            {
                if (m_SelectedSlot.InventoryEntry.Item)
                    Combine();
                else
                    CancelCombine();
            }
            else if (m_SelectedSlot.InventoryEntry.Item)
            {
                // First item of the combination picked
                m_SelectedSlot.SetSelectionLocked(true);
                m_CombiningSlot = m_SelectedSlot;
                EventSystem.current.SetSelectedGameObject(m_SelectedSlot.gameObject);
            }
        }


        // --------------------------------------------------------------------

        private void OnExamine()
        {
            ItemData item = m_SelectedSlot.InventoryEntry.Item;
            if (item == null)
                return;

            Hide();

            UIManager.Get<UIExamineItem>().Show(item);
        }

        // --------------------------------------------------------------------

        private void OnDrop()
        {
            ItemData item = m_SelectedSlot.InventoryEntry.Item;
            if (item == null)
                return;

            GameManager gameMgr = GameManager.Instance;
            
            if (item.flags.HasFlag(ItemFlags.CreatePickupOnDrop))
            {
                gameMgr.Player.GetComponentInChildren<PickupDropper>().Drop(m_SelectedSlot.InventoryEntry);
            }

            gameMgr.Inventory.Remove(m_SelectedSlot.InventoryEntry, m_SelectedSlot.InventoryEntry.Count); 

            Hide();
        }
        
        // --------------------------------------------------------------------
        
        private void OnSwap()
        {
            if (m_FirstItemForSwap == null)
            {
                m_FirstItemForSwap = m_SelectedSlot;
                // TODO: Add visual feedback
            }
            else
            {
                // Second item selected, perform the swap
                var index1 = Array.IndexOf(m_ItemSlots, m_FirstItemForSwap);
                var index2 = Array.IndexOf(m_ItemSlots, m_SelectedSlot);
                GameManager.Instance.Inventory.SwapItems(index1, index2);

                // Reset and refresh UI
                m_FirstItemForSwap = null;
                Fill();
                FillEquipped();
                // SelectDefault();
            }

            m_ContextMenu.gameObject.SetActive(false);
        }
        
        // private void ResetSwapSelection()
        // {
        //     m_FirstItemForSwap = null;
        //     // Optionally, remove any visual indication that an item was selected for swapping
        // }

        // --------------------------------------------------------------------

        public void OnFilesCategory()
        {
            Hide();
            UIManager.Get<UIDocs>().Show();
        }
        
        // --------------------------------------------------------------------

        private void Combine()
        {
            InventoryEntry highlightedEntry = GameManager.Instance.Inventory.Combine(m_SelectedSlot.InventoryEntry, m_CombiningSlot.InventoryEntry);
            
            Fill();
            FillEquipped();

            if (highlightedEntry != null)
            {
                for (var i = 0; i < m_ItemSlots.Length; ++i)
                {
                    if (m_ItemSlots[i].InventoryEntry == highlightedEntry)
                    {
                        EventSystem.current.SetSelectedGameObject(m_ItemSlots[i].gameObject);
                    }
                }
            }
            else
            {
                // SelectDefault();
            }

            m_CombiningSlot = null;
        }

        // --------------------------------------------------------------------

        private void Fill()
        {
            Inventory.Inventory inventory = GameManager.Instance.Inventory;
            var items = inventory.Items;
            var itemsCount = items.Length;

            m_Expanding = inventory.Expanded;
            inventory.Expanded = false;

            for (var i = 0; i < items.Length; ++i)
            {
                m_ItemSlots[i].gameObject.SetActive(i < itemsCount);

                if (i >= inventory.PreExpansionSize && m_Expanding)
                {
                    m_ItemSlots[i].GetComponent<Animator>().Play("Expand");
                }

                m_ItemSlots[i].Fill(items[i]);
            }

            for (var i = items.Length; i < m_ItemSlots.Length; ++i)
            {
                m_ItemSlots[i].gameObject.SetActive(false);
            }
        }

        // --------------------------------------------------------------------

        private void FillEquipped()
        {
            FillEquipped(EquipmentSlot.Primary, m_Equipped);
            FillEquipped(EquipmentSlot.Secondary, m_EquippedSecondary);
        }

        // --------------------------------------------------------------------

        private void FillEquipped(EquipmentSlot slot, UIInventoryItem uiItem)
        {
            InventoryEntry equippedEntry = GameManager.Instance.Inventory.GetEquipped(slot);
            if (equippedEntry != null)
            {
                uiItem.Fill(equippedEntry);
                uiItem.gameObject.SetActive(true);
                RegisterItemCallbacks(uiItem);  // Make sure callbacks are registered
            }
            else
            {
                uiItem.Fill(null);
                // uiItem.gameObject.SetActive(false);  // Deactivate the slot if it is empty
            }
        }

        // --------------------------------------------------------------------

        // private void SelectDefault()
        // {
        //     EventSystem.current.SetSelectedGameObject(null);
        //     EventSystem.current.SetSelectedGameObject(m_SelectedSlot ? m_SelectedSlot.gameObject : m_ItemSlots[0].gameObject);
        // }
    }
}