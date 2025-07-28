using SoLoud;
using System;

namespace Rubedo.Audio;

/// <summary>
/// A group of different sound handles.
/// </summary>
public class AudioMixer : IDisposable
{
    internal uint playHandle;
    private AudioMixer outputTarget;

    public Bus MixingBus { get; private set; }
    public readonly AudioCore audioCore;

    public string Name { get; private set; }
    public bool IsDisposed { get; private set; } = false;

    public AudioMixer(string name, AudioCore core, AudioMixer? outputTarget)
    {
        Name = name;
        audioCore = core;
        MixingBus = new Bus();
        this.outputTarget = outputTarget;
        if (outputTarget != null)
            playHandle = outputTarget.MixingBus.play(MixingBus, 1f);
        else
            playHandle = core._soLoudInstance.play(MixingBus, 1f);
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

    internal bool StopAllSounds()
    {
        if (outputTarget == null) //top level players do nothing.
            return true;

        if (this.IsDisposed || outputTarget.IsDisposed)
            return false; //this is trying to output to something that doesn't exist!

        MixingBus.stopAllNonBusSounds();

        return true;
    }
}