using System.Collections;
using Equipment;
using Player;
using Systems;
using UnityEngine;

namespace Weapon
{
    public class BatonController : MonoBehaviour
    {
        [SerializeField] private LayerMask player;
    
        [SerializeField] private Collider hitbox;
        [SerializeField] private GameObject chargedAttackEffect;

        private Animator _animator;
        private DepleteEquipment _depleteEquipment;
        private IPlayerInput _input;
    
        [SerializeField] private Transform weaponHolderTransform;
        [SerializeField] private float attackDelay;
        // [SerializeField] private float maxDistance = 2f;
    
        private bool _isAttacking = false; 
        private bool IsCharged => _depleteEquipment.HasCharge();
    
        // public EventReference batonAttack;
        // public EventReference batonSwoosh;
        // public EventReference batonElectric;
        // public EventInstance batonInstance;
    

        private void Awake()
        {
            _input = GetComponentInParent<IPlayerInput>();
            _animator = GetComponent<Animator>();
            _depleteEquipment = GetComponent<DepleteEquipment>();
            weaponHolderTransform = GameObject.Find("WeaponHolder").transform;
        }

        private void Start()
        {
            hitbox.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (GameManager.Instance.IsPlaying && _input.IsAttackDown() && !_isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
        }

        private IEnumerator AttackRoutine()
        {
            StartAttack();
            yield return new WaitForSeconds(GetCurrentAnimationLength() + attackDelay);
            FinishAttack();
        }

        private float GetCurrentAnimationLength()
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.length;
        }
   
        private void StartAttack()
        {
            _isAttacking = true;
            // _animator.SetTrigger("Attack");

            var isElectricActive = IsCharged;
            ToggleElectricEffect(isElectricActive);

            if (isElectricActive)
            {
                _depleteEquipment.Deplete();
                EnableChargedAttackEffects();
                hitbox.GetComponent<BatonHitbox>().SetChargedAttack(true);
            }
            else
            {
                hitbox.GetComponent<BatonHitbox>().SetChargedAttack(false);
            }

            // Perform the raycast to detect the surface
            /*if (Physics.Raycast(weaponHolderTransform.position, weaponHolderTransform.forward, out RaycastHit hitInfo, maxDistance, player))
            {
                // Check if the hit object's tag is one of the expected tags.
                /*if (hitInfo.transform.CompareTag("Wall"))
                {
                    PlaySurfaceHitSFX("Base");
                }
                else if (hitInfo.transform.CompareTag("Tar"))
                {
                    PlaySurfaceHitSFX("Tar_Spawn");
                }
                else if (hitInfo.transform.CompareTag("Chip"))
                {
                    PlaySurfaceHitSFX("Chip");
                }
                // ... add other surfaces as needed ...
                else
                {
                    // The object hit doesn't have a specific tag, play the "Empty" sound.
                    // PlaySurfaceHitSFX("Empty");
                }
                Debug.Log(hitInfo.transform.tag);
            }
            else
            {
                // Ray didn't hit any collider.
                // PlaySurfaceHitSFX("Empty");
                Debug.Log("Raycast didn't hit any collider.");
            }*/
        
            EnableHitbox();
        }

        public void EnableHitbox()
        {
            hitbox.gameObject.SetActive(true);
        }

        public void DisableHitbox()
        {
            hitbox.gameObject.SetActive(false);
        }

        private void FinishAttack()
        {
            _isAttacking = false;
        }

        private void EnableChargedAttackEffects()
        {
            if (chargedAttackEffect == null) return;
            GameObject effectInstance = Instantiate(chargedAttackEffect, hitbox.transform.position, Quaternion.identity);
            Destroy(effectInstance, 1f);
        }

        private void ToggleElectricEffect(bool isElectricActive)
        {
            if (isElectricActive)
            {
                EnableChargedAttackEffects();
            }
        }
    
        /*private void PlaySurfaceHitSFX(string surfaceType)
    {
        // Ensure we have an instance of the baton sound event.
        //if (!batonInstance.isValid())
        //{
            batonInstance = RuntimeManager.CreateInstance(batonAttack);
        //}

        // Set the parameter for the surface type.
        switch (surfaceType)
        {
            case "Tar_Spawn":
                batonInstance.setParameterByName("Baton_Hit", 1);
                break;
            case "Chip":
                batonInstance.setParameterByName("Baton_Hit", 2);
                break;
            case "Glass":
                batonInstance.setParameterByName("Baton_Hit", 3);
                break;
            default:
                batonInstance.setParameterByName("Baton_Hit", 0); // Default or 'Empty' surface
                break;
        }
    
        // Check if the electric state is active and set the parameter.
        // if (IsCharged) // Assuming IsCharged is true when electric state is active
        // {
        //     batonInstance.setParameterByName("ElectricStage", 2); // Set parameter for electric state
        // }
        // else
        // {
        //     batonInstance.setParameterByName("ElectricStage", 1); // Set parameter for non-electric state
        // }

        // Start the sound instance.
        batonInstance.start();
    }
        
        /*batonInstance = RuntimeManager.CreateInstance(batonAttack);
        switch (surfaceHitSfx)
        {
            case "Base":
                batonInstance.setParameterByName("HitSurfaces", 3);
                Debug.Log("duvarım");
                
                break;
            case "Electric":
                batonInstance.setParameterByName("HitSurfaces", 1);
                Debug.Log("tarım");
                break;

            // Add other cases as needed
        }#1#
        
        private void OnDestroy()
        {
            if (batonInstance.isValid())
            {
                batonInstance.release();
            }
        }*/
        
    }
}
