using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inlumino_SHARED
{
    static class SoundManager
    {
        static Dictionary<SoundEffect, SFXData> looping = new Dictionary<SoundEffect, SFXData>();
        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <param name="cat"> Set to null to bypass auto volume settings.</param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <param name="pan"></param>
        internal static void PlaySound(SoundEffect s, SoundCategory cat, bool loop = false, float volume = 1, float pitch = 0, float pan = 0)
        {
            if (cat == SoundCategory.Music)
                volume = Manager.GameSettings.MusicVolume;
            if (cat == SoundCategory.SFX)
                volume = Manager.GameSettings.SFXVolume;
            s.Play(volume, pitch, pan);
            if (loop)
                looping.Add(s, new SFXData(volume, pitch, pan));
        }
        internal static void StopLoop(SoundEffect s)
        {
            if (looping.ContainsKey(s))
                looping.Remove(s);
        }
        internal static void Update(GameTime time)
        {
            foreach (KeyValuePair<SoundEffect, SFXData> p in looping)
            {
                SFXData data = looping[p.Key];
                data.Elapsed += (float)time.ElapsedGameTime.TotalMilliseconds;
                if (data.Elapsed >= p.Key.Duration.TotalMilliseconds)
                { p.Key.Play(data.Volume, data.Pitch, data.Pan); data.Elapsed = 0; }
                looping[p.Key].CopyFrom(data);
            }
        }
    }
    class SFXData
    {
        internal float Elapsed, Volume, Pitch, Pan;
        internal SFXData(float volume = 1, float pitch = 0, float pan = 0, float elapsed = 0)
        {
            Elapsed = elapsed;
            Volume = volume;
            Pitch = pitch;
            Pan = pan;
        }

        internal void CopyFrom(SFXData data)
        {
            Elapsed = data.Elapsed;
            Volume = data.Volume;
            Pitch = data.Pitch;
            Pan = data.Pan;
        }
    }
    enum SoundCategory { Music, SFX }
}
