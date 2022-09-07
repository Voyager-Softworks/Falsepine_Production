using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  This class is used to manage the music in the game.
/// </summary>
public class AudioController : MonoBehaviour
{
    /// <summary>
    ///  The audio channels that are used to play the music.
    /// </summary>
    [System.Serializable]
    public class AudioChannel
    {
        public string name = "New Audio Channel";
        [Range(0, 1)]
        public float volume;
        [Range(0, 1)]
        public float pitch;
        public AudioClip clip;
        public bool loop;
        public bool playOnAwake;
        [Range(0, 1)]
        public float SpatialBlend;
        [Range(0, 300)]
        public float maxDistance;
        [Range(0, 300)]
        public float minDistance;
        public AnimationCurve distanceCurve = AnimationCurve.Linear(0, 1, 1, 0);
        [Header("Runtime Data")]
        [ReadOnly]
        public AudioSource source;
        [ReadOnly]
        public bool playing;
        [ReadOnly]
        public bool paused;
        [ReadOnly]
        public float time;
        [ReadOnly]
        public float timeNormalized;
        [ReadOnly]
        public float duration;

        public AudioChannel()
        {
            volume = 1;
            pitch = 1;
            SpatialBlend = 1;
            maxDistance = 300;
            minDistance = 0;
            distanceCurve = AnimationCurve.Linear(0, 1, 1, 0);
            name = "New Audio Channel";
        }
    }

    public string[] triggers = { }; ///< The triggers that can be used to trigger transitions.

                                    /// <summary>
                                    ///  Transitions between audio channels.
                                    /// </summary>
    [System.Serializable]
    public class AudioChannelTransition
    {
        public string from, to;
        public float duration;
        public bool hasExitTime;
        public float exitTime;
        public bool inheritNormalisedTime;
        public string trigger;
        public bool autoTransitionOnTrackEnd;
    }

    public List<AudioChannel> audioChannels; ///< The current audio channels.
    public List<AudioChannelTransition> transitions; ///< The current transitions.



    // Start is called before the first frame update
    void Start()
    {
        foreach (AudioChannel channel in audioChannels)
        {
            channel.source = Instantiate<GameObject>(new GameObject(channel.name), transform).AddComponent<AudioSource>();
            channel.source.clip = channel.clip;
            channel.source.volume = channel.volume;
            channel.source.pitch = channel.pitch;
            channel.source.loop = channel.loop;
            channel.source.spatialBlend = channel.SpatialBlend;
            channel.source.maxDistance = channel.maxDistance;
            channel.source.minDistance = channel.minDistance;
            channel.source.rolloffMode = AudioRolloffMode.Custom;
            channel.source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, channel.distanceCurve);
            if (channel.playOnAwake) channel.source.Play();
            channel.playing = channel.playOnAwake;
            channel.paused = !channel.playing;
            channel.time = 0;
            channel.timeNormalized = 0;
            channel.duration = channel.source.clip.length;
        }
    }

    /// <summary>
    ///  Gets the audiosource of the channel with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public AudioSource GetChannelSource(string name)
    {
        foreach (AudioChannel channel in audioChannels)
        {
            if (channel.name == name) return channel.source;
        }
        return null;
    }

    /// <summary>
    ///  Plays the channel with the given index.
    /// </summary>
    /// <param name="index"></param>
    public void Play(int index)
    {
        if (index < 0 || index >= audioChannels.Count) return;
        audioChannels[index].source.Play();
        audioChannels[index].playing = true;
        audioChannels[index].paused = false;
    }

    /// <summary>
    ///  Plays the channel with the given name.
    /// </summary>
    /// <param name="name"></param>
    public void Play(string name)
    {
        foreach (AudioChannel channel in audioChannels)
        {
            if (channel.name == name)
            {
                channel.source.Play();
                channel.playing = true;
                channel.paused = false;
                return;
            }
        }
    }

    /// <summary>
    ///  Pauses the channel with the given index.
    /// </summary>
    /// <param name="index"></param>
    public void Pause(int index)
    {
        if (index < 0 || index >= audioChannels.Count) return;
        audioChannels[index].source.Pause();
        audioChannels[index].playing = false;
        audioChannels[index].paused = true;
    }

    /// <summary>
    ///  Pauses the channel with the given name.
    /// </summary>
    /// <param name="name"></param>
    public void Pause(string name)
    {
        foreach (AudioChannel channel in audioChannels)
        {
            if (channel.name == name)
            {
                channel.source.Pause();
                channel.playing = false;
                channel.paused = true;
                return;
            }
        }
    }

    /// <summary>
    ///  Stops the channel with the given index.
    /// </summary>
    /// <param name="index"></param>
    public void Stop(int index)
    {
        if (index < 0 || index >= audioChannels.Count) return;
        audioChannels[index].source.Stop();
        audioChannels[index].playing = false;
        audioChannels[index].paused = false;
    }

    /// <summary>
    ///  Stops the channel with the given name.
    /// </summary>
    /// <param name="name"></param>
    public void Stop(string name)
    {
        foreach (AudioChannel channel in audioChannels)
        {
            if (channel.name == name)
            {
                channel.source.Stop();
                channel.playing = false;
                channel.paused = false;
                return;
            }
        }
    }

    /// <summary>
    ///  Fades in the channel with the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="time"></param>
    /// <param name="normalizedTime"></param>
    public void FadeIn(int index, float time, float normalizedTime = 0.0f)
    {
        if (index < 0 || index >= audioChannels.Count) return;
        StartCoroutine(FadeInCoroutine(index, time, normalizedTime));
    }

    /// <summary>
    ///  Fades out the channel with the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="time"></param>
    public void FadeOut(int index, float time)
    {
        if (index < 0 || index >= audioChannels.Count) return;
        StartCoroutine(FadeOutCoroutine(index, time));
    }

    IEnumerator FadeInCoroutine(int index, float time, float normalizedTime = 0.0f)
    {
        if (index < 0 || index >= audioChannels.Count) yield break;
        float t = 0;
        float startVolume = audioChannels[index].source.volume;
        audioChannels[index].source.volume = 0;
        audioChannels[index].source.Play();
        audioChannels[index].playing = true;
        audioChannels[index].paused = false;
        while (t < time)
        {
            t += Time.deltaTime;
            audioChannels[index].source.volume = Mathf.Lerp(0, startVolume, t / time);
            yield return null;
        }
        audioChannels[index].source.volume = startVolume;
    }
    IEnumerator FadeOutCoroutine(int index, float time, bool reset = false)
    {
        if (index < 0 || index >= audioChannels.Count) yield break;
        float t = 0;
        audioChannels[index].playing = false;
        audioChannels[index].paused = false;
        float startVolume = audioChannels[index].source.volume;
        while (t < time)
        {
            t += Time.deltaTime;
            audioChannels[index].source.volume = Mathf.Lerp(startVolume, 0, t / time);
            yield return null;
        }
        audioChannels[index].source.volume = 0;
        if (reset) audioChannels[index].source.Stop();
        else audioChannels[index].source.Pause();

    }

    public int GetChannelIndex(string name)
    {
        for (int i = 0; i < audioChannels.Count; i++)
        {
            if (audioChannels[i].name == name) return i;
        }
        return -1;
    }

    public void Trigger(string name)
    {
        foreach (AudioChannelTransition transition in transitions)
        {
            if (transition.trigger == name)
            {

                if (transition.inheritNormalisedTime)
                {
                    float time = GetChannelSource(transition.from).time;
                    float duration = GetChannelSource(transition.from).clip.length;
                    float normalizedTime = time / duration;
                    FadeOut(GetChannelIndex(transition.from), transition.duration);
                    FadeIn(GetChannelIndex(transition.to), transition.duration, normalizedTime);
                }
                else
                {
                    FadeOut(GetChannelIndex(transition.from), transition.duration);
                    FadeIn(GetChannelIndex(transition.to), transition.duration);
                }


            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        foreach (AudioChannel channel in audioChannels)
        {
            channel.time = channel.source.time;
            channel.timeNormalized = channel.time / channel.duration;
            channel.playing = channel.source.isPlaying;
            channel.paused = !channel.source.isPlaying;

            //Update source variables to match channel variables
            channel.source.volume = channel.volume;
            channel.source.pitch = channel.pitch;
            channel.source.loop = channel.loop;
            channel.source.spatialBlend = channel.SpatialBlend;
            channel.source.maxDistance = channel.maxDistance;
            channel.source.minDistance = channel.minDistance;
            channel.source.rolloffMode = AudioRolloffMode.Custom;
            channel.source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, channel.distanceCurve);

        }

        foreach (AudioChannelTransition transition in transitions)
        {
            if (!transition.autoTransitionOnTrackEnd) continue;
            AudioChannel from = audioChannels[GetChannelIndex(transition.from)];
            AudioChannel to = audioChannels[GetChannelIndex(transition.to)];

            if (from.timeNormalized >= 0.99 && from.playing)
            {
                if (transition.inheritNormalisedTime)
                {
                    float time = GetChannelSource(transition.from).time;
                    float duration = GetChannelSource(transition.from).clip.length;
                    float normalizedTime = time / duration;
                    FadeOut(GetChannelIndex(transition.from), transition.duration);
                    FadeIn(GetChannelIndex(transition.to), transition.duration, normalizedTime);
                }
                else
                {
                    FadeOut(GetChannelIndex(transition.from), transition.duration);
                    FadeIn(GetChannelIndex(transition.to), transition.duration);
                }
            }
        }
    }
}
