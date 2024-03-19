using HEScripts.States;
using HEScripts.Systems;
using HorrorEngine;

namespace HEScripts.Player
{
    public class PlayerActor : Actor
    {
        void Start()
        {
            if (GameManager.Exists)
                GameManager.Instance.RegisterPlayer(this);
        }

        public void SetVisible(bool visible)
        {
            MainAnimator.gameObject.SetActive(visible);
        }
    }

}