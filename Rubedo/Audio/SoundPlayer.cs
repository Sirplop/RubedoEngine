using Rubedo.Components;
using Rubedo.Lib;
using SoLoud;

namespace Rubedo.Audio;

/// <summary>
/// A component that can play a configured sound.
/// </summary>
public class SoundPlayer : Component
{
    private readonly Wav sound;
    private readonly AudioInstance[] _instances;
    public readonly AudioCore audioCore;

    /// <summary>
    /// The mixing group this sound will play through.
    /// </summary>
    public int audioType;
    /// <summary>
    /// The volume of this sound.
    /// </summary>
    public float volume = 1f;
    /// <summary>
    /// The pitch offset of this sound.
    /// </summary>
    public float pitch = 1f;
    /// <summary>
    /// The +- range of this sound. Only used if <see cref="randomizePitch"/> is true.
    /// </summary>
    public float pitchRange = 0f;
    /// <summary>
    /// Whether the pitch is randomized by the <see cref="pitchRange"/> or not.
    /// </summary>
    public bool randomizePitch = false;
    /// <summary>
    /// Whether this sound should loop or not.
    /// </summary>
    public bool loop = false;
    /// <summary>
    /// Whether sounds this player is playing should keep playing after this has been destroyed.
    /// Will be ignored if sounds are looping, because otherwise sound references are lost.
    /// </summary>
    public bool persistAfterDestroy = false;

    public SoundPlayer(string soundPath, int audioType, int maxInstances, AudioCore audio)
    {
        sound = Assets.LoadSoundEffect(soundPath);
        _instances = new AudioInstance[maxInstances];
        audioCore = audio;
        this.audioType = audioType;
    }

    /// <summary>
    /// Sets the playback volume and pitch of this sound. This does not randomize the pitch.
    /// </summary>
    /// <param name="volume">The volume of playback</param>
    /// <param name="pitch">The pitch of playback</param>
    /// <returns></returns>
    public SoundPlayer SetInfo(float volume, float pitch)
    {
        this.volume = volume;
        this.pitch = pitch;
        this.pitchRange = 0;
        this.randomizePitch = false;
        return this;
    }

    /// <summary>
    /// Sets the playback volume and pitch of this sound. 
    /// The pitch is chosen randomly between <paramref name="pitchMin"/> and <paramref name="pitchMax"/> for each sound instance.
    /// </summary>
    /// <param name="volume">The volume of playback</param>
    /// <param name="pitchMin">The minimum pitch of playback</param>
    /// <param name="pitchMax">The maximum pitch of playback</param>
    /// <returns></returns>
    public SoundPlayer SetRandomInfo(float volume, float pitchMin, float pitchMax)
    {
        this.volume = volume;
        this.randomizePitch = true;
        float halfRange = System.MathF.Abs(pitchMax - pitchMin) * 0.5f;
        this.pitch = pitchMin + halfRange;
        this.pitchRange = halfRange;

        return this;
    }

    /// <summary>
    /// Attempts to play this sound.
    /// </summary>
    /// <param name="volume">The volume to play at.</param>
    /// <param name="pitch">The pitch to play at. Effects playback speed.</param>
    /// <returns>The <see cref="AudioInstance"/> of the playing sound, or null if no sound could be played.</returns>
    public AudioInstance Play()
    {
        for (int i = 0; i < _instances.Length; i++)
        {
            if (_instances[i] == null || _instances[i].IsClosed())
            {
                float pitch = this.pitch;
                if (randomizePitch)
                    pitch += Random.Range(-pitchRange, pitchRange);
                if (pitch < 0)
                    pitch = 0;
                AudioInstance ret = audioCore.CreateSound(sound, audioType, volume, pitch);
                if (loop)
                    ret.SetLoop(true);
                ret.Play();
                _instances[i] = ret;
                return ret;
            }
        }
        return null;
    }

    public void Stop(int index)
    {
        if (index > _instances.Length)
            return; //nothing to stop.

        AudioInstance instance = _instances[index];
        if (instance != null && !instance.IsClosed())
            instance.Stop();
    }

    public void StopAll()
    {
        for (int i = 0; i < _instances.Length; i++)
        {
            AudioInstance instance = _instances[i];
            if (instance != null && !instance.IsClosed())
                instance.Stop();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (!persistAfterDestroy || loop)
        {
            for (int i = 0; i < _instances.Length; i++)
            {
                AudioInstance instance = _instances[i];
                if (instance != null && !instance.IsClosed())
                    instance.Stop();
            }
        }
    }
}