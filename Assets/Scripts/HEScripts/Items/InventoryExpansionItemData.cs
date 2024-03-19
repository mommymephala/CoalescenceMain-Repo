using HEScripts.Inventory;
using HEScripts.Systems;
using HEScripts.UI;
using HorrorEngine;

namespace HEScripts.Items
{
    public class InventoryExpansionItemData : ItemData
    {
        public int Amount = 2;

        public override void OnUse(InventoryEntry entry)
        {
            base.OnUse(entry);

            GameManager.Instance.Inventory.Expand(Amount);

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
