using UnityEngine;

namespace HEScripts.Senses
{
    public class SenseTarget : MonoBehaviour
    {
        public virtual Transform GetTransform()
        {
            return transform;
        }
    }
}