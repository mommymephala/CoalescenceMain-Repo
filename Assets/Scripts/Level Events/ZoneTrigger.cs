using UnityEngine;

namespace Level_Events
{
    public class ZoneTrigger : MonoBehaviour
    {
        [SerializeField]
        private string[] objectsToActivate;
        [SerializeField]
        private string[] objectsToDeactivate;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach (var id in objectsToActivate)
                {
                    ActivationManager.Instance.ActivateObject(id);
                }

                foreach (var id in objectsToDeactivate)
                {
                    ActivationManager.Instance.DeactivateObject(id);
                }
            }
        }
    }
}