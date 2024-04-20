using Systems;

namespace Container
{
    public class GlobalItemContainer : ItemContainerBase
    {
        protected override ContainerData GetData()
        {
            return GameManager.Instance.StorageBox;
        }
    }
}