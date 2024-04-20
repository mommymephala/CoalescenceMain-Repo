using System;
using HEScripts.Pooling;
using HEScripts.SaveSystem;
using HorrorEngine;
using UnityEngine;
using UnityEngine.Events;

namespace HEScripts.Combat
{
    [Serializable]
    public class HealthDepletedEvent : UnityEvent<Health> { }

    [Serializable]
    public class HealthAlteredEvent : UnityEvent<float, float> { }

    [Serializable]
    public class HealthDecreasedEvent : UnityEvent<float> { }
    
    [Serializable]
    public class SignificantDamageEvent : UnityEvent<float> { }

    public struct HealthSaveData
    {
        public float Value;
    }

    public class Health : MonoBehaviour, IResetable, ISavableObjectStateExtra
    {
        public bool Infinite;
        public bool Invulnerable;
        public float Max;
        public float Min;
        public float Value;
        [Tooltip("Optional value that will used on reset. Leave to 0 to reset to Max")]
        public float InitialValue;

        public HealthAlteredEvent OnHealthAltered = new HealthAlteredEvent();
        public SignificantDamageEvent OnSignificantDamageTaken = new SignificantDamageEvent();
        public HealthDecreasedEvent OnHealthDecreased = new HealthDecreasedEvent();
        public HealthDepletedEvent OnDeath = new HealthDepletedEvent();
        public UnityEvent OnLoadedDead;

        public Damageable LastDamageableHit { get; private set; }
        public bool IsDead => Value <= 0;

        // --------------------------------------------------------------------

        public void OnReset()
        {
            Value = InitialValue > 0 ? InitialValue : Max;
        }

        // --------------------------------------------------------------------

        public void Kill()
        {
            SetHealth(0);
        }

        // --------------------------------------------------------------------

        public void TakeDamage(float amount, Damageable damageable = null)
        {
            LastDamageableHit = damageable;

            if (Invulnerable)
                return;

            if (Infinite)
                Value += amount;

            var previousValue = Value;
            SetHealth(Value - amount);

            float significantDamageThreshold = Max * 0.2f;
            if (amount >= significantDamageThreshold)
            {
                OnSignificantDamageTaken?.Invoke(amount);
            }
        }

        // --------------------------------------------------------------------

        public void Regenerate(float amount)
        {
            SetHealth(Value + amount);
        }

        // --------------------------------------------------------------------

        public void RegenerateAll()
        {
            SetHealth(Max);
        }

        // --------------------------------------------------------------------

        private void SetHealth(float value)
        {
            var prev = Value;
            Value = Mathf.Clamp(value, Min, Max);

            if (prev != Value)
            {
                OnHealthAltered?.Invoke(prev, Value);

                if (prev > Value)
                {
                    OnHealthDecreased?.Invoke(Value);

                    var significantDamageThreshold = Max * 0.2f;
                    var damageTaken = prev - Value;
                    if (damageTaken >= significantDamageThreshold)
                    {
                        OnSignificantDamageTaken?.Invoke(damageTaken);
                    }
                }

                if (IsDead)
                    OnDeath?.Invoke(this);
            }
        }

        // --------------------------------------------------------------------
        // ISavable implementation
        // --------------------------------------------------------------------

        string ISavable<string>.GetSavableData()
        {
            return Value.ToString();
        }

        public void SetFromSavedData(string savedData)
        {
            Value = (float)Convert.ToDouble(savedData);
            if (Value <= 0)
            {
                OnLoadedDead?.Invoke();
            }
        }
    }
}