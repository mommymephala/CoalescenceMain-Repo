using HEScripts.States;
using HEScripts.Systems;

namespace HEScripts.Player
{
    public class PlayerActor : Actor
    {
        private void Start()
        {
            if (GameManager.Exists)
                GameManager.Instance.RegisterPlayer(this);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }

}