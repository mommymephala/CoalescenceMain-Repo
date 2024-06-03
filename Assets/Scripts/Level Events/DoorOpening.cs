using System.Collections;
using Audio;
using UnityEngine;

namespace Level_Events
{
    public class DoorOpening : MonoBehaviour
    {
        public bool isOpen;
        [SerializeField] private float speed = 1f;

        [Header("Sliding Configs")]
        [SerializeField] private Vector3 slideDirection = Vector3.right;
        [SerializeField] private float slideAmount = 3f;

        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private Coroutine _animationCoroutine;

        private void Awake()
        {
            _startPosition = transform.position;
            _endPosition = _startPosition + slideAmount * slideDirection;
        }

        public void Open()
        {
            if (isOpen) return;

            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(SlideDoor(_endPosition));
            AudioManager.Instance.PlayDoorOpen(transform.position);
        }

        public void Close()
        {
            if (!isOpen) return;

            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(SlideDoor(_startPosition));
            AudioManager.Instance.PlayDoorClosed(transform.position);
        }

        private IEnumerator SlideDoor(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.position;
            float distance = Vector3.Distance(startPosition, targetPosition);
            float duration = distance / speed;
            float elapsedTime = 0f;
            isOpen = targetPosition == _endPosition;

            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            _animationCoroutine = null;
        }
    }
}