using System.Collections;
using Audio;
using UnityEngine;
using UnityEngine.Events;

namespace Physical
{
    [RequireComponent(typeof(Rigidbody))]
    public class Elevator : MonoBehaviour
    {
        [SerializeField] private AudioManager.ElevatorType elevatorType;
        
        [SerializeField] private float moveTime = 3.0f;
        [SerializeField] private Vector3 moveDirection = Vector3.up;
        [SerializeField] private float moveDistance = 5.0f;
        [SerializeField] private float initialDelay = 2.0f;

        private Rigidbody _rigidbody;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _currentLerpTime;
        private bool _isActive;
        private bool _isAvailable = true;

        public UnityEvent onElevatorStopped;
        public UnityEvent onElevatorActivated;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }

        private void Start()
        {
            InitializePositions();
        }

        private void InitializePositions()
        {
            _startPosition = transform.position;
            _targetPosition = _startPosition + moveDirection.normalized * moveDistance;
        }

        private void FixedUpdate()
        {
            if (!_isActive) return;

            _currentLerpTime += Time.deltaTime;
            if (_currentLerpTime >= moveTime)
            {
                _isActive = false;
                _currentLerpTime = 0;
                SetAvailability(false);
                onElevatorStopped.Invoke();
                
                // Reverse direction
                (_startPosition, _targetPosition) = (_targetPosition, _startPosition);
            }
            else
            {
                var perc = _currentLerpTime / moveTime;
                Vector3 newPos = Vector3.Lerp(_startPosition, _targetPosition, perc);
                _rigidbody.MovePosition(newPos);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_isActive && _isAvailable)
            {
                StartCoroutine(ActivateElevator());
            }
        }

        private IEnumerator ActivateElevator()
        {
            onElevatorActivated.Invoke();
            AudioManager.Instance.PlayElevatorActivation(elevatorType, transform.position);
            yield return new WaitForSeconds(initialDelay);
            _isActive = true;
        }

        public void SetAvailability(bool available)
        {
            _isAvailable = available;
        }

        public void CallElevator()
        {
            Debug.Log("called");
            if (!_isActive && _isAvailable)
            {
                _isActive = true;
                AudioManager.Instance.PlayElevatorActivation(elevatorType, transform.position);
                var perc = _currentLerpTime / moveTime;
                Vector3 newPos = Vector3.Lerp(_startPosition, _targetPosition, perc);
                _rigidbody.MovePosition(newPos);
                // Debug.Log("called correctly");
            }
        }
    }
}