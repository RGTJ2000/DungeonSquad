using UnityEngine;

[System.Serializable]
public class AudioVariation
{
    public string variationID;  // e.g. "fighter_confirm"
    public AudioClip[] clips;   // All variations for this sound

    [Tooltip("Optional descriptive names")]
    [HideInInspector] public string[] descriptiveNames; // For editor debugging only

    [Tooltip("For editor use only - not runtime accessible")]
    [HideInInspector] public string[] fullNames; // Optional: store original names for debugging

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

}



[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Audio/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    [Header("SFX")]
    public AudioVariation[] sfxVariations;
    public AudioClip[] gameplayClips;

    [Header("VOICE")]
    public AudioVariation[] voiceVariations;
    public AudioClip[] characterVoices;

    [Header("MUSIC")]
    public AudioClip[] musicTracks;

    [Header("UI")]
    public AudioClip[] uiClips;

}