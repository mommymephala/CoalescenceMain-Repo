using HEScripts.UI;
using UI_Codebase;
using UnityEngine;
using UnityEngine.Video;

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