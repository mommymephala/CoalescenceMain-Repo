using System;
using System.Collections.Generic;
using Pooling;
using SaveSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
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
        public float significantDamagePercentage;

        public HealthAlteredEvent OnHealthAltered = new HealthAlteredEvent();
        public SignificantDamageEvent OnSignificantDamageTaken = new SignificantDamageEvent();
        public HealthDecreasedEvent OnHealthDecreased = new HealthDecreasedEvent();
        public HealthDepletedEvent OnDeath = new HealthDepletedEvent();
        public UnityEvent OnLoadedDead;
        
        private List<float> damageQueue = new List<float>();
        private float damageAccumulationTime = 0.1f; // Time frame to accumulate damage
        private float lastDamageTime;

        public Damageable LastDamageableHit { get; private set; }
        public float Normalized => Value / Max;
        public bool IsDead => Value <= 0;

        // --------------------------------------------------------------------

        public void OnReset()
        {
            Value = InitialValue > 0 ? InitialValue : Max;
        }

        // --------------------------------------------------------------------
        
        private void Update()
        {
            if (Time.time > lastDamageTime + damageAccumulationTime && damageQueue.Count > 0)
            {
                ApplyAccumulatedDamage();
            }
        }
        
        // --------------------------------------------------------------------

        public void Kill()
        {
            SetHealth(0);
        }

        // --------------------------------------------------------------------
        
        public void TakeDamage(float amount, Damageable damageable = null)
        {
            if (Invulnerable)
                return;

            LastDamageableHit = damageable;
            damageQueue.Add(amount);
            lastDamageTime = Time.time;
        }
        
        private void ApplyAccumulatedDamage()
        {
            float totalDamage = 0;
            foreach (var damage in damageQueue)
            {
                totalDamage += damage;
            }
            
            damageQueue.Clear();

            if (Infinite)
                Value += totalDamage;
            else
                SetHealth(Value - totalDamage);
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
                    var damageTaken = prev - Value;
                    OnHealthDecreased?.Invoke(Value);

                    var significantDamageThreshold = Max * significantDamagePercentage;
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