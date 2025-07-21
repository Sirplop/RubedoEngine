using SoLoud;
using System;

namespace Rubedo.Audio;

/// <summary>
/// A group of different sound handles.
/// </summary>
public class AudioMixer : IDisposable
{
    private readonly uint playHandle;

    public Bus MixingBus { get; private set; }
    public readonly AudioCore audioCore;

    public string Name { get; private set; }
    public bool IsDisposed { get; private set; } = false;

    public AudioMixer(string name, AudioCore core, AudioMixer? outputTarget)
    {
        Name = name;
        audioCore = core;
        MixingBus = new Bus();
        if (outputTarget != null)
            playHandle = outputTarget.MixingBus.play(MixingBus, 1f);
        else
            playHandle = core._soLoudInstance.play(MixingBus, 1f);

        MixingBus.setVisualizationEnable(1);
    }

    public void PausePlayback()
    {
        if (IsDisposed)
            return;
        audioCore._soLoudInstance.setPause(playHandle, 1);
    }

    public void UnpausePlayback()
    {
        if (IsDisposed)
            return;
        audioCore._soLoudInstance.setPause(playHandle, 0);
    }

    public void SetVolume(float volume)
    {
        if (IsDisposed)
            return;
        audioCore._soLoudInstance.setVolume(playHandle, volume);
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        audioCore._soLoudInstance.stop(playHandle);
        MixingBus = null;
        GC.SuppressFinalize(this);
    }
}