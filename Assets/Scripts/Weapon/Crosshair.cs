using System;
using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Weapon 
{
    public class Crosshair : MonoBehaviour
    {
        [SerializeField] private int range;
        [SerializeField] private Vector2 minSize = new Vector2(87, 87);
        [SerializeField] private Vector2 maxSize = new Vector2(300, 300);
        private GameObject _targetedObject;
        private float _sizeChangeDuration;

        private void Update()
        {
            if (_sizeChangeDuration > 0)
            {
                _sizeChangeDuration -= Time.deltaTime;
            }
        }
        
        public bool IsSizeChangeActive() 
        {
            return _sizeChangeDuration > 0;
        }
        
        public Vector2 GetSize()
        {
            return GetComponent<RectTransform>().sizeDelta;
        }
        
        public void SetSize(Vector2 size, float time = 0.1f)
        {
            Vector2 initialSize = GetComponent<RectTransform>().sizeDelta;
            if (size.x > maxSize.x || size.y > maxSize.y)
            {
                size = new Vector2(maxSize.x, maxSize.y);
            }
            else if (size.x < minSize.x || size.y < minSize.y)
            {
                size = new Vector2(minSize.x, minSize.y);
            }
            Vector2 smoothedSize = Vector2.Lerp(initialSize, size, time);
            GetComponent<RectTransform>().sizeDelta = smoothedSize;
        }
        
        public void MultiplySize(float multiplier, float duration, float time)
        {
            Vector2 initialSize = GetComponent<RectTransform>().sizeDelta;
            var newSize = new Vector2(initialSize.x * multiplier, initialSize.y * multiplier);
            
            if (newSize.x > maxSize.x || newSize.y > maxSize.y)
            {
                newSize = new Vector2(maxSize.x, maxSize.y);
            }
            else if (newSize.x < minSize.x || newSize.y < minSize.y)
            {
                newSize = new Vector2(minSize.x, minSize.y);
            }
            
            Vector2 smoothedSize = Vector2.Lerp(initialSize, newSize, time);
            GetComponent<RectTransform>().sizeDelta = smoothedSize;
            _sizeChangeDuration = duration;
        }
        
        public void SetColor(Color color, float time = 0.1f)
        {
            var myImages = GetComponentsInChildren<Image>();
            foreach (Image image in myImages)
            {
                Color initialColor = image.color;
                Color smoothedColor = Color.Lerp(initialColor, color, time);
                image.color = smoothedColor;
            }
        }
        
        public GameObject GetTarget()
        {
            var rayDirection = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = Camera.main.ScreenPointToRay(rayDirection);
            
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                _targetedObject = hit.transform.gameObject;
                return hit.transform.gameObject;
            }

            _targetedObject = null;
            return null;
        }
        
        public Vector3 GetHitPoint()
        {
            var rayDirection = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = Camera.main.ScreenPointToRay(rayDirection);
            
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                return hit.point;
            }

            return Vector3.zero;
        }
        
        public void SetRange(int range)
        {
            this.range = range;
        }
        
        public int GetRange()
        {
            return range;
        }
    }
}