using Microsoft.Xna.Framework;
using SoLoud;
using System;
using System.Threading.Tasks;

namespace Rubedo.Audio;

/// <summary>
/// Plays and transitions music. There should probably only be one of these.
/// </summary>
public class MusicPlayer
{
    private WavStream currentStream;

    public readonly AudioCore audioCore;
    public AudioInstance musicInstance;

    public MusicPlayer(AudioCore audioCore)
    {
        this.audioCore = audioCore;
    }

    private void CreateMusic(string path, float volume)
    {
        currentStream = Assets.LoadSoundStream(path);
        musicInstance = audioCore.CreateSound(currentStream, (int)DefaultMixers.Type.Music, volume);
        musicInstance.Protect();
        musicInstance.SetLoop(true);
        musicInstance.Play();
    }

    public void SetPause(bool pause)
    {
        if (musicInstance == null || musicInstance.IsClosed())
            return;

        musicInstance.SetPause(pause);
    }

    public void Fade(float targetVolume, float time)
    {
        if (musicInstance == null || musicInstance.IsClosed())
            return;

        audioCore._soLoudInstance.fadeVolume(musicInstance.handle, targetVolume, time);
    }

    /// <summary>
    /// Plays the given track and immediately kills any currently playing music.
    /// </summary>
    /// <param name="songPath">The path to the new music.</param>
    /// <param name="volume">The volume to play the music at.</param>
    /// <param name="playbackPosition">The playback position.</param>
    /// <returns></returns>
    public void PlayMusic(string songPath, float volume, float playbackPosition)
    {
        if (musicInstance != null && !musicInstance.IsClosed())
            musicInstance.Stop();

        CreateMusic(songPath, volume);
    }

    /// <summary>
    /// Swaps music by fading in/out the current track and the new track at the same time.
    /// </summary>
    /// <param name="songPath">The path to the new music.</param>
    /// <param name="volume">The volume to fade the new music to.</param>
    /// <param name="playbackPosition">The playback position.</param>
    /// <param name="fadeTime">How long the transition lasts.</param>
    public void SwapMusic(string songPath, float volume, float playbackPosition, float fadeTime)
    {
        Soloud soloud = audioCore._soLoudInstance;
        //fade out the old music.
        soloud.fadeVolume(musicInstance.handle, 0, fadeTime);
        soloud.scheduleStop(musicInstance.handle, fadeTime); 

        //fade in the new music.
        CreateMusic(songPath, 0);
        soloud.fadeVolume(musicInstance.handle, volume, fadeTime);
    }
    /// <summary>
    /// Swaps music by fading out the old music, then fading in the new music.
    /// </summary>
    /// <param name="songPath"></param>
    /// <param name="volume"></param>
    /// <param name="playbackPosition"></param>
    /// <param name="fadeTime"></param>
    public async Task FullSwapMusic(string songPath, float volume, float playbackPosition, float fadeOutTime, float fadeInTime)
    {
        Soloud soloud = audioCore._soLoudInstance;
        //fade out the old music.
        soloud.fadeVolume(musicInstance.handle, 0, fadeOutTime);
        soloud.scheduleStop(musicInstance.handle, fadeOutTime);

        await Task.Delay(Lib.Math.CeilToInt(fadeOutTime * 1000)); //TODO: Update this to an actual coroutine when those are implemented.

        //fade in the new music.
        CreateMusic(songPath, 0);
        soloud.fadeVolume(musicInstance.handle, volume, fadeInTime);
    }
}