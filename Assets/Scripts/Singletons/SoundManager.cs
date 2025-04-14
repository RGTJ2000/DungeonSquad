using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    [Serializable]
    public struct CharacterVoices
    {
        public int characterID; // First dimension: Character ID
        public AudioClip[] voiceClips; // Second dimension: Voice clips
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

    public enum SoundCategory
    {
        sfx,
        music,
        UI,
        ambient,
        voice
    }

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
            this.TargetObject = targetTransform.gameObject; // Set the TargetObject property
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
                // Clean up if target is destroyed
                SoundCategory category = SoundManager.Instance.GetSourceCategory(audioSource);
                SoundManager.Instance.ReturnAudioSourceToPool(audioSource, category);
                Destroy(this); // Remove this component
            }
        }
    }




    
    /*
    // Audio sources for music and sound effects
    public AudioSource characterVoiceSource;
    public AudioSource sfxSource;
    public AudioSource magicSfxSource;
    public AudioSource uiSource;
    */
    // Audio clips

    public CharacterVoices[] characterVoices;
    public AudioClip arrowLaunch;
    public AudioClip[] mmClips;
    public AudioClip click;
    public AudioClip fireBurning;
    public AudioClip fireballBoom;
  

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize category volumes and active sources tracking
        foreach (SoundCategory category in System.Enum.GetValues(typeof(SoundCategory)))
        {
            categoryVolumes[category] = 1f;
            activeCategorySources[category] = new List<AudioSource>();
        }

        // Apply settings from inspector
        foreach (var setting in categorySettings)
        {
            categoryVolumes[setting.category] = setting.defaultVolume;
        }

        InitializePool();
    }


    private void Update()
    {
        if (logPoolStatistics && Time.frameCount % 60 == 0) // Log once per second
        {
            if (!editorOnlyLogging || Application.isEditor)
            {
                Debug.Log($"SoundManager Pool: {audioSourcePool.Count} available, {activeAudioSources.Count} active");
            }
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
            Debug.LogError("AudioSource prefab is missing AudioSource component!");
            source = sourceObj.AddComponent<AudioSource>();
        }

        sourceObj.SetActive(false);
        audioSourcePool.Enqueue(source);
        return source;
    }


    #region Public Play Methods

    public AudioSource PlaySound(AudioClip clip, SoundCategory category, float volume = 1f, float pitch = 1f, bool loop = false, int priority = 128)
    {
        if (clip == null)
        {
            Debug.LogWarning("Tried to play null audio clip");
            return null;
        }

        // Check if we should stop a previous sound from this category
        if (!GetCategorySettings(category).allowMultiple && activeCategorySources[category].Count > 0)
        {
            foreach (var sound_source in activeCategorySources[category])
            {
                ReturnAudioSourceToPool(sound_source, category);
            }
        }

        AudioSource source = GetAvailableAudioSource(priority);
        if (source == null) return null;

        // Apply category volume
        float finalVolume = volume * categoryVolumes[category];

        ConfigureAudioSource(source, clip, finalVolume, pitch, loop, priority);
        source.gameObject.SetActive(true);
        source.Play();

        activeAudioSources.Add(source);
        activeCategorySources[category].Add(source);

        if (!loop)
        {
            StartCoroutine(ReturnToPoolWhenFinished(source, category));
        }

        return source;
    }

    public AudioSource PlaySoundAtPosition(AudioClip clip, Vector3 position, SoundCategory category,
                                         float spatialBlend = 1f, float volume = 1f, float pitch = 1f)
    {
        // Configure position and spatial blend BEFORE playing
        AudioSource source = GetAvailableAudioSource(128); // Default priority
        if (source == null) return null;

        // Set spatial properties first
        source.transform.position = position;
        source.spatialBlend = spatialBlend;

        // Then configure and play
        float finalVolume = volume * categoryVolumes[category];
        ConfigureAudioSource(source, clip, finalVolume, pitch, false, 128);
        source.gameObject.SetActive(true);
        source.Play();

        activeAudioSources.Add(source);
        activeCategorySources[category].Add(source);
        StartCoroutine(ReturnToPoolWhenFinished(source, category));

        return source;
    }

    public AudioSource PlaySoundAtGameObject(AudioClip clip, GameObject targetObject, SoundCategory category,
                                       float spatialBlend = 1f, float volume = 1f, float pitch = 1f,
                                       bool followObject = true, float maxDistance = 500f)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Tried to play sound at null GameObject");
            return null;
        }

        // Get source directly from pool instead of using PlaySound()
        AudioSource source = GetAvailableAudioSource(128);
        if (source == null) return null;

        // Configure ALL spatial properties FIRST
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

        // Configure and play AFTER spatial setup
        float finalVolume = volume * categoryVolumes[category];
        ConfigureAudioSource(source, clip, finalVolume, pitch, false, 128);
        source.gameObject.SetActive(true);
        source.Play();

        // Manually add to tracking lists
        activeAudioSources.Add(source);
        activeCategorySources[category].Add(source);
        StartCoroutine(ReturnToPoolWhenFinished(source, category));

        return source;
    }

    public void StopAllSoundsOnObject(GameObject target)
    {
        foreach (var source in activeAudioSources.ToArray())
        {
            var follower = source.GetComponent<AudioSourceFollower>();
            if (follower != null && follower.TargetObject == target)
            {
                SoundCategory category = GetSourceCategory(source);
                ReturnAudioSourceToPool(source, category);
            }
        }
    }

    #endregion

    #region Pool Management

    private AudioSource GetAvailableAudioSource(int priority)
    {
        if (audioSourcePool.Count > 0)
        {
            return audioSourcePool.Dequeue();
        }

        if (expandPoolWhenEmpty)
        {
            ExpandPool();
            return audioSourcePool.Dequeue();
        }

        // Try to find a lower priority sound to stop
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
        Debug.Log($"Expanding AudioSource pool by {poolExpandAmount}");
        for (int i = 0; i < poolExpandAmount; i++)
        {
            CreateNewAudioSource();
        }
    }

    private IEnumerator ReturnToPoolWhenFinished(AudioSource source, SoundCategory category)
    {
        if (source == null) yield break;
        yield return new WaitWhile(() => source != null && source.isPlaying);
        if (source != null) ReturnAudioSourceToPool(source, category);
    }

    public void ReturnAudioSourceToPool(AudioSource source, SoundCategory category)
    {
        if (source == null) return;

        // Remove follower component if it exists
        var follower = source.GetComponent<AudioSourceFollower>();
        if (follower != null)
        {
            Destroy(follower);
        }

        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);

        activeAudioSources.Remove(source);
        activeCategorySources[category].Remove(source);
        audioSourcePool.Enqueue(source);

    }

    public void StopSound(AudioSource source)
    {
        if (source != null && source.isPlaying)
        {
            SoundCategory category = GetSourceCategory(source);

            ReturnAudioSourceToPool(source, category);
        }
    }

    #endregion

    #region Category Management

    public void SetCategoryVolume(SoundCategory category, float volume)
    {
        categoryVolumes[category] = Mathf.Clamp01(volume);

        // Update volume for all active sounds in this category
        foreach (var source in activeCategorySources[category])
        {
            source.volume = (source.volume / categoryVolumes[category]) * volume;
        }
    }

    public float GetCategoryVolume(SoundCategory category)
    {
        return categoryVolumes.ContainsKey(category) ? categoryVolumes[category] : 1f;
    }

    public void StopAllInCategory(SoundCategory category)
    {
        foreach (var source in activeCategorySources[category].ToArray())
        {
            ReturnAudioSourceToPool(source, category);
        }
    }

    private SoundCategorySettings GetCategorySettings(SoundCategory category)
    {
        foreach (var setting in categorySettings)
        {
            if (setting.category == category)
            {
                return setting;
            }
        }
        return new SoundCategorySettings() { category = category, defaultVolume = 1f, allowMultiple = true };
    }

    private SoundCategory GetSourceCategory(AudioSource source)
    {
        // Search through all categories to find which one contains this source
        foreach (var pair in activeCategorySources)
        {
            if (pair.Value.Contains(source))
            {
                return pair.Key;
            }
        }

        // Default fallback if not found (shouldn't happen in normal operation)
        Debug.LogWarning($"AudioSource {source.name} not found in any category, defaulting to SFX");
        return SoundCategory.sfx;
    }

    #endregion

    #region Helper Methods

    private AudioSource FindLowestPriorityActiveSource()
    {
        if (activeAudioSources.Count == 0) return null;

        AudioSource lowestPrioritySource = activeAudioSources[0];
        foreach (var source in activeAudioSources)
        {
            if (source.priority < lowestPrioritySource.priority)
            {
                lowestPrioritySource = source;
            }
        }
        return lowestPrioritySource;
    }

    private void ConfigureAudioSource(AudioSource source, AudioClip clip, float volume, float pitch, bool loop, int priority)
    {
        source.clip = clip;
        source.volume = volume;
        source.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
        source.loop = loop;
        source.priority = Mathf.Clamp(priority, 0, 256);
        source.spatialBlend = 0f; // Default to 2D
    }

    #endregion

    #region Cleanup

    private void OnDestroy()
    {
        // Clean up all coroutines when destroyed
        StopAllCoroutines();
    }

    #endregion










    

    public void PlayFireLoop(System.Func<bool> condition)
    {
        /*
        magicSfxSource.loop = true;
        magicSfxSource.clip = fireBurning;
        magicSfxSource.Play();
        StartCoroutine(MonitorSound(magicSfxSource, condition));
        */
    }
 
    public void PlayFireballBoom()
    {
        //magicSfxSource.PlayOneShot(fireballBoom);
    }
    

    // Play a sound effect
    public void PlayCharacterSelectAffirm(int ch_ID, GameObject ch_obj)
    {

        AudioClip clip = characterVoices[ch_ID].voiceClips[ UnityEngine.Random.Range(0,2) ];

        //characterVoiceSource.PlayOneShot(clip, 1.0f);
        PlaySoundAtGameObject(clip, ch_obj, SoundCategory.voice);
        //PlaySoundAtPosition(clip, ch_obj.transform.position, SoundCategory.voice);
        //PlaySound(clip, SoundCategory.voice);
    }

    
    public void PlayArrowLaunch(GameObject launch_obj)
    {
        //sfxSource.PlayOneShot(arrowLaunch, .6f);
        PlaySoundAtPosition(arrowLaunch, launch_obj.transform.position, SoundCategory.sfx, 1, 0.6f, 1);
    }

    public void PlayMMLaunch()
    {
        //magicSfxSource.PlayOneShot(mmClips[0]);
    }

    public void PlayMMBoom()
    {
        //magicSfxSource.PlayOneShot(mmClips[1]);
    }
    
    public void PlayClick()
    {
        //uiSource.PlayOneShot(click);
    }

    private IEnumerator MonitorSound(AudioSource source, System.Func<bool> condition)
    {
        while (source != null && condition())
        {
            yield return null; // Wait one frame
        }
        if (source != null)
        {
            source.Stop();
            source.loop = false;
        }
    }
    
}