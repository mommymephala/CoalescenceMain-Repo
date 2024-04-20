using UnityEngine;

namespace SaveSystem
{
    public class Register : ScriptableObject
    {
        public string uniqueId;

        private void OnValidate()
        {
            GenerateId();
        }

        public void GenerateId()
        {
            if (string.IsNullOrEmpty(uniqueId))
                uniqueId = IdUtils.GenerateId();
        }
    }
}