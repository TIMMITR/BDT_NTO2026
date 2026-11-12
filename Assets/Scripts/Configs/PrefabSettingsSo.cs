using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "PrefabSettings", menuName = "Configs/New Prefab Settings", order = 1)]
    public class PrefabSettingsSo : ScriptableObject
    {
        [SerializeField] private List<PrefabEntry> prefabs = new();

        public GameObject Get(PrefabType type) => 
            prefabs.Where(entry => entry.type == type)
                .Select(entry => entry.prefab)
                .FirstOrDefault();
    }
    
    [Serializable]
    public struct PrefabEntry
    {
        public PrefabType type;
        public GameObject prefab;
    }
    
    public enum PrefabType
    {
        Player,
        Camera,
    }
    
}