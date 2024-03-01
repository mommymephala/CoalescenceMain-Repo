using Character_Movement.Components;
using Character_Movement.Controllers;
using UnityEngine;

namespace Level_Events
{
    public class SeamlessTeleporter : MonoBehaviour
    {
        public Transform teleportTarget;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var playerRb = other.GetComponent<Rigidbody>();
                var playerMovement = other.GetComponent<CustomFirstPersonController>();
                var playerController = other.GetComponent<CharacterMovement>();

                if (playerRb != null && playerMovement != null)
                {
                    TeleportationManager.TeleportPlayer(other.transform, teleportTarget, playerRb, playerMovement, playerController);
                }
            }
        }
    }
}