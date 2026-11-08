using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Plugins.EasyTransitions.Scripts
{
    public class Transition : MonoBehaviour
    {
        public TransitionSettings transitionSettings;

        [SerializeField] private Transform transitionPanelIn;
        [SerializeField] private Transform transitionPanelOut;
        [SerializeField] private CanvasScaler transitionCanvas;

        private Material _instantiatedMaterial;
        private bool _hasTransitionTriggeredOnce;
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            if (!transitionSettings)
            {
                Debug.LogError("[Transition] TransitionSettings не назначены!", this);
                return;
            }

            if (transitionCanvas)
                transitionCanvas.referenceResolution = transitionSettings.refrenceResolution;

            transitionPanelIn.gameObject.SetActive(true);
            transitionPanelOut.gameObject.SetActive(false);

            PrepareMaterialInstance();

            SetupTransitionObject(transitionSettings.transitionIn, transitionPanelIn);

            SceneManager.sceneLoaded += OnSceneLoad;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;

            if (!_instantiatedMaterial) return;
            Destroy(_instantiatedMaterial);
            _instantiatedMaterial = null;
        }

        public void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (_hasTransitionTriggeredOnce) return;
            _hasTransitionTriggeredOnce = true;

            transitionPanelIn.gameObject.SetActive(false);
            transitionPanelOut.gameObject.SetActive(true);

            SetupTransitionObject(transitionSettings.transitionOut, transitionPanelOut);

            float destroyTime = transitionSettings.destroyTime;
            if (transitionSettings.autoAdjustTransitionTime && transitionSettings.transitionSpeed > 0f)
                destroyTime /= transitionSettings.transitionSpeed;

            Destroy(gameObject, destroyTime);
        }

        private void PrepareMaterialInstance()
        {
            var baseMat = transitionSettings.colorTintMode == ColorTintMode.Multiply
                ? transitionSettings.multiplyColorMaterial
                : transitionSettings.addColorMaterial;

            if (!baseMat || transitionSettings.isCutoutTransition) return;
            _instantiatedMaterial = new Material(baseMat);
            _instantiatedMaterial.SetColor(ColorProperty, transitionSettings.colorTint);
        }

        private void SetupTransitionObject(GameObject prefab, Transform parent)
        {
            if (!prefab) return;

            var instance = Instantiate(prefab, parent);

            var canvasGroup = instance.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = transitionSettings.blockRaycasts;

            if (!transitionSettings.isCutoutTransition && _instantiatedMaterial != null)
            {
                var images = instance.GetComponentsInChildren<Image>(true);
                foreach (var image in images)
                {
                    image.material = _instantiatedMaterial;
                }
            }

            var scale = instance.transform.localScale;
            float signX = transitionSettings.flipX ? -1f : 1f;
            float signY = transitionSettings.flipY ? -1f : 1f;
            instance.transform.localScale = new Vector3(Mathf.Abs(scale.x) * signX, Mathf.Abs(scale.y) * signY, scale.z);

            if (transitionSettings.transitionSpeed <= 0f) return;
            var animators = instance.GetComponentsInChildren<Animator>(true);
            foreach (var animator in animators)
            {
                animator.speed = transitionSettings.transitionSpeed;
            }
        }
    }
}