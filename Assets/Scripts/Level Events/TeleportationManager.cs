using Character_Movement.Components;
using Character_Movement.Controllers;
using UnityEngine;

namespace Level_Events
{
    public class TeleportationManager : MonoBehaviour
    {
        private static TeleportationManager Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public static void TeleportPlayer(Transform playerTransform, Transform targetTransform, Rigidbody playerRb, CustomFirstPersonController playerMovement, CharacterMovement playerController)
        {
            playerController.enabled = false;
            playerMovement.enabled = false;

            RigidbodyInterpolation originalInterpolation = playerRb.interpolation;
            playerRb.interpolation = RigidbodyInterpolation.None;

            Vector3 velocity = playerRb.velocity;
            Vector3 angularVelocity = playerRb.angularVelocity;

            playerTransform.position = targetTransform.position;

            playerTransform.rotation = targetTransform.rotation;


            playerRb.velocity = targetTransform.TransformDirection(velocity);
            playerRb.angularVelocity = angularVelocity;

            playerRb.interpolation = originalInterpolation;

            playerController.enabled = true;
            playerMovement.enabled = true;
        }
    }
}