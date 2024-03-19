using HEScripts.Messages;
using HEScripts.Singleton;
using UnityEngine;

namespace HEScripts.Systems
{
    public class GamePausedMessage : BaseMessage
    {
        public static GamePausedMessage Default = new GamePausedMessage();
    }

    public class GameUnpausedMessage : BaseMessage
    {
        public static GameUnpausedMessage Default = new GameUnpausedMessage();
    }

    public class PauseController : SingletonBehaviourDontDestroy<PauseController>
    {
        private int _mPauseCount;

        public bool IsPaused => _mPauseCount > 0;

        public void Pause()
        {
            ++_mPauseCount;
            if (_mPauseCount == 1)
                MessageBuffer<GamePausedMessage>.Dispatch(GamePausedMessage.Default);
            
            Time.timeScale = 0f;
        }

        // --------------------------------------------------------------------

        public void Resume()
        {
            --_mPauseCount;

            if (_mPauseCount <= 0)
            {
                MessageBuffer<GameUnpausedMessage>.Dispatch(GameUnpausedMessage.Default);
                Time.timeScale = 1f;
            }

            Debug.Assert(_mPauseCount >= 0, "PauseController:  PauseCount went below 0");
        }

#if UNITY_EDITOR
        public void Update()
        {
            if (Input.GetKey(KeyCode.Numlock))
            {
                Debug.Break();
            }
        }
#endif
    }
}