using UnityEngine;

namespace HEScripts.Player
{
    public interface IPlayerInput
    {
        Vector2 GetPrimaryAxis();
        bool IsRunHeld();
        bool IsJumpHeld();
        bool IsAimingHeld();
        bool IsAttackDown();
        bool IsAttackHeld();
        bool IsAttackUp();
        bool IsInteractingDown();
        bool IsReloadDown();
        bool IsOpenSystemMenuDown();
        bool IsOpenInventoryMenuDown();
        void Flush();
    }
}