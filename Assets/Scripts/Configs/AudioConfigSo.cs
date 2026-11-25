using System;
using System.Collections.Generic;
using System.Linq;
using Features.AudioPack.Data;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "AudioConfigSo", menuName = "Configs/Audio Config", order = 10)]
    public class AudioConfigSo : ScriptableObject
    {
        public List<AudioEntry> audioEntries = new();

        public IReadOnlyList<AudioFile> GetAudioFiles(AudioType audioType)
        {
            foreach (var entry in audioEntries.Where(entry => entry.audioType == audioType))
            {
                return entry.audioFiles;
            }

            return Array.Empty<AudioFile>();
        }

        [Serializable]
        public sealed class AudioEntry
        {
            public AudioType audioType;
            public List<AudioFile> audioFiles = new();
        }
    }

    public enum AudioType
    {
        UnitMove, // ++
        Explosion, // ++
        EndCasting, // звук выстрела
        MeleeAttack, // ++
        MeleeHit, // ++
        UnitsSpawned, // +
        UnitsDestroyed,
        UnitsHealing,
        UnitsHit,
        PointCaptured, // +-
        CardPicked, // +-
        NotEnoughMana, // +-
        VireteVoice,
        WarlockVoice,
        Ambient
    }
}