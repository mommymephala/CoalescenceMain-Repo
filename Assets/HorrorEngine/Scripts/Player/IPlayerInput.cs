using UnityEngine;

namespace HorrorEngine
{
    public interface IPlayerInput
    {
        Vector2 GetPrimaryAxis();
        bool IsRunHeld();
        bool IsJumpHeld();
        bool IsAimingHeld();
        bool IsAttackDown();
        bool IsAttackUp();
        bool IsInteractingDown();
        bool IsReloadDown();
        bool IsTurn180Down();
        bool IsChangeAimTargetDown();
        void Flush();
    }
}