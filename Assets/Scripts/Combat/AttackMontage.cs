using System.Collections;
using Audio;
using States;
using UnityEngine;
using Utils;

namespace Combat
{
    public class AttackMontage : MonoBehaviour
    {
        public AttackBase Attack;
        public AnimatorStateHandle Animation;
        public float AnimationBlendTime = 0.2f;
        public float Duration;
        public float MontageDelay; // Can be used later if needed
        public float AttackActivationDelay;

        // --------------------------------------------------------------------

        private void Awake()
        {
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
            //TODO: Test this
            AudioManager.Instance.PlayEnemyAttack(gameObject, AudioManager.EnemyType.TarSpawn, AudioManager.EnemyAttackType.NormalAttack);
            // Debug.Log("Audio played");

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
