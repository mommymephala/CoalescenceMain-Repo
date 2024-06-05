using Extensions;
using Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace UI_Codebase
{
    public class UICinematicPlayer : MonoBehaviour
    {
        private VideoPlayer m_Player;

        private void Awake()
        {
            m_Player = GetComponentInChildren<VideoPlayer>();
            m_Player.loopPointReached += OnVideoFinished;
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Show(VideoClip clip)
        {
            PauseController.Instance.Pause();
            gameObject.SetActive(true);

            m_Player.clip = clip;
            m_Player.Play();

            this.InvokeActionUnscaled(Hide, (float)clip.length);
        }

        public void Hide()
        {
            PauseController.Instance.Resume();
            gameObject.SetActive(false);
            UIManager.PopAction();
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            SceneManager.LoadScene("MainMenuTest");
        }
    }
}