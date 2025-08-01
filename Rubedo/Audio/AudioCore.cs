﻿using SoLoud;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rubedo.Audio;

/// <summary>
/// Utility wrapper around SoLoud. Used to play and modify sounds.
/// </summary>
public class AudioCore
{
    internal Soloud _soLoudInstance;
    internal AudioMixer outputBus;

    internal readonly List<AudioMixer> audioMixers = new List<AudioMixer>();
    public ReadOnlyCollection<AudioMixer> AudioMixers => audioMixers.AsReadOnly();

    internal AudioCore(bool createDefaultMixers = true)
    {
        _soLoudInstance = new Soloud();
        _soLoudInstance.init(aBackend: Soloud.SDL2);

        outputBus = new AudioMixer("out", this, null);

        if (createDefaultMixers)
            DefaultMixers.CreateDefaultMixers(this);
    }

    ~AudioCore()
    {
        _soLoudInstance.deinit();
        _soLoudInstance = null;
    }

    /// <summary>
    /// Creates a new audio mixer group and puts it in the list of audio mixers.
    /// </summary>
    /// <param name="name">The name of the new mixer.</param>
    /// <param name="outputTarget">The mixer this mixer will output to. If null, defaults to <see cref="outputBus"/>.</param>
    /// <returns>The ID of the new audio mixer.</returns>
    public int CreateNewMixer(string name, AudioMixer outputTarget)
    {
        audioMixers.Add(new AudioMixer(name, this, outputTarget == null ? outputBus : outputTarget));
        return audioMixers.Count - 1;
    }

    public AudioInstance CreateSound(Wav sourceSound, int audioType, float volume = 1f, float pitch = 1f, float pan = 0f)
    {
        if (audioType < 0 ||  audioType > audioMixers.Count)
            throw new System.ArgumentOutOfRangeException(nameof(audioType));

        uint handle = audioMixers[audioType].MixingBus.play(sourceSound, volume, pan);
        _soLoudInstance.setRelativePlaySpeed(handle, pitch);
        _soLoudInstance.setPause(handle, 1); //all sounds start paused, must be played by the user.

        AudioInstance instance = new AudioInstance(handle, this, sourceSound);

        return instance;
    }
    public AudioInstance CreateSound(WavStream sourceSound, int audioType, float volume = 1f, float pitch = 1f, float pan = 0f)
    {
        if (audioType < 0 || audioType > audioMixers.Count)
            throw new System.ArgumentOutOfRangeException(nameof(audioType));

        uint handle = audioMixers[audioType].MixingBus.play(sourceSound, volume, pan);
        _soLoudInstance.setRelativePlaySpeed(handle, pitch);
        _soLoudInstance.setPause(handle, 1); //all sounds start paused, must be played by the user.

        AudioInstance instance = new AudioInstance(handle, this, sourceSound);

        return instance;
    }

    public void StopAll()
    {
        for (int i = 0; i < audioMixers.Count; i++)
        {
            audioMixers[i].StopAllSounds();
            Log.Debug("Playing mixers: " + outputBus.MixingBus.getActiveVoiceCount());
        }
    }
}