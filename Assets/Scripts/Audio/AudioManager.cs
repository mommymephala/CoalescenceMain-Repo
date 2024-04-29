using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Singleton;
using UnityEngine;

namespace Audio
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        public enum EnemyAttackType
        {
            NormalAttack,
            HeavyAttack
        }
        
        public enum EnemyType
        {
            TarSpawn,
            ExperimentalMan
        }
        
        [Serializable]
        public struct EnemySounds
        {
            public EventReference idle;
            public EventReference footstep;
            public EventReference normalAttack;
            public EventReference heavyAttack;
            public EventReference enemyHurt;
            public EventReference death;
        }
        
        [Header("Player")]
        public EventReference playerRunning;
        public EventReference playerHurt;
        public EventReference playerDeath;
        
        [Header("Player Footsteps")]
        public EventReference playerFootsteps;
        public float footstepTimer;
        public float footstepDelay = 0.5f;
        public float runningFootstepDelay = 0.25f;
        
        [Header("Enemy Sounds")]
        public EnemySounds tarSpawnSounds;
        public EnemySounds experimentalManSounds;
        public Dictionary<EnemyType, EnemySounds> enemySoundsMap;
        
        [Header("Environment")]
        public EventReference ambient;
        public EventReference safeRoom;
        public EventReference metalDoor;
        public EventReference metalDoorClosed;
        public EventReference dysonActivation;
        
        // EventInstances
        private EventInstance _playerFootstepInstance;
        private EventInstance _playerTakeDamage;
        private EventInstance _playerRunning;
        private EventInstance _baseEnemyFootstepInstance;
        private EventInstance _ambient;
        private EventInstance _safeRoom;
        
        // Cross-fade related fields
        public Dictionary<EventInstance, Coroutine> fadeCoroutines = new Dictionary<EventInstance, Coroutine>();
        [SerializeField] private float crossfadeDuration = 1.0f;
        private Coroutine _idleCoroutine;
        
        // Flags
        private bool _isInsideSafeRoom;

        protected override void Awake()
        {
            base.Awake();
            
            enemySoundsMap = new Dictionary<EnemyType, EnemySounds>
            {
                { EnemyType.TarSpawn, tarSpawnSounds },
                { EnemyType.ExperimentalMan, experimentalManSounds }
            };
        }

        private void Start()
        {
            _ambient = RuntimeManager.CreateInstance(ambient);
            _ambient.start();
        }

        public void Crossfade(EventInstance fromInstance, EventInstance toInstance, float duration)
        {
            // Stop existing fade on the 'from' instance
            if (fadeCoroutines.TryGetValue(fromInstance, out Coroutine existingCoroutine))
            {
                StopCoroutine(existingCoroutine);
                fadeCoroutines.Remove(fromInstance);
            }

            // Start new cross-fade
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
                var t = currentTime / duration;

                // Adjust volumes for cross-fade
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
        private static void PauseEventInstance(EventInstance instance)
        {
            instance.setPaused(true);
        }

        // Method to resume an event instance
        private static void ResumeEventInstance(EventInstance instance)
        {
            instance.setPaused(false);
        }

        public void EnterSafeRoom()
        {
            if (!_isInsideSafeRoom)
            {
                FadeOutSound(_ambient, crossfadeDuration, true);
                _safeRoom = RuntimeManager.CreateInstance(safeRoom);
                _safeRoom.setVolume(0);
                _safeRoom.start();
                FadeInSound(_safeRoom, crossfadeDuration);
                _isInsideSafeRoom = true;
            }
        }

        public void ExitSafeRoom()
        {
            if (_isInsideSafeRoom)
            {
                FadeOutSound(_safeRoom, crossfadeDuration);
                ResumeEventInstance(_ambient);
                FadeInSound(_ambient, crossfadeDuration);
                _isInsideSafeRoom = false;
            }
        }

        private static IEnumerator StartFade(EventInstance sound, float targetVolume, float duration, bool pauseOnFadeOut = false)
        {
            float currentTime = 0;
            sound.getVolume(out var startVolume);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                var newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
                sound.setVolume(newVolume);
                yield return null;
            }
            
            sound.setVolume(targetVolume);

            if (targetVolume == 0 && pauseOnFadeOut)
            {
                PauseEventInstance(sound);
            }
        }

        private void FadeInSound(EventInstance sound, float duration)
        {
            StartCoroutine(StartFade(sound, 1.0f, duration));
        }

        private void FadeOutSound(EventInstance sound, float duration, bool pauseOnFadeOut = false)
        {
            StartCoroutine(StartFade(sound, 0.0f, duration, pauseOnFadeOut));
        }
    
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
                Debug.LogWarning("Fmod event not found: playerRunning");
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
                PlayEnemyIdle(gameObject, EnemyType.TarSpawn);
                yield return new WaitForSeconds(3);
            }
        }
        
        public void PlayPlayerTakeDamage()
        {
            if (playerHurt.IsNull)
            {
                Debug.LogWarning("Fmod event not found: playerHurt");
                return;
            }

            _playerTakeDamage = RuntimeManager.CreateInstance(playerHurt);
        
            _playerTakeDamage.start();
            _playerTakeDamage.release();
        }

        public void PlayPlayerDeath()
        {
            if (playerHurt.IsNull)
            {
                Debug.LogWarning("Fmod event not found: playerDeath");
                return;
            }
            RuntimeManager.PlayOneShot(playerDeath, transform.position);

        }
        
        public void PlayEnemyFootstep(GameObject enemyObject, EnemyType enemyType)
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
                Debug.LogWarning("Fmod event not found: enemySoundsMap[enemyType].idle");
                return;
            }
            
            RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].idle, enemyObject.transform.position);
        }
        
        public void PlayEnemyAttack(GameObject enemyobject, EnemyType enemyType, EnemyAttackType enemyAttackType)
        {
            if (enemySoundsMap[enemyType].normalAttack.IsNull)
            {
                Debug.LogWarning("Fmod event not found: enemySoundsMap[enemyType].normalAttack");
                return;
            }
            
            if (enemySoundsMap[enemyType].heavyAttack.IsNull)
            {
                Debug.LogWarning("Fmod event not found: enemySoundsMap[enemyType].heavyAttack");
                return;
            }
            
            switch (enemyAttackType)
            {
                case EnemyAttackType.NormalAttack:
                    RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].normalAttack, enemyobject.transform.position);
                    break;
                case EnemyAttackType.HeavyAttack:
                    RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].heavyAttack, enemyobject.transform.position);
                    break;
                default:
                    Debug.LogWarning($"Unsupported attack type: {enemyAttackType}");
                    return;
            }
        }
        
        public void PlayEnemyHurt(GameObject enemyobject, EnemyType enemyType)
        {
            if (enemySoundsMap[enemyType].enemyHurt.IsNull)
            {
                Debug.LogWarning("Fmod event not found: enemySoundsMap[enemyType].enemyHurt");
                return;
            }
                
            RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].enemyHurt, enemyobject.transform.position);
        }
        
        public void PlayEnemyDeath(GameObject enemyobject, EnemyType enemyType)
        {
            if (enemySoundsMap[enemyType].death.IsNull)
            {
                Debug.LogWarning("Fmod event not found: enemySoundsMap[enemyType].death");
                return;
            }
        
            RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].death, enemyobject.transform.position);
        }
    
        public void PlayDoor(GameObject doorObject)
        {
            if (metalDoor.IsNull)
            {
                Debug.LogWarning("Fmod event not found: metalDoor");
                return;
            }
        
            RuntimeManager.PlayOneShot(metalDoor, doorObject.transform.position);
        }
        
        public void PlayDoorClosed(GameObject doorObject)
        {
            if (metalDoorClosed.IsNull)
            {
                Debug.LogWarning("Fmod event not found: metalDoorClosed");
                return;
            }
        
            RuntimeManager.PlayOneShot(metalDoorClosed, doorObject.transform.position);
        }
        
        public void PlayDysonActivation(GameObject dysonObject)
        {
            if (dysonActivation.IsNull)
            {
                Debug.LogWarning("Fmod event not found: dysonActivation");
                return;
            }
        
            RuntimeManager.PlayOneShot(dysonActivation, dysonObject.transform.position);
        }
    }
}