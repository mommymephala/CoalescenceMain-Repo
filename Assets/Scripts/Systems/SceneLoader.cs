using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(SceneReference scene)
        {
            SceneManager.LoadScene(scene.Name);
        }
    }
}