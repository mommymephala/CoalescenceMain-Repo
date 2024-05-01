using UnityEngine;

namespace Physical
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicMove : MonoBehaviour
    {

        [SerializeField]
        public float moveTime = 3.0f;

        [SerializeField]
        private Vector3 offset;
        
        private Rigidbody _rigidbody;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        
        public float MoveTime
        {
            get => moveTime;
            set => moveTime = Mathf.Max(1.0f, value);
        }

        public Vector3 Offset
        {
            get => offset;
            set => offset = value;
        }

        public void OnValidate()
        {
            MoveTime = moveTime;
        }

        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;

            _startPosition = transform.position;
            _targetPosition = _startPosition + Offset;
        }

        public void FixedUpdate()
        {
            var t = Common.Utils.EaseInOut(Mathf.PingPong(Time.time, moveTime), moveTime);
            Vector3 p = Vector3.Lerp(_startPosition, _targetPosition, t);

            _rigidbody.MovePosition(p);
        }
    }
}
