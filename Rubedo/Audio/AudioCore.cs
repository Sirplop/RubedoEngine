using SoLoud;
using System.Collections.Generic;

namespace Rubedo.Audio;

/// <summary>
/// Utility wrapper around SoLoud. Used to play and modify sounds.
/// </summary>
public class AudioCore
{
    public enum SoloudReturnCode
    {
        SO_NO_ERROR = 0,        // No error
        INVALID_PARAMETER = 1,  // Some parameter is invalid
        FILE_NOT_FOUND = 2,     // File not found
        FILE_LOAD_FAILED = 3,   // File found, but could not be loaded
        DLL_NOT_FOUND = 4,      // DLL not found, or wrong DLL
        OUT_OF_MEMORY = 5,      // Out of memory
        NOT_IMPLEMENTED = 6,    // Feature not implemented
        UNKNOWN_ERROR = 7       // Other error
    }

    internal Soloud _soLoudInstance;
    internal AudioMixer outputBus;

    public Dictionary<string, AudioMixer> audioGroups = new Dictionary<string, AudioMixer>();

    public AudioCore()
    {
        _soLoudInstance = new Soloud();
        _soLoudInstance.init(aBackend: Soloud.SDL2);

        outputBus = new AudioMixer("out", this, null);

        audioGroups.Add("effects", new AudioMixer("effects", this, outputBus));
    }

    ~AudioCore()
    {
        _soLoudInstance.deinit();
        _soLoudInstance = null;
    }


    public AudioInstance CreateSound(Wav sourceSound, float volume = 1f, float pitch = 1f, float pan = 0f)
    {
        uint handle = audioGroups["effects"].MixingBus.play(sourceSound, volume, pan);
        _soLoudInstance.setRelativePlaySpeed(handle, pitch);
        _soLoudInstance.setPause(handle, 1); //all sounds start paused, must be played by the user.

        RubedoEngine.Logger.Info($"Played sound, currently playing {audioGroups["effects"].MixingBus.getActiveVoiceCount()} sounds.");

        AudioInstance instance = new AudioInstance(handle, this, sourceSound);

        return instance;
    }
}