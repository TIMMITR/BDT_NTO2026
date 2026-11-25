using Configs;
using UnityEngine;

namespace Scenes
{
    public class GlobalContext : MonoBehaviour
    {
        public WorldSettingsSo worldSettings;
        public PrefabSettingsSo prefabSettings;
        public TransitionsSettingsSo transitionSettings;
        public CardsConfigSo cardsConfig;
        public AudioConfigSo audioConfig;
        public AudioContext audioContext;
    }
}