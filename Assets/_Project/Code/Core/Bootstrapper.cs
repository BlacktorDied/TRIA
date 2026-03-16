using UnityEngine;

namespace TRIA.Core
{
    public static class Bootstrapper
    {
        // This runs instantly when the game starts, before any scene loads
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute()
        {
            // Create a new empty GameObject to hold our core systems
            GameObject coreSystems = new GameObject("[CORE_SYSTEMS]");
            Object.DontDestroyOnLoad(coreSystems);

            // Load and spawn the Master Persistent UI
            GameObject uiPrefab = Resources.Load<GameObject>("PersistentUI");
            if (uiPrefab != null)
            {
                GameObject uiInstance = Object.Instantiate(uiPrefab);
                uiInstance.name = "[PERSISTENT_UI]";
                Object.DontDestroyOnLoad(uiInstance);
            }

            // Add the components to it
            coreSystems.AddComponent<GameManager>();
            coreSystems.AddComponent<SceneLoader>();
            coreSystems.AddComponent<PlayerManager>();

            MusicManager music = coreSystems.AddComponent<MusicManager>();

            // We need two AudioSources for crossfading
            // We use reflection/logic to assign them or just create them
            var sA = coreSystems.AddComponent<AudioSource>();
            var sB = coreSystems.AddComponent<AudioSource>();

            // Use private reflection or make a 'Setup' method in MusicManager to link these

            // In the future, we can also add our SaveManager, etc. here

            Debug.Log("TRIA Bootstrapper: Core systems successfully initialized.");
        }
    }
}
