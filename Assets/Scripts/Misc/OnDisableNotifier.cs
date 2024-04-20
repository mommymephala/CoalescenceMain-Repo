using System;
using UnityEngine;

namespace Misc
{
    public class OnDisableNotifier : MonoBehaviour
    {
        private Action<OnDisableNotifier> mOnDisableCallback;

        public void AddCallback(Action<OnDisableNotifier> callback)
        {
            mOnDisableCallback += callback;
        }

        public void RemoveCallback(Action<OnDisableNotifier> callback)
        {
            mOnDisableCallback -= callback;
        }

        private void OnDisable()
        {
            if (mOnDisableCallback != null)
            {
                mOnDisableCallback(this);
            }
        }
    }
}