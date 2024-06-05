using UnityEngine;
using UnityEngine.Video;
using UI_Codebase;

namespace Cinematics
{
    public class CinematicPlayer : MonoBehaviour
    {
        public void Play(VideoClip clip)
        {
            UIManager.Get<UICinematicPlayer>().Show(clip);
        }
    }
}