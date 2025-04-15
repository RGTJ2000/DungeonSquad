using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public enum SoundCategory
{
    sfx,
    music,
    UI,
    ambient,
    voice
}

public class SoundManager : MonoBehaviour
{
    [Serializable]
    public struct CharacterVoices
    {
        public int characterID;
        public AudioClip[] voiceClips;
    }

    // Singleton instance
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class SoundCategorySettings
    {
        public SoundCategory category;
        [Range(0f, 1f)] public float defaultVolume = 1f;
        public bool allowMultiple = true;
    }

    

    [Header("Audio Database")]
    [SerializeField] private AudioDatabase audioDatabase;
    private Dictionary<string, AudioVariation> _variationLookup;
    private Dictionary<string, AudioClip> _soundLookup; 


    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private GameObject audioSourcePrefab;
    [SerializeField] private bool expandPoolWhenEmpty = true;
    [SerializeField] private int poolExpandAmount = 5;

    [Header("Category Settings")]
    [SerializeField] private SoundCategorySettings[] categorySettings;

    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private List<AudioSource> activeAudioSources = new List<AudioSource>();
    private Dictionary<SoundCategory, float> categoryVolumes = new Dictionary<SoundCategory, float>();
    private Dictionary<SoundCategory, List<AudioSource>> activeCategorySources = new Dictionary<SoundCategory, List<AudioSource>>();

    [Header("Debug")]
    [SerializeField] private bool logPoolStatistics = false;
    [SerializeField] private bool editorOnlyLogging = true;

    private class AudioSourceFollower : MonoBehaviour
    {
        public GameObject TargetObject { get; private set; }
        private Transform targetTransform;
        private AudioSource audioSource;

        public void Initialize(Transform targetTransform)
        {
            this.targetTransform = targetTransform;
            this.TargetObject = targetTransform.gameObject;
            this.audioSource = GetComponent<AudioSource>();
            UpdatePosition();
        }

        private void Update()
        {
            if (targetTransform != null)
            {
                UpdatePosition();
            }
            else if (audioSource != null && !audioSource.isPlaying)
            {
                Destroy(gameObject);
            }
        }

        private void UpdatePosition()
        {
            if (targetTransform != null)
            {
                transform.position = targetTransform.position;
            }
            else if (audioSource != null)
            {
                SoundCategory category = SoundManager.Instance.GetSourceCategory(audioSource);
                SoundManager.Instance.ReturnAudioSourceToPool(audioSource, category);
                Destroy(this);
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize categories
        foreach (SoundCategory category in System.Enum.GetValues(typeof(SoundCategory)))
        {
            categoryVolumes[category] = 1f;
            activeCategorySources[category] = new List<AudioSource>();
        }

        foreach (var setting in categorySettings)
        {
            categoryVolumes[setting.category] = setting.defaultVolume;
        }

        InitializePool();
        InitializeVariations();
    }

    private void InitializeVariations()
    {
        _variationLookup = new Dictionary<string, AudioVariation>();
        _soundLookup = new Dictionary<string, AudioClip>();

        // Existing variation setup
        foreach (var variation in audioDatabase.sfxVariations)
        {
            if (!_variationLookup.TryAdd(variation.variationID, variation))
                Debug.LogWarning($"Duplicate SFX variation: {variation.variationID}");
        }

        foreach (var variation in audioDatabase.voiceVariations)
        {
            if (!_variationLookup.TryAdd(variation.variationID, variation))
                Debug.LogWarning($"Duplicate voice variation: {variation.variationID}");
        }

        // NEW: Add single clips from all categories
        AddClipsToLookup(audioDatabase.gameplayClips);
        AddClipsToLookup(audioDatabase.characterVoices);
        AddClipsToLookup(audioDatabase.musicTracks);
        AddClipsToLookup(audioDatabase.uiClips);
    }

    private void AddClipsToLookup(AudioClip[] clips)
    {
        if (clips == null) return;

        foreach (AudioClip clip in clips)
        {
            if (clip == null) continue;

            string key = GetKeyFromClip(clip);
            if (!_soundLookup.TryAdd(key, clip))
                Debug.LogWarning($"Duplicate clip key: {key} (from {clip.name})");
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        GameObject sourceObj = Instantiate(audioSourcePrefab, transform);
        AudioSource source = sourceObj.GetComponent<AudioSource>();
        if (source == null)
        {
            source = sourceObj.AddComponent<AudioSource>();
        }
        sourceObj.SetActive(false);
        audioSourcePool.Enqueue(source);
        return source;
    }

    #region Core Playback Methods
    public AudioSource PlaySound(AudioClip clip, SoundCategory category,
                               float volume = 1f, float pitch = 1f,
                               bool loop = false, int priority = 128)
    {
        if (clip == null)
        {
            Debug.LogWarning("Tried to play null audio clip");
            return null;
        }

        if (!GetCategorySettings(category).allowMultiple && activeCategorySources[category].Count > 0)
        {
            foreach (var activeSource in activeCategorySources[category])
            {
                ReturnAudioSourceToPool(activeSource, category);
            }
        }

        AudioSource source = GetAvailableAudioSource(priority);
        if (source == null) return null;

        ConfigureAudioSource(source, clip, volume * categoryVolumes[category], pitch, loop, priority);
        source.gameObject.SetActive(true);
        source.Play();

        activeAudioSources.Add(source);
        activeCategorySources[category].Add(source);

        if (!loop) StartCoroutine(ReturnToPoolWhenFinished(source, category));
        return source;
    }

    public AudioSource PlaySoundAtPosition(AudioClip clip, Vector3 position, SoundCategory category,
                                         float spatialBlend = 1f, float volume = 1f, float pitch = 1f)
    {
        AudioSource source = GetAvailableAudioSource(128);
        if (source == null) return null;

        source.transform.position = position;
        source.spatialBlend = spatialBlend;
        ConfigureAudioSource(source, clip, volume * categoryVolumes[category], pitch, false, 128);
        source.gameObject.SetActive(true);
        source.Play();

        activeAudioSources.Add(source);
        activeCategorySources[category].Add(source);
        StartCoroutine(ReturnToPoolWhenFinished(source, category));
        return source;
    }

    public AudioSource PlaySoundAtGameObject(AudioClip clip, GameObject targetObject, SoundCategory category,
                                        float spatialBlend = 1f, float volume = 1f, float pitch = 1f,
                                        bool followObject = true, float maxDistance = 500f,
                                        bool loop = false)  // Added here
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Tried to play sound at null GameObject");
            return null;
        }

        AudioSource source = GetAvailableAudioSource(128);
        if (source == null) return null;

        source.spatialBlend = spatialBlend;
        source.maxDistance = maxDistance;
        source.rolloffMode = AudioRolloffMode.Logarithmic;

        if (followObject)
        {
            var follower = source.gameObject.AddComponent<AudioSourceFollower>();
            follower.Initialize(targetObject.transform);
        }
        else
        {
            source.transform.position = targetObject.transform.position;
        }

        ConfigureAudioSource(source, clip, volume * categoryVolumes[category], pitch, loop, 128);  // Loop passed here
        source.gameObject.SetActive(true);
        source.Play();

        activeAudioSources.Add(source);
        activeCategorySources[category].Add(source);

        if (!loop) StartCoroutine(ReturnToPoolWhenFinished(source, category));  // Only auto-return if not looping
        return source;
    }
    #endregion

    #region Key-Based Playback Methods
    public AudioSource PlaySoundByKey(string soundKey, SoundCategory category,
                                    float volume = 1f, float pitch = 1f,
                                    bool loop = false, int priority = 128)
    {
        if (!_soundLookup.TryGetValue(soundKey, out AudioClip clip))
        {
            Debug.LogWarning($"Sound key '{soundKey}' not found!");
            return null;
        }
        return PlaySound(clip, category, volume, pitch, loop, priority);
    }

    public AudioSource PlaySoundByKeyAtPosition(string soundKey, Vector3 position, SoundCategory category,
                                              float spatialBlend = 1f, float volume = 1f, float pitch = 1f)
    {
        if (!_soundLookup.TryGetValue(soundKey, out AudioClip clip))
        {
            Debug.LogWarning($"Sound key '{soundKey}' not found!");
            return null;
        }
        return PlaySoundAtPosition(clip, position, category, spatialBlend, volume, pitch);
    }

    public AudioSource PlaySoundByKeyAtGameObject(string soundKey, GameObject targetObject, SoundCategory category,
                                              float spatialBlend = 1f, float volume = 1f, float pitch = 1f,
                                              bool followObject = true, float maxDistance = 500f,
                                              bool loop = false)  // Added loop parameter
    {
        if (!_soundLookup.TryGetValue(soundKey, out AudioClip clip))
        {
            Debug.LogWarning($"Sound key '{soundKey}' not found!");
            return null;
        }
        return PlaySoundAtGameObject(clip, targetObject, category,
                                   spatialBlend, volume, pitch,
                                   followObject, maxDistance, loop);  // Pass through loop
    }
    #endregion

    #region Variation Playback
    public AudioSource PlayVariation(string variationID, SoundCategory category,
                                   float volume = 1f, float pitch = 1f,
                                   bool loop = false, int priority = 128)
    {
        if (!_variationLookup.TryGetValue(variationID, out AudioVariation variation))
        {
            Debug.LogWarning($"Variation '{variationID}' not found in category {category}");
            return null;
        }

        AudioClip clip = variation.GetRandomClip();
        return PlaySound(clip, category, volume, pitch, loop, priority);
    }

    public AudioSource PlayVariationAtPosition(string variationID, Vector3 position, SoundCategory category,
                                             float spatialBlend = 1f, float volume = 1f, float pitch = 1f)
    {
        if (!_variationLookup.TryGetValue(variationID, out AudioVariation variation))
        {
            Debug.LogWarning($"Variation '{variationID}' not found!");
            return null;
        }
        return PlaySoundAtPosition(variation.GetRandomClip(), position, category, spatialBlend, volume, pitch);
    }

    public AudioSource PlayVariationAtObject(string variationID, GameObject targetObject, SoundCategory category,
                                           float spatialBlend = 1f, float volume = 1f, float pitch = 1f,
                                           bool followObject = true, float maxDistance = 500f)
    {
        if (!_variationLookup.TryGetValue(variationID, out AudioVariation variation))
        {
            Debug.LogWarning($"Variation '{variationID}' not found!");
            return null;
        }
        return PlaySoundAtGameObject(variation.GetRandomClip(), targetObject, category,
                                   spatialBlend, volume, pitch, followObject, maxDistance);
    }
    #endregion

    #region Pool Management
    private AudioSource GetAvailableAudioSource(int priority)
    {
        if (audioSourcePool.Count > 0) return audioSourcePool.Dequeue();

        if (expandPoolWhenEmpty)
        {
            ExpandPool();
            return audioSourcePool.Dequeue();
        }

        AudioSource lowestPrioritySource = FindLowestPriorityActiveSource();
        if (lowestPrioritySource != null && lowestPrioritySource.priority < priority)
        {
            SoundCategory category = GetSourceCategory(lowestPrioritySource);
            ReturnAudioSourceToPool(lowestPrioritySource, category);
            return audioSourcePool.Dequeue();
        }

        Debug.LogWarning("No available AudioSources in pool");
        return null;
    }

    private void ExpandPool()
    {
        for (int i = 0; i < poolExpandAmount; i++)
        {
            CreateNewAudioSource();
        }
    }

    private IEnumerator ReturnToPoolWhenFinished(AudioSource source, SoundCategory category)
    {
        yield return new WaitWhile(() => source != null && source.isPlaying);
        if (source != null) ReturnAudioSourceToPool(source, category);
    }

    public void ReturnAudioSourceToPool(AudioSource source, SoundCategory category)
    {
        if (source == null) return;

        var follower = source.GetComponent<AudioSourceFollower>();
        if (follower != null) Destroy(follower);

        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);

        activeAudioSources.Remove(source);
        activeCategorySources[category].Remove(source);
        audioSourcePool.Enqueue(source);
    }
    #endregion

    #region Helper Methods
    private SoundCategorySettings GetCategorySettings(SoundCategory category)
    {
        foreach (var setting in categorySettings)
        {
            if (setting.category == category) return setting;
        }
        return new SoundCategorySettings { category = category, defaultVolume = 1f, allowMultiple = true };
    }

    private SoundCategory GetSourceCategory(AudioSource source)
    {
        foreach (var pair in activeCategorySources)
        {
            if (pair.Value.Contains(source)) return pair.Key;
        }
        Debug.LogWarning($"AudioSource {source.name} not found in any category, defaulting to SFX");
        return SoundCategory.sfx;
    }

    private AudioSource FindLowestPriorityActiveSource()
    {
        if (activeAudioSources.Count == 0) return null;
        AudioSource lowest = activeAudioSources[0];
        foreach (var source in activeAudioSources)
        {
            if (source.priority < lowest.priority) lowest = source;
        }
        return lowest;
    }

    private void ConfigureAudioSource(AudioSource source, AudioClip clip,
                                    float volume, float pitch, bool loop, int priority)
    {
        source.clip = clip;
        source.volume = volume;
        source.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
        source.loop = loop;
        source.priority = Mathf.Clamp(priority, 0, 256);
        source.spatialBlend = 0f;
    }

    public void StopLoopingSource(AudioSource source)
    {
        if (source == null || !source.loop) return;

        SoundCategory category = GetSourceCategory(source);
        ReturnAudioSourceToPool(source, category);
    }

    #endregion

    #region Cleanup
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private string GetKeyFromClip(AudioClip clip)
    {
        // Remove "(Clone)" if present (common with instantiated assets)
        string cleanName = clip.name.Replace("(Clone)", "").Trim();
        // Remove file extension if present
        return System.IO.Path.GetFileNameWithoutExtension(cleanName);
    }
    #endregion



   

  


}