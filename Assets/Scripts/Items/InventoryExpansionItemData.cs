using Inventory;
using Systems;
using UI_Codebase;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Items/ExpansionItem")]
    public class InventoryExpansionItemData : ItemData
    {
        public int expansionAmount = 2;

        public override void OnUse(InventoryEntry entry)
        {
            base.OnUse(entry);

            GameManager.Instance.Inventory.Expand(expansionAmount);

            UIManager.PushAction(new UIStackedAction()
            {
                Action = () =>
                {
                    UIManager.Get<UIInventory>().Show();
                },
                StopProcessingActions = true,
                Name = "InventoryExpansionItemData.OnUse (Show Inventory)"
            });
        }
    }
}
