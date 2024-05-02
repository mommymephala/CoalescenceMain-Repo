using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Singleton;
using States;
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

        public Dictionary<Actor.ActorType, EnemyType> actorToEnemyTypeMap;

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
        public EventReference playerHurt; 
        public EventReference playerDeath;

        [Header("Player Footsteps")] 
        public EventReference playerFootsteps;
        public EventReference playerRunning;
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
        public EventReference door;
        public EventReference doorClosed;
        public EventReference elevatorActivation;
        public EventReference dysonActivation;

        // Private fields for FMOD instances and internal state
        private Dictionary<EventInstance, Coroutine> _fadeCoroutines = new Dictionary<EventInstance, Coroutine>();
        private float _crossfadeDuration = 1.0f;

        private bool _isInsideSafeRoom;


        protected override void Awake()
        {
            base.Awake();
            InitializeSoundsMap();
            InitializeTypeMappings();
        }

        private void Start()
        {
            StartAmbientSound();
        }
        
        private void InitializeSoundsMap()
        {
            enemySoundsMap = new Dictionary<EnemyType, EnemySounds>
            {
                { EnemyType.TarSpawn, tarSpawnSounds },
                { EnemyType.ExperimentalMan, experimentalManSounds }
            };
        }
        
        private void InitializeTypeMappings()
        {
            actorToEnemyTypeMap = new Dictionary<Actor.ActorType, EnemyType>
            {
                { Actor.ActorType.TarSpawn, EnemyType.TarSpawn },
                { Actor.ActorType.ExperimentalMan, EnemyType.ExperimentalMan }
            };
        }

        public EnemyType GetEnemyTypeFromActorType(Actor.ActorType actorType)
        {
            if (actorToEnemyTypeMap.TryGetValue(actorType, out EnemyType enemyType))
            {
                return enemyType;
            }
            
            Debug.LogWarning("No AudioManager.EnemyType mapped for Actor.ActorType: " + actorType);
            return default;  // Or throw an exception or return a default value as necessary
        }
        
        // --------------------------------------------------------------------
        
        private void PlayOneShot(EventReference eventRef, string eventName)
        {
            if (!eventRef.IsNull)
            {
                RuntimeManager.PlayOneShot(eventRef, transform.position);
            }
            else
            {
                Debug.LogWarning($"FMOD event not found: {eventName}");
            }
        }

        private void StartAmbientSound()
        {
            EventInstance ambientInstance = RuntimeManager.CreateInstance(ambient);
            ambientInstance.start();
            _fadeCoroutines.Add(ambientInstance, null);
        }

        // --------------------------------------------------------------------

        public void PlayFootstep()
        {
            PlayOneShot(playerFootsteps, "Player footstep");
        }

        public void PlayRunning()
        {
            PlayOneShot(playerRunning, "Player running");
        }

        public void PlayPlayerHurt()
        {
            PlayOneShot(playerHurt, "Player hurt");
        }

        public void PlayPlayerDeath()
        {
            PlayOneShot(playerDeath, "Player death");
        }

        public void PlayDoorOpen()
        {
            PlayOneShot(door, "Door open");
        }

        public void PlayDoorClosed()
        {
            PlayOneShot(doorClosed, "Door closed");
        }
        
        public void PlayElevatorActivation()
        {
            PlayOneShot(elevatorActivation, "Elevator activation");
        }

        public void PlayDysonActivation()
        {
            PlayOneShot(dysonActivation, "Dyson activation");
        }

        public void PlayEnemyFootstep(EnemyType enemyType)
        {
            PlayOneShot(enemySoundsMap[enemyType].footstep, "Enemy footstep");
        }

        public void PlayEnemyAttack(EnemyType enemyType, EnemyAttackType attackType)
        {
            EventReference attackSound = attackType == EnemyAttackType.NormalAttack ? 
                enemySoundsMap[enemyType].normalAttack : enemySoundsMap[enemyType].heavyAttack;

            PlayOneShot(attackSound, "Enemy attack");
        }

        public void PlayEnemyHurt(EnemyType enemyType)
        {
            PlayOneShot(enemySoundsMap[enemyType].enemyHurt, "Enemy hurt");
        }

        public void PlayEnemyDeath(EnemyType enemyType)
        {
            PlayOneShot(enemySoundsMap[enemyType].death, "Enemy death");
        }

        public void PlayEnemyIdle(EnemyType enemyType)
        {
            PlayOneShot(enemySoundsMap[enemyType].idle, "Enemy idle");
        }
    }
}