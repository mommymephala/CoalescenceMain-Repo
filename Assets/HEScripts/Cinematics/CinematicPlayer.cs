using HEScripts.UI;
using HorrorEngine;
using UnityEngine;
using UnityEngine.Video;

namespace HEScripts.Cinematics
{
    public class CinematicPlayer : MonoBehaviour
    {
        public void Play(VideoClip clip)
        {
            UIManager.Get<UICinematicPlayer>().Show(clip);
        }
    }
}