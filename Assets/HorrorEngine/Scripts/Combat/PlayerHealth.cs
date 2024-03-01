using Audio;
using Interfaces;

namespace HorrorEngine
{
    public class PlayerHealth : Health, IDamageable
    {
        public void TakeDamage(float damage, bool isChargedAttack, bool isWeakpoint)
        {
            AudioManager.Instance.PlayPlayerTakeDamage();
            DamageReceived(damage);
        } 
        public void ResetPlayerHealth()
        {
            // Reset health to max and reset any other player-specific states
           
                Destroy(gameObject); // This will destroy the player object
            
            // Add any additional reset logic specific to PlayerHealth here
        }
    }
}