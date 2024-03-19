using HEScripts.Inventory;
using HEScripts.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HEScripts.UI
{
    //TODO: INTEGRATE MINECRAFT DRAG AND DROP WITH EQUIPMENT SLOTS
    public class UIInventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private GameObject m_SelectionLocked;
        [SerializeField] private TMPro.TextMeshProUGUI m_ItemName;
        [SerializeField] private TMPro.TextMeshProUGUI m_Count;
        [SerializeField] private string m_NoItemName = "No item";
        [SerializeField] private Color m_NormalAmountColor = Color.green;
        [SerializeField] private Color m_EmptyAmountColor = Color.red;
        [SerializeField] private GameObject m_Status;
        [SerializeField] private Image m_StatusFill;
        [SerializeField] private Gradient m_StatusFillColorOverValue;

        public InventoryEntry InventoryEntry { get; private set; }

        public ItemData Data => InventoryEntry.Item;
        
        // Reference to the original parent in the hierarchy to return the item if drop is not valid.
        private Transform _originalParent;

        // Reference to a temporary clone of the item for visual feedback during drag.
        private GameObject _draggingItem;

        private void Awake()
        {
            _originalParent = transform.parent;
        }

        public void Fill(InventoryEntry entry)
        {
            InventoryEntry = entry;
           
            int amount = 0;
            if (entry != null && entry.Item)
            {
                amount = entry.Item.flags.HasFlag(ItemFlags.Bulkable) ? entry.Count : entry.SecondaryCount;
            }

            FillItem(entry != null ? entry.Item : null, amount, entry != null ? entry.Status : 0f);
           
            if (m_SelectionLocked)
                m_SelectionLocked.gameObject.SetActive(false);
        }

        public void FillItem(ItemData data, int amount = 0, float status = 0)
        {
            if (data)
            {
                m_Icon.sprite = data.image;
                m_Icon.gameObject.SetActive(true);

                m_Count.text = amount.ToString();
                bool isReloadable = data as ReloadableWeaponData;
                m_Count.gameObject.SetActive(data.flags.HasFlag(ItemFlags.Bulkable) || amount > 0 || isReloadable);
                m_Count.color = amount == 0 ? m_EmptyAmountColor : m_NormalAmountColor;

                m_Status.gameObject.SetActive(data.flags.HasFlag(ItemFlags.Depletable));
                m_StatusFill.fillAmount = status;
                m_StatusFill.color = m_StatusFillColorOverValue.Evaluate(status);

                if (m_ItemName)
                {
                    m_ItemName.text = data.Name;
                }
            }
            else
            {
                m_Icon.gameObject.SetActive(false);
                m_Count.gameObject.SetActive(false);
                m_Status.SetActive(false);

                if (m_ItemName)
                {
                    m_ItemName.text = m_NoItemName;
                }

                if (m_SelectionLocked)
                    m_SelectionLocked.gameObject.SetActive(false);
            }
        }

        public void SetSelectionLocked(bool islocked)
        {
            if (m_SelectionLocked)
                m_SelectionLocked.gameObject.SetActive(islocked);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Ignore drag if there's no item in this slot.
            if (InventoryEntry == null || InventoryEntry.Item == null) return;

            _draggingItem = new GameObject("Dragging Item");
            var rt = _draggingItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50); // Adjust size as needed
            var canvas = FindObjectOfType<Canvas>(); // Find the canvas in the scene to ensure correct positioning.
            _draggingItem.transform.SetParent(canvas.transform, false);
            var img = _draggingItem.AddComponent<Image>();
            img.sprite = m_Icon.sprite;
            img.raycastTarget = false; // Make sure the temporary item doesn't interfere with events.

            SetDraggedPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_draggingItem != null)
                SetDraggedPosition(eventData);
        }

        private void SetDraggedPosition(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null && _draggingItem != null)
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)_originalParent, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
                {
                    _draggingItem.transform.position = globalMousePos;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Destroy the temporary item.
            if (_draggingItem != null)
                Destroy(_draggingItem);

            // Check if the item was dropped over an equipment slot or another inventory slot and handle accordingly.
            // This may involve calling a method on your GameManager or Inventory system to move the item.
        }

        public void OnDrop(PointerEventData eventData)
        {
            // Handle an item being dropped onto this slot.
            // This will involve identifying the item being dropped and, if valid,
            // updating the inventory system to reflect the item's new location.

            var droppedItem = eventData.pointerDrag.GetComponent<UIInventoryItem>();
            if (droppedItem != null)
            {
                // Example: Swap items between the two slots.
                // This requires additional logic in your Inventory system to support such an operation.
                Debug.Log($"Dropped item {droppedItem.Data.Name} onto {Data?.Name}");
                // YourInventorySystem.SwapItems(droppedItem.InventoryEntry, InventoryEntry);
            }
        }
    }
}