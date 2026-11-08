using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Plugins.EasyTransitions.Scripts
{
    public class TransitionManager : MonoBehaviour
    {
        [SerializeField] private Transition transitionTemplate;

        private bool _runningTransition;

        private readonly UnityAction _onTransitionBegin;
        private readonly UnityAction _onTransitionCutPointReached;
        private readonly UnityAction _onTransitionEnd;

        public TransitionManager(UnityAction onTransitionBegin, UnityAction onTransitionCutPointReached, UnityAction onTransitionEnd)
        {
            _onTransitionBegin = onTransitionBegin;
            _onTransitionCutPointReached = onTransitionCutPointReached;
            _onTransitionEnd = onTransitionEnd;
        }

        private static TransitionManager _instance;

        private void Awake()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static TransitionManager Instance()
        {
            if (_instance) return _instance;
            Debug.LogError("[TransitionManager] Экземпляр отсутствует на сцене! Убедитесь, что префаб добавлен.");
            return null;
        }

        public void Transition(string sceneName, TransitionSettings transition, float startDelay = 0f)
        {
            StartTransitionInternal(transition, startDelay, () => SceneManager.LoadSceneAsync(sceneName));
        }

        public void Transition(int sceneIndex, TransitionSettings transition, float startDelay = 0f)
        {
            StartTransitionInternal(transition, startDelay, () => SceneManager.LoadSceneAsync(sceneIndex));
        }

        private void StartTransitionInternal(TransitionSettings transition, float startDelay, Func<AsyncOperation> loadSceneAction)
        {
            if (!transition)
            {
                Debug.LogError("[TransitionManager] TransitionSettings не могут быть null!");
                return;
            }

            if (!transitionTemplate)
            {
                Debug.LogError("[TransitionManager] transitionTemplate не назначен в инспекторе!");
                return;
            }

            if (_runningTransition)
            {
                Debug.LogWarning("[TransitionManager] Переход уже выполняется. Запрос проигнорирован.");
                return;
            }

            StartCoroutine(TransitionRoutine(transition, startDelay, loadSceneAction));
        }

        private IEnumerator TransitionRoutine(TransitionSettings settings, float delay, Func<AsyncOperation> loadSceneAction)
        {
            _runningTransition = true;

            try
            {
                if (delay > 0f)
                    yield return new WaitForSecondsRealtime(delay);

                _onTransitionBegin?.Invoke();

                var transitionInstance = Instantiate(transitionTemplate);
                transitionInstance.transitionSettings = settings;

                float transitionTime = settings.transitionTime;
                if (settings.autoAdjustTransitionTime && settings.transitionSpeed > 0f)
                    transitionTime /= settings.transitionSpeed;

                yield return new WaitForSecondsRealtime(transitionTime);

                _onTransitionCutPointReached?.Invoke();

                if (loadSceneAction != null)
                {
                    var asyncLoad = loadSceneAction.Invoke();
                    while (!asyncLoad.isDone)
                        yield return null;
                }
                else
                {
                    transitionInstance.OnSceneLoad(SceneManager.GetActiveScene(), LoadSceneMode.Single);
                }

                float destroyTime = settings.destroyTime;
                if (settings.autoAdjustTransitionTime && settings.transitionSpeed > 0f)
                    destroyTime /= settings.transitionSpeed;

                yield return new WaitForSecondsRealtime(destroyTime);

                _onTransitionEnd?.Invoke();
            }
            finally
            {
                _runningTransition = false;
            }
        }
    }
}