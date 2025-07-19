using SoLoud;

namespace Rubedo.Audio;

/// <summary>
/// A group of different sound handles.
/// </summary>
public class AudioMixer
{
    private readonly AudioMixer outputTarget;
    private uint playHandle;

    public Bus MixingBus { get; private set; }
    public readonly AudioCore audioCore;

    public string Name { get; private set; }

    private bool flip = false;

    public AudioMixer(string name, AudioCore core, AudioMixer outputTarget)
    {
        Name = name;
        audioCore = core;
        MixingBus = new Bus();
        this.outputTarget = outputTarget;
        if (outputTarget != null)
            playHandle = outputTarget.MixingBus.play(MixingBus, 1f);
        else
            playHandle = core._soLoudInstance.play(MixingBus, 1f);

        MixingBus.setVisualizationEnable(1);
    }

    public void PausePlayback()
    {
        audioCore._soLoudInstance.setPause(playHandle, 1);
    }

    public void UnpausePlayback()
    {
        audioCore._soLoudInstance.setPause(playHandle, 0);
    }

    public void FlipVolume()
    {
        if (flip)
        {
            audioCore._soLoudInstance.setVolume(playHandle, 1f);
        }
        else
        {
            audioCore._soLoudInstance.setVolume(playHandle, 0.5f);
        }
        float ch1 = MixingBus.getApproximateVolume(0);
        float ch2 = MixingBus.getApproximateVolume(1);
        RubedoEngine.Logger.Info($"{ch1}, {ch2}");
        RubedoEngine.Logger.Info($"Tried changing volume, currently playing {MixingBus.getActiveVoiceCount()} sounds.");

        flip = !flip;
    }
}