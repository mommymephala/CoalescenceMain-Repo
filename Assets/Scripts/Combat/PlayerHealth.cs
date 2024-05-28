using UnityEngine;
using UnityEngine.UI;

namespace Combat
{
    public class PlayerHealth : Health
    {
        private Slider _healthSlider;

        private void Awake()
        {
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

        private void UpdateHealthUI(float previousHealth, float currentHealth)
        {
            if (_healthSlider != null)
            {
                _healthSlider.value = currentHealth;
            }
        }
    }
}