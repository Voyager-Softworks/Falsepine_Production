using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  This class is used to manage the music in the game.
/// </summary>
public class AudioController : MonoBehaviour ///< @todo comment
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
        public float lastVolume;
        [Range(0, 1)]
        public float pitch;
        public bool layered = false;
        public AudioClip clip;
        public AudioClip[] layers;
        public int timeSignature = 4;
        public int beatsPerMinute = 80;
        public float BarDuration { get { return 60f * timeSignature / beatsPerMinute; } }
        public float BeatDuration { get { return BarDuration / timeSignature; } }
        public int CurrentBar { get { return (int)(time / BarDuration); } }
        public int layerIndex = 0;
        public int currentLayer = 0;
        public float longestLayerDuration = 0;
        public int longestLayerIndex = 0;
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
        public AudioSource[] layerSources;
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

        public void Play()
        {
            if (layered)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    if (layers[i] != null)
                    {
                        layerSources[i].Play();
                    }
                }
            }
            else
            {
                source.Play();
            }
            playing = true;
            paused = false;
        }

        public void Pause()
        {
            if (layered)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    if (layers[i] != null)
                    {
                        layerSources[i].Pause();
                    }
                }
            }
            else
            {
                source.Pause();
            }
            playing = false;
            paused = true;
        }

        public void Stop()
        {
            if (layered)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    if (layers[i] != null)
                    {
                        layerSources[i].Stop();
                    }
                }
            }
            else
            {
                source.Stop();
            }
            playing = false;
            paused = false;
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
            // if clip is null, skip this channel
            if (channel.clip == null)
            {
                Debug.LogWarning("Audio Channel " + channel.name + " has no clip assigned!", this);
                continue;
            }
            if (!channel.layered)
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
            }
            else
            {
                channel.layerSources = new AudioSource[channel.layers.Length];
                GameObject layerParent = Instantiate<GameObject>(new GameObject(channel.name), transform);
                float longestLayerDuration = 0;
                for (int i = 0; i < channel.layers.Length; i++)
                {
                    channel.layerSources[i] = Instantiate<GameObject>(new GameObject(channel.name + " Layer " + i), layerParent.transform).AddComponent<AudioSource>();
                    channel.layerSources[i].clip = channel.layers[i];
                    channel.layerSources[i].volume = (i == 0) ? channel.volume : 0f;
                    channel.layerSources[i].pitch = channel.pitch;
                    channel.layerSources[i].loop = channel.loop;
                    channel.layerSources[i].spatialBlend = channel.SpatialBlend;
                    channel.layerSources[i].maxDistance = channel.maxDistance;
                    channel.layerSources[i].minDistance = channel.minDistance;
                    channel.layerSources[i].rolloffMode = AudioRolloffMode.Custom;
                    channel.layerSources[i].SetCustomCurve(AudioSourceCurveType.CustomRolloff, channel.distanceCurve);
                    if (channel.playOnAwake) channel.layerSources[i].Play();
                    if (channel.layers[i].length > longestLayerDuration) longestLayerDuration = channel.layers[i].length;
                }
                channel.longestLayerDuration = longestLayerDuration;
            }

            channel.playing = channel.playOnAwake;
            channel.paused = !channel.playing;
            channel.time = 0;
            channel.timeNormalized = 0;
            channel.duration = !channel.layered ? channel.source.clip.length : channel.longestLayerDuration;
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
        audioChannels[index].Play();
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
                channel.Play();
                channel.playing = true;
                channel.paused = false;
                return;
            }
        }
    }

    /// <summary>
    ///  Plays the channel with the given name once.
    /// </summary>
    /// <param name="name"></param>
    public void PlayOnce(string name)
    {
        foreach (AudioChannel channel in audioChannels)
        {

            if (channel.name == name)
            {
                if (channel.layered)
                {
                    Debug.LogWarning("Cannot play channel " + name + " once, as it is layered.", this);
                    return;
                }
                channel.source.PlayOneShot(channel.clip);
                channel.playing = true;
                channel.paused = false;
                return;
            }
        }
    }

    /// <summary>
    ///  Plays the channel with the given index once.
    /// </summary>
    /// <param name="index"></param>
    public void PlayOnce(int index)
    {
        if (index < 0 || index >= audioChannels.Count) return;
        if (audioChannels[index].layered)
        {
            Debug.LogWarning("Cannot play channel " + audioChannels[index].name + " once, as it is layered.", this);
            return;
        }
        audioChannels[index].source.PlayOneShot(audioChannels[index].clip);
        audioChannels[index].playing = true;
        audioChannels[index].paused = false;
    }

    /// <summary>
    ///  Pauses the channel with the given index.
    /// </summary>
    /// <param name="index"></param>
    public void Pause(int index)
    {
        if (index < 0 || index >= audioChannels.Count) return;
        audioChannels[index].Pause();
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
                channel.Pause();
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
        audioChannels[index].Stop();
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
                channel.Stop();
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
        float startVolume = audioChannels[index].lastVolume;
        audioChannels[index].volume = 0;
        audioChannels[index].Play();
        audioChannels[index].playing = true;
        audioChannels[index].paused = false;
        while (t < time)
        {
            t += Time.deltaTime;
            audioChannels[index].volume = Mathf.Lerp(0, startVolume, t / time);
            yield return null;
        }
        audioChannels[index].volume = startVolume;
    }
    IEnumerator FadeOutCoroutine(int index, float time, bool reset = false)
    {
        if (index < 0 || index >= audioChannels.Count) yield break;
        float t = 0;
        audioChannels[index].playing = false;
        audioChannels[index].paused = false;
        audioChannels[index].lastVolume = audioChannels[index].volume;
        float startVolume = audioChannels[index].volume;
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
            // if clip is null, skip this channel
            if (channel.clip == null)
            {
                Debug.LogWarning("Audio Channel " + channel.name + " has no clip assigned!", this);
                continue;
            }

            if (channel.layered)
            {
                channel.time = channel.layerSources[channel.longestLayerIndex].time;
            }
            else
            {
                channel.time = channel.source.time;
            }
            channel.timeNormalized = channel.time / channel.duration;

            //Update source variables to match channel variables
            if (channel.layered)
            {
                foreach (AudioSource source in channel.layerSources)
                {
                    source.pitch = channel.pitch;
                    source.loop = channel.loop;
                    source.spatialBlend = channel.SpatialBlend;
                    source.maxDistance = channel.maxDistance;
                    source.minDistance = channel.minDistance;
                    source.rolloffMode = AudioRolloffMode.Custom;
                    source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, channel.distanceCurve);
                }
            }
            else
            {
                channel.source.volume = channel.volume;
                channel.source.pitch = channel.pitch;
                channel.source.loop = channel.loop;
                channel.source.spatialBlend = channel.SpatialBlend;
                channel.source.maxDistance = channel.maxDistance;
                channel.source.minDistance = channel.minDistance;
                channel.source.rolloffMode = AudioRolloffMode.Custom;
                channel.source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, channel.distanceCurve);
            }

            if (channel.layered)
            {
                if (channel.currentLayer != channel.layerIndex)
                {
                    // Check if the channel is at the end of a bar
                    if (channel.time % channel.BarDuration <= 0.05f)
                    {
                        channel.currentLayer = channel.layerIndex;
                    }
                }
                for (int i = 0; i < channel.layers.Length; i++)
                {
                    if (i <= channel.currentLayer)
                    {
                        channel.layerSources[i].volume = Mathf.Lerp(channel.layerSources[i].volume, channel.volume * (1 - ((channel.currentLayer) * 0.05f)), Time.deltaTime * 4f);
                    }
                    else
                    {
                        if (channel.layerSources[i].volume < 0.02f)
                        {
                            channel.layerSources[i].volume = 0;
                        }
                        else
                        {
                            channel.layerSources[i].volume = Mathf.Lerp(channel.layerSources[i].volume, 0, Time.deltaTime * 4f);
                        }
                    }
                }
            }
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
                    float time = from.time;
                    float duration = from.duration;
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
