using UnityEngine;

namespace Level_Events
{
    public class DoorTrigger : MonoBehaviour
    {
        [SerializeField] private DoorOpening[] doorOpenings;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            foreach (DoorOpening doorOpening in doorOpenings)
            {
                if (!doorOpening.isOpen)
                {
                    doorOpening.Open();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            foreach (DoorOpening doorOpening in doorOpenings)
            {
                if (doorOpening.isOpen)
                {
                    doorOpening.Close();
                }
            }
        }
    }
}