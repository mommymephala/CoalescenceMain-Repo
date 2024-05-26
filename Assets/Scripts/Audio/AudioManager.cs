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
        [Serializable]
        public struct EnemySounds
        {
            public EventReference idle;
            public EventReference alert;
            public EventReference footstep;
            public EventReference normalAttack;
            public EventReference heavyAttack;
            public EventReference enemyHurt;
            public EventReference weakpoint;
            public EventReference death;
        }

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

        public enum ElevatorType
        {
            MainElevator,
            PowerCoreElevator
        }

        public Dictionary<Actor.ActorType, EnemyType> actorToEnemyTypeMap;

        [Header("Player")]
        public EventReference playerHurt;
        public EventReference playerDeath;

        [Header("Player Footsteps")]
        [HideInInspector] public float footstepTimer;
        public EventReference playerFootsteps;
        public EventReference playerRunning;
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
        public EventReference mainElevatorSound;
        public EventReference powerCoreElevatorSound;
        public Dictionary<ElevatorType, EventReference> elevatorSoundsMap;
        public EventReference dysonActivation;

        private Dictionary<EventInstance, Coroutine> _fadeCoroutines = new Dictionary<EventInstance, Coroutine>();
        private float _crossfadeDuration;

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

            elevatorSoundsMap = new Dictionary<ElevatorType, EventReference>
            {
                { ElevatorType.MainElevator, mainElevatorSound },
                { ElevatorType.PowerCoreElevator, powerCoreElevatorSound }
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
            return default;
        }

        private static void PlayOneShot(EventReference eventRef, Vector3 position, string eventName)
        {
            if (!eventRef.IsNull)
            {
                RuntimeManager.PlayOneShot(eventRef, position);
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

        public void PlayFootstep(Vector3 position)
        {
            PlayOneShot(playerFootsteps, position, "Player footstep");
        }

        public void PlayRunning(Vector3 position)
        {
            PlayOneShot(playerRunning, position, "Player running");
        }

        public void PlayPlayerHurt(Vector3 position)
        {
            PlayOneShot(playerHurt, position, "Player hurt");
        }

        public void PlayPlayerDeath(Vector3 position)
        {
            PlayOneShot(playerDeath, position, "Player death");
        }

        public void PlayDoorOpen(Vector3 position)
        {
            PlayOneShot(door, position, "Door open");
        }

        public void PlayDoorClosed(Vector3 position)
        {
            PlayOneShot(doorClosed, position, "Door closed");
        }

        public void PlayElevatorActivation(ElevatorType elevatorType, Vector3 position)
        {
            if (elevatorSoundsMap.TryGetValue(elevatorType, out EventReference elevatorActivationSound))
            {
                PlayOneShot(elevatorActivationSound, position, $"Elevator activation: {elevatorType}");
            }
            else
            {
                Debug.LogWarning($"Elevator type not found: {elevatorType}");
            }
        }

        public void PlayDysonActivation(Vector3 position)
        {
            PlayOneShot(dysonActivation, position, "Dyson activation");
        }

        public void PlayEnemyFootstep(EnemyType enemyType, Vector3 position)
        {
            PlayOneShot(enemySoundsMap[enemyType].footstep, position, "Enemy footstep");
        }

        public void PlayEnemyAttack(EnemyType enemyType, EnemyAttackType attackType, Vector3 position)
        {
            EventReference attackSound = attackType == EnemyAttackType.NormalAttack ?
                enemySoundsMap[enemyType].normalAttack : enemySoundsMap[enemyType].heavyAttack;

            PlayOneShot(attackSound, position, "Enemy attack");
        }

        public void PlayEnemyHurt(EnemyType enemyType, Vector3 position)
        {
            PlayOneShot(enemySoundsMap[enemyType].enemyHurt, position, "Enemy hurt");
        }
        
        public void PlayEnemyWeakpoint(string weakpointTag, Vector3 position)
        {
            EnemyType enemyType;
    
            switch (weakpointTag)
            {
                case "TarSpawnWeakpoint":
                    enemyType = EnemyType.TarSpawn;
                    break;
                case "ExperimentalManWeakpoint":
                    enemyType = EnemyType.ExperimentalMan;
                    break;
                default:
                    Debug.LogWarning($"Unknown weakpoint tag: {weakpointTag}");
                    return;
            }

            if (enemySoundsMap.TryGetValue(enemyType, out EnemySounds sounds))
            {
                PlayOneShot(sounds.weakpoint, position, "Enemy weakpoint");
            }
            else
            {
                Debug.LogError($"No sounds defined for enemy type: {enemyType}");
            }
        }

        public void PlayEnemyDeath(EnemyType enemyType, Vector3 position)
        {
            PlayOneShot(enemySoundsMap[enemyType].death, position, "Enemy death");
        }

        public void PlayEnemyIdle(EnemyType enemyType, Vector3 position)
        {
            PlayOneShot(enemySoundsMap[enemyType].idle, position, "Enemy idle");
        }

        public void PlayEnemyAlert(EnemyType enemyType, Vector3 position)
        {
            PlayOneShot(enemySoundsMap[enemyType].alert, position, "Enemy alert");
        }
    }
}