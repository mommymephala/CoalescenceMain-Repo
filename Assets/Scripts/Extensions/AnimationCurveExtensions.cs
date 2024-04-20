using UnityEngine;

namespace Extensions
{
    public static class AnimationCurveExtensions
    {
        public static float GetDuration(this AnimationCurve curve)
        {
            return curve.keys[curve.length - 1].time;
        }
    }
}