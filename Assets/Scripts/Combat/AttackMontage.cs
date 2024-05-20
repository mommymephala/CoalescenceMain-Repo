using System;
using System.Collections;
using Audio;
using States;
using UnityEngine;
using Utils;

namespace Combat
{
    public class AttackMontage : MonoBehaviour
    {
        private Actor _actor;

        public AttackBase Attack;
        public AnimatorStateHandle Animation;
        public float AnimationBlendTime = 0.2f;
        public float Duration;
        public float MontageDelay; // Can be used later if needed
        public float AttackActivationDelay;

        // --------------------------------------------------------------------

        private void Awake()
        {
            _actor = GetComponentInParent<Actor>();
            
            if (!Attack)
            {
                Attack = GetComponent<AttackBase>();
                if (Attack)
                {
                    Debug.LogWarning("Attack wasn't assigned on the AttackMontage but was found on the object. Please assign the reference manually", gameObject);
                }
                else
                {
                    Debug.LogError("Attack reference not assigned on AttackMontage", gameObject);
                }
            }
        }

        // --------------------------------------------------------------------

        public void Play(Animator animator)
        {
            switch (_actor.type)
            {
                case Actor.ActorType.TarSpawn:
                    AudioManager.Instance.PlayEnemyAttack(AudioManager.EnemyType.TarSpawn, AudioManager.EnemyAttackType.NormalAttack, transform.position);
                    break;
                case Actor.ActorType.ExperimentalMan:
                    AudioManager.Instance.PlayEnemyAttack(AudioManager.EnemyType.ExperimentalMan, AudioManager.EnemyAttackType.NormalAttack, transform.position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            animator.CrossFadeInFixedTime(Animation.Hash, AnimationBlendTime);

            StartCoroutine(StartAttackDelayed(AttackActivationDelay));
        }

        // --------------------------------------------------------------------

        public void OnNotStarted()
        {
            Attack.OnAttackNotStarted();
        }

        // --------------------------------------------------------------------

        private IEnumerator StartAttackDelayed(float delay)
        {
            yield return Yielders.Time(delay);
            Attack.StartAttack();
        }
    }
}
