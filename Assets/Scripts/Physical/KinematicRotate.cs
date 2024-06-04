using UnityEngine;

namespace Physical
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicRotate : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float maxRotationSpeed;
        [SerializeField] private float accelerationRate;

        private Rigidbody _rigidbody;
        private float _angle;

        public float RotationSpeed
        {
            get => rotationSpeed;
            set => rotationSpeed = Mathf.Clamp(value, -maxRotationSpeed, maxRotationSpeed);
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
            // Increase rotation speed gradually
            RotationSpeed += accelerationRate * Time.deltaTime;
            RotationSpeed = Mathf.Clamp(RotationSpeed, -maxRotationSpeed, maxRotationSpeed);

            Angle += RotationSpeed * Time.deltaTime;
            
            Quaternion rotation = Quaternion.Euler(0.0f, Angle, 0.0f);
            _rigidbody.MoveRotation(rotation);
        }
    }
}