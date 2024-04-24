using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
    
        public enum AttackType
        {
            NormalAttack,
            HeavyAttack,
        }
        
        public enum EnemyType
        {
            BaseEnemy,
            ChipEnemy,
        }
        
        [System.Serializable]
        public class EnemySounds
        {
            public EventReference footstep;
            //  public EventReference takeDamage;
            public EventReference normalAttack;
            public EventReference heavyAttack;
            public EventReference death;
            public EventReference idle;
        }
        
        //[SerializeField] private EventReference ambient;
        //[SerializeField] private EventReference safeRoom;
        
        [Header("Player")]
        [SerializeField] private EventReference playerFootsteps;
        [SerializeField] private EventReference playerRunning;
        [SerializeField] private EventReference playerTakeDamage;
        [SerializeField] private EventReference playerDeath;
        
        [Header("Player Footsteps")]
        public float footstepTimer;
        public float footstepDelay = 0.5f;
        public float runningFootstepDelay = 0.25f;
        
        [Header("Enemy Sounds")]
        public EnemySounds baseEnemySounds;
        public EnemySounds chipEnemySounds;
        
        [Header("Environment")]
        // [SerializeField] private EventReference weaponSwitch;
        [SerializeField] private EventReference metalDoor;
        [SerializeField] private EventReference metalDoorClosed;
        [SerializeField] private EventReference powerCore;
        // [SerializeField] private EventReference playerHurt;
        private EventInstance _playerFootstepInstance;
        private EventInstance _playerTakeDamage;
        private EventInstance _playerRunning;

        private EventInstance _baseEnemyFootstepInstance;
        // private EventInstance _Ambient;
        //private EventInstance _SafeRoom;
    
        public Dictionary<EnemyType, EnemySounds> enemySoundsMap;
        //private bool _isInsideSafeRoom = false;
    
        public float crossfadeDuration = 1.0f;
    
        public Dictionary<EventInstance, Coroutine> fadeCoroutines = new Dictionary<EventInstance, Coroutine>();
        private Coroutine idleCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            //  DontDestroyOnLoad(gameObject);

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            enemySoundsMap = new Dictionary<EnemyType, EnemySounds>
            {
                { EnemyType.BaseEnemy, baseEnemySounds },
                { EnemyType.ChipEnemy, chipEnemySounds }
                // Add other enemies here
            }; 
            //_Ambient = RuntimeManager.CreateInstance(ambient);
            //_Ambient.start();
            //DontDestroyOnLoad(this);
        }
        public void Crossfade(EventInstance fromInstance, EventInstance toInstance, float duration)
        {
            // Stop existing fade on the 'from' instance
            if (fadeCoroutines.TryGetValue(fromInstance, out Coroutine existingCoroutine))
            {
                StopCoroutine(existingCoroutine);
                fadeCoroutines.Remove(fromInstance);
            }

            // Start new crossfade
            Coroutine newCoroutine = StartCoroutine(CrossfadeCoroutine(fromInstance, toInstance, duration));
            fadeCoroutines[toInstance] = newCoroutine;
        }

        private IEnumerator CrossfadeCoroutine(EventInstance fromInstance, EventInstance toInstance, float duration)
        {
            float currentTime = 0;

            // Ensure the 'to' instance is playing
            toInstance.start();

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / duration;

                // Adjust volumes for crossfade
                fromInstance.setVolume(1 - t);
                toInstance.setVolume(t);

                yield return null;
            }

            // Stop the 'from' instance after fading
            fromInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            fromInstance.release();
            fadeCoroutines.Remove(toInstance);
        }

        // Method to pause an event instance
        public void PauseEventInstance(EventInstance instance)
        {
            instance.setPaused(true);
        }

        // Method to resume an event instance
        public void ResumeEventInstance(EventInstance instance)
        {
            instance.setPaused(false);
        }
        
        /*private void PauseSound(EventInstance sound)
        {
            sound.setPaused(true);
        }

        private void ResumeSound(EventInstance sound)
        {
            sound.setPaused(false);
        }

        public void EnterSafeRoom()
        {
            if (!_isInsideSafeRoom)
            {
                FadeOutSound(_Ambient, crossfadeDuration, true);
                _SafeRoom = RuntimeManager.CreateInstance(safeRoom);
                _SafeRoom.setVolume(0);
                _SafeRoom.start();
                FadeInSound(_SafeRoom, crossfadeDuration);
                _isInsideSafeRoom = true;
            }
        }

        public void ExitSafeRoom()
        {
            if (_isInsideSafeRoom)
            {
                FadeOutSound(_SafeRoom, crossfadeDuration);
                ResumeSound(_Ambient);
                FadeInSound(_Ambient, crossfadeDuration);
                _isInsideSafeRoom = false;
            }
        }

        private IEnumerator StartFade(EventInstance sound, float targetVolume, float duration, bool pauseOnFadeOut = false)
        {
            float currentTime = 0;
            sound.getVolume(out float startVolume);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
                sound.setVolume(newVolume);
                yield return null;
            }
            sound.setVolume(targetVolume);

            if (targetVolume == 0 && pauseOnFadeOut)
            {
                PauseSound(sound);
            }
        }

        private void FadeInSound(EventInstance sound, float duration)
        {
            StartCoroutine(StartFade(sound, 1.0f, duration));
        }

        private void FadeOutSound(EventInstance sound, float duration, bool pauseOnFadeOut = false)
        {
            StartCoroutine(StartFade(sound, 0.0f, duration, pauseOnFadeOut));
        }*/
    
        public void PlayFootstep()
        { 
            if (playerFootsteps.IsNull)
            {
                Debug.LogWarning("Fmod event not found: playerFootstep");
                return;
            }
            _playerFootstepInstance = RuntimeManager.CreateInstance(playerFootsteps);
        
            _playerFootstepInstance.start();
            _playerFootstepInstance.release();
        
        }
        public void PlayRunning()
        {
            if (playerRunning.IsNull)
            {
                Debug.LogWarning("Fmod event not found: player running");
                return;
            }
            _playerRunning = RuntimeManager.CreateInstance(playerRunning);
        
            _playerRunning.start();
            _playerRunning.release();
        
        }
    
        public IEnumerator PlayIdleSoundLoop()
        {
            while (true)
            {
                PlayEnemyIdle(gameObject, EnemyType.BaseEnemy);
                yield return new WaitForSeconds(3);
            }
        }
        public void PlayPlayerTakeDamage()
        {
            if (playerTakeDamage.IsNull)
            {
                Debug.LogWarning("Fmod event not found: playertakedamage");
                return;
            }

            _playerTakeDamage = RuntimeManager.CreateInstance(playerTakeDamage);
        
            _playerTakeDamage.start();
            _playerTakeDamage.release();
        }

        public void PlayPlayerDeath()
        {
            if (playerTakeDamage.IsNull)
            {
                Debug.LogWarning("Fmod event not found: playertakedamage");
                return;
            }
            RuntimeManager.PlayOneShot(playerDeath, transform.position);

        }
        public void PlayEnemyFootStep(GameObject enemyObject, EnemyType enemyType)
        {
            if (enemySoundsMap[enemyType].footstep.IsNull)
            {
                Debug.LogWarning("Fmod event not found: enemyFootstep");
                return;
            }
            RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].footstep, enemyObject.transform.position);
        }
        public void PlayEnemyIdle(GameObject enemyObject, EnemyType enemyType)
        {
            if (enemySoundsMap[enemyType].idle.IsNull)
            {
                Debug.LogWarning("Fmod event not found: enemy idle");
                return;
            }
            RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].idle, enemyObject.transform.position);
        }
        public void PlayEnemyAttack(GameObject enemyobject, EnemyType enemyType, AttackType attackType)
        {
            if (enemySoundsMap[enemyType].normalAttack.IsNull)
            {
                Debug.LogWarning("Fmod event not found: enemyattack");
                return;
            }
            if (enemySoundsMap[enemyType].heavyAttack.IsNull)
            {
                Debug.LogWarning("Fmod event not found: enemyattack");
                return;
            }
            switch (attackType)
            {
                case AttackType.NormalAttack:
                    RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].normalAttack, enemyobject.transform.position);
                    break;
                case AttackType.HeavyAttack:
                    RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].heavyAttack, enemyobject.transform.position);
                    break;
                default:
                    Debug.LogWarning($"Unsupported attack type: {attackType}");
                    return;
            }
        
            //  RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].attack, enemyobject.transform.position);
        }
    

        /*  public void PlayEnemyTakeDamage(GameObject enemyobject, EnemyType enemyType)
    {
        if (enemySoundsMap[enemyType].takeDamage.IsNull)
        {
            Debug.LogWarning("Fmod event not found: enemy take damage");
            return;
        }
        
       RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].takeDamage, enemyobject.transform.position);
    }*/
        public void PlayEnemyDeath(GameObject enemyobject,EnemyType enemyType)
        {
            if (enemySoundsMap[enemyType].death.IsNull)
            {
                Debug.LogWarning("Fmod event not found: enemy death");
                return;
            }
        
            RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].death, enemyobject.transform.position);
        }
    
        public void PlayDoor(GameObject doorObject)
        {
            if (metalDoor.IsNull)
            {
                Debug.LogWarning("Fmod event not found: doorOpen");
                return;
            }
        
            RuntimeManager.PlayOneShot(metalDoor, doorObject.transform.position);
        }
        public void PlayDoorClosed(GameObject doorObject)
        {
            if (metalDoorClosed.IsNull)
            {
                Debug.LogWarning("Fmod event not found: door closed");
                return;
            }
        
            RuntimeManager.PlayOneShot(metalDoorClosed, doorObject.transform.position);
        }
        public void PlayPowerCore(GameObject Powecore)
        {
            if (powerCore.IsNull)
            {
                Debug.LogWarning("Fmod event not found: powercore");
                return;
            }
        
            RuntimeManager.PlayOneShot(powerCore, Powecore.transform.position);
        }
    }
}
