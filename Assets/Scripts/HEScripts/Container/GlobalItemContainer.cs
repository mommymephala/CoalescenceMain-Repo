using HEScripts.Systems;
using HorrorEngine;

namespace HEScripts.Container
{
    public class GlobalItemContainer : ItemContainerBase
    {
        protected override ContainerData GetData()
        {
            return GameManager.Instance.StorageBox;
        }
    }
}