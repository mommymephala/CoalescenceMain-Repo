using UnityEngine;

namespace Senses
{
    public class SenseTarget : MonoBehaviour
    {
        public virtual Transform GetTransform()
        {
            return transform;
        }
    }
}