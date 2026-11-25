using System.Collections.Generic;
using Configs;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.AudioPack.Systems
{
    public sealed class AudioSystem : ISystem
    {
        public World World { get; set; }

        private Request<PlayEffectRequest> _request;
        private readonly List<AudioSource> _audioSources;
        private readonly AudioConfigSo _audioConfig;
        private readonly WorldSettingsSo _worldSettings;

        public AudioSystem(
            List<AudioSource> audioSources,
            AudioConfigSo audioConfig,
            WorldSettingsSo worldSettings)
        {
            _audioSources = audioSources;
            _audioConfig = audioConfig;
            _worldSettings = worldSettings;
        }

        public void OnAwake()
        {
            _request = World.GetRequest<PlayEffectRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _request.Consume())
            {
                var audioFiles = _audioConfig.GetAudioFiles(request.audioType);
                if (audioFiles.Count == 0)
                {
                    Debug.LogWarning("No audio files found");
                    continue;
                }

                var audioSource = GetFreeAudioSource();
                if (audioSource == null)
                {
                    Debug.LogWarning("No audio source found");
                    continue;
                }

                var audioFile = audioFiles[Random.Range(0, audioFiles.Count)];
                if (audioFile == null || audioFile.clip == null)
                {
                    Debug.LogWarning("No audio file found");
                    continue;
                }

                audioSource.clip = audioFile.clip;
                audioSource.volume = audioFile.volume;
                audioSource.pitch = audioFile.pitch + Random.Range(
                    -_worldSettings.audioPitchVariation,
                    _worldSettings.audioPitchVariation);
                audioSource.Play();
            }
        }

        private AudioSource GetFreeAudioSource()
        {
            foreach (var audioSource in _audioSources)
            {
                if (audioSource != null && !audioSource.isPlaying)
                {
                    return audioSource;
                }
            }

            return null;
        }

        public void Dispose() { }
    }
}