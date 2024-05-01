using UnityEngine;

namespace Physical
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicRotate : MonoBehaviour
    {
        [SerializeField]
        private float rotationSpeed = 30.0f;

        private Rigidbody _rigidbody;

        private float _angle;

        public float RotationSpeed
        {
            get => rotationSpeed;
            set => rotationSpeed = Mathf.Clamp(value, -360.0f, 360.0f);
        }

        public float Angle
        {
            get => _angle;
            set => _angle = Common.Utils.WrapAngle(value);
        }

        public void OnValidate()
        {
            RotationSpeed = rotationSpeed;
        }

        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }

        public void FixedUpdate()
        {
            Angle += RotationSpeed * Time.deltaTime;
            
            Quaternion rotation = Quaternion.Euler(0.0f, Angle, 0.0f);
            _rigidbody.MoveRotation(rotation);
        }
    }
}
