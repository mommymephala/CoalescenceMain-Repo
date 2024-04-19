using System.Collections;
using HEScripts.States;
using HEScripts.Utils;
using UnityEngine;

namespace HEScripts.Combat
{
    public class AttackMontage : MonoBehaviour
    {
        public AttackBase Attack;
        public AnimatorStateHandle Animation;
        public float AnimationBlendTime = 0.2f;
        public float Duration;
        public float MontageDelay; // Can be used later if needed
        public float AttackActivationDelay;

        [SerializeField] public AudioSource m_AudioSource;
        [SerializeField] private AudioClip m_AttackSound;

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
            if (m_AttackSound && m_AudioSource)
                m_AudioSource.PlayOneShot(m_AttackSound);

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
