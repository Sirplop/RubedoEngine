using SoLoud;

namespace Rubedo.Audio;

/// <summary>
/// Handle for an instance of a sound playing through Soloud.
/// </summary>
public class AudioInstance
{
    internal uint handle;
    private AudioCore _coreRef;
    private bool played = false;
    private readonly Wav baseSoundRef; //this is here solely so that we don't accidentally unload the reference while playing this sound.

    public AudioInstance(uint handle, AudioCore core, Wav baseSoundRef)
    {
        this.handle = handle;
        this._coreRef = core;
        this.baseSoundRef = baseSoundRef;
    }

    /// <summary>
    /// Unpauses the sound and finalizes some stuff.
    /// </summary>
    /// <returns>True if the sound played, false if it did not.</returns>
    public bool Play()
    {
        if (played)
            return false;
        played = true;
        if (IsClosed())
            return false;
        _coreRef._soLoudInstance.setPause(handle, 0);
        return true;
    }

    /// <summary>
    /// Checks if the handle of this instance is no longer valid, whether that be from the sound being stopped, or the sound concluding.
    /// </summary>
    /// <returns></returns>
    public bool IsClosed()
    {
        return _coreRef._soLoudInstance.isValidVoiceHandle(handle) == 0;
    }

    /// <summary>
    /// Sets the looping behavior of a live sound. The loop point is, by default, the start of the sound.
    /// </summary>
    public AudioInstance SetLoop(bool doLoop, double loopPoint = 0)
    {
        _coreRef._soLoudInstance.setLooping(handle, doLoop ? 1 : 0);
        _coreRef._soLoudInstance.setLoopPoint(handle, loopPoint);
        return this;
    }

    /// <summary>
    /// Set the inaudible behavior of a live sound. By default, if a sound is inaudible, it’s paused, and
    /// will resume when it becomes audible again. With this function you can tell SoLoud to either kill
    /// the sound if it becomes inaudible, or to keep ticking the sound even if it’s inaudible.
    /// </summary>
    public AudioInstance SetInaudibleBehavior(bool kill, bool continuePlaying)
    {
        _coreRef._soLoudInstance.setInaudibleBehavior(handle, continuePlaying ? 1 : 0, kill ? 1 : 0);
        return this;
    }
    /// <summary>
    /// <para>Normally, if you try to play more sounds than there are voices, SoLoud will kill off the oldest
    /// playing sound to make room. This will most likely be your background music. This can be worked
    /// around by protecting the sound.</para>
    /// <para>If all voices are protected, the result will be undefined.</para>
    /// </summary>
    public AudioInstance Protect()
    {
        _coreRef._soLoudInstance.setProtectVoice(handle, 1);
        return this;
    }

    public AudioInstance SetPause(bool pause)
    {
        if (!played)
            throw new System.NotSupportedException("Cannot un/pause a sound that has yet to play! Use Play() first.");
        int paused = _coreRef._soLoudInstance.getPause(handle);
        if (paused != (pause ? 1 : 0))
            _coreRef._soLoudInstance.setPause(handle, pause ? 1 : 0);
        return this;
    }

    public bool Stop()
    {
        if (!played)
            return false;
        _coreRef._soLoudInstance.stop(handle);
        return true;
    }
}