namespace Rubedo.Audio;

/// <summary>
/// Creates a set of 3 default mixers for the audio core: Music, Effect, Menu. Also contains an enum for them.
/// </summary>
public static class DefaultMixers
{
    public enum Type
    {
        Music = 0,
        Effect = 1,
        Menu = 2
    }
    public static void CreateDefaultMixers(AudioCore core)
    {
        core.audioMixers.Add(new AudioMixer("music", core, core.outputBus));
        core.audioMixers.Add(new AudioMixer("effect", core, core.outputBus));
        core.audioMixers.Add(new AudioMixer("menu", core, core.outputBus));
    }
}
