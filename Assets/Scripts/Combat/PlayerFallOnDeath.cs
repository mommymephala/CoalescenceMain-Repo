using System.Collections;
using Pooling;
using UnityEngine;

namespace Combat
{
    public class PlayerFallOnDeath : MonoBehaviour, IResetable
    {
        public float dropDuration = 1.0f;
        public float rotationDuration = 1.0f;
        public float dropDistance = 1.0f;
        public float rotationAngle = 90.0f;

        private Transform _cameraTransform;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        private void Awake()
        {
            if (Camera.main == null) return;
            
            _cameraTransform = Camera.main.transform;
            _initialPosition = _cameraTransform.localPosition;
            _initialRotation = _cameraTransform.localRotation;
        }

        public void ApplyDeathEffect()
        {
            StartCoroutine(DropCamera());
        }

        private IEnumerator DropCamera()
        {
            var elapsedTime = 0f;

            Vector3 targetPosition = _initialPosition + Vector3.down * dropDistance;

            while (elapsedTime < dropDuration)
            {
                _cameraTransform.localPosition = Vector3.Lerp(_initialPosition, targetPosition, elapsedTime / dropDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _cameraTransform.localPosition = targetPosition;
            
            StartCoroutine(RotateCamera());
        }

        private IEnumerator RotateCamera()
        {
            var elapsedTime = 0f;
            var randomRotationAngle = (Random.Range(0, 2) == 0 ? 1 : -1) * rotationAngle;
            Quaternion targetRotation = Quaternion.Euler(_cameraTransform.localEulerAngles.x, _cameraTransform.localEulerAngles.y, randomRotationAngle);

            while (elapsedTime < rotationDuration)
            {
                _cameraTransform.localRotation = Quaternion.Lerp(_initialRotation, targetRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _cameraTransform.localRotation = targetRotation;
        }

        public void OnReset()
        {
            if (_cameraTransform != null)
            {
                _cameraTransform.localPosition = _initialPosition;
                _cameraTransform.localRotation = _initialRotation;
            }
        }
    }
}