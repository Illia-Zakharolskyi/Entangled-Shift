using UnityEngine;
using UnityEngine.SceneManagement;

namespace EntangledShift.Runtime.Systems
{
    public class BootstrapLoader : MonoBehaviour
    {
        [SerializeField] private string _sceneToLoad;

        void Start()
        {
            if (_sceneToLoad != null)
            {
                SceneManager.LoadScene(_sceneToLoad);
            }
        }
    }
}
