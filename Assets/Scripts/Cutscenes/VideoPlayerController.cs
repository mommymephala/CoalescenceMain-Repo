using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Cutscenes
{
    public class VideoPlayerController : MonoBehaviour
    {
        public VideoClip videoClip;
        public string nextSceneName;
        public float delayBeforeTransition = 1f;

        private VideoPlayer _videoPlayer;

        private void Start()
        {
            _videoPlayer = GetComponent<VideoPlayer>();

            _videoPlayer.loopPointReached += OnVideoFinished;

            _videoPlayer.clip = videoClip;

            _videoPlayer.Play();
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            StartCoroutine(DelayedSceneTransition());
        }

        private IEnumerator DelayedSceneTransition()
        {
            yield return new WaitForSeconds(delayBeforeTransition);

            SceneManager.LoadScene(nextSceneName);
        }
    }
}