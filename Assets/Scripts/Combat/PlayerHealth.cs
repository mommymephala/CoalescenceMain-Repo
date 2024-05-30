using Audio;
using Character_Movement.Components;
using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Combat
{
    public class PlayerHealth : Health
    {
        private Slider _healthSlider;
        private CharacterMovement _controller;

        private void Awake()
        {
            _controller = GetComponent<CharacterMovement>();
            
            GameObject healthSliderObject = GameObject.Find("HealthSlider");
            if (healthSliderObject != null)
            {
                _healthSlider = healthSliderObject.GetComponent<Slider>();
                _healthSlider.maxValue = Max;
                _healthSlider.value = Value;
            }
            else
            {
                Debug.LogWarning("HealthSlider object not found in the scene.");
            }
        }

        private void Start()
        {
            if (_healthSlider != null)
            {
                OnHealthAltered.AddListener(UpdateHealthUI);
                UpdateHealthUI(Value, Value);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.K))
            {
                Kill();
            }
        }

        private void UpdateHealthUI(float previousHealth, float currentHealth)
        {
            if (_healthSlider != null)
            {
                _healthSlider.value = currentHealth;
            }
        }

        protected override void SetHealth(float value)
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
                    AudioManager.Instance.PlayPlayerHurt(transform.position);

                    var significantDamageThreshold = Max * significantDamagePercentage;
                    if (damageTaken >= significantDamageThreshold)
                    {
                        OnSignificantDamageTaken?.Invoke(damageTaken);
                        AudioManager.Instance.PlayPlayerHurt(transform.position);
                    }
                }

                if (IsDead)
                {
                    GameManager.Instance.IsPlaying = false;
                    OnDeath?.Invoke(this);
                    _controller.Pause(true, false);
                    AudioManager.Instance.StopAmbientSound();
                    AudioManager.Instance.PlayPlayerDeath(transform.position);
                }
            }
        }
    }
}