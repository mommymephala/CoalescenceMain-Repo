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

        private Coroutine _animationCoroutine;

        private void Awake()
        {
            _startPosition = transform.position;
        }

        public void Open()
        {
            if (isOpen) return;
        
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            else
            {
                _animationCoroutine = StartCoroutine(DoSlidingOpen()); 
                AudioManager.Instance.PlayDoorOpen(transform.position);
            }
        }

        private IEnumerator DoSlidingOpen()
        {
            Vector3 endPosition = _startPosition + slideAmount * slideDirection;
            Vector3 startPosition = transform.position;

            float time = 0;
            isOpen = true;
            while (time < 1)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, time);
                yield return null;
                time += Time.deltaTime * speed;
            }
        }

        public void Close()
        {
            if (!isOpen) return;
            
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            _animationCoroutine = StartCoroutine(DoSlidingClose());
            AudioManager.Instance.PlayDoorClosed(transform.position);
        }

        private IEnumerator DoSlidingClose()
        {
            Vector3 endPosition = _startPosition;
            Vector3 startPosition = transform.position;
            float time = 0;

            isOpen = false;

            while (time < 1)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, time);
                yield return null;
                time += Time.deltaTime * speed;
            }
        }
    }
}