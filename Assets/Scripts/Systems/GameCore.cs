using Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Systems
{
    public class GameCore : MonoBehaviour
    {
        [SerializeField] private CorePrefabs CorePrefabs;
        [SerializeField] private SceneReference[] DestroyInScenes;

        private void Awake()
        {
            Debug.Assert(transform.parent == null, "GameCore has to be placed at the top level of the hierarchy (without a parent)");

            var cores = FindObjectsOfType<GameCore>();
            if (cores.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += OnSceneChange;
        }

        private void Start()
        {
            if (CorePrefabs)
            {
                InitCorePrefabs();
            }
            else
            {
                Debug.LogWarning("CorePrefabs has not been assigned.");
            }
        }

        private void InitCorePrefabs()
        {
            var mapped = CorePrefabs.GetMappedPrefabs();
            foreach (var mapEntry in mapped)
            {
                var path = mapEntry.Key;
                var objects = mapEntry.Value;

                Transform parent = string.IsNullOrEmpty(path) ? transform : transform.Find(path);
                if (!parent)
                {
                    var parentGo = new GameObject(path);
                    parent = parentGo.transform;
                    parent.SetParent(transform);
                }

                foreach (GameObject go in objects)
                {
                    if (!go)
                        continue;

                    Instantiate(go, parent);
                }
            }
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            EventSystem oldEventSystem = EventSystem.current;
            GameObject selected = oldEventSystem?.currentSelectedGameObject;

            bool shouldDestroy = false;
            foreach (SceneReference scene in DestroyInScenes)
            {
                if (newScene.name == scene.Name)
                {
                    shouldDestroy = true;
                    break;
                }
            }

            if (shouldDestroy)
            {
                AudioManager.Instance.StopAmbientSound();
                SceneManager.activeSceneChanged -= OnSceneChange;
                Destroy(gameObject);

                // Fix for EventSystem issues
                EventSystem newEventSystem = EventSystem.current;
                if (oldEventSystem != newEventSystem && newEventSystem)
                {
                    newEventSystem.gameObject.SetActive(false);
                    newEventSystem.gameObject.SetActive(true);

                    if (selected)
                        newEventSystem.SetSelectedGameObject(selected);
                }
            }
        }
    }
}