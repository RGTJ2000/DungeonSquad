#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

[CustomEditor(typeof(AudioDatabase))]
public class AudioDatabaseEditor : Editor
{
    private struct ParsedVariationName
    {
        public string BaseID;       // "clericConfirm"
        public string Description;  // "hello", "whatsup"
        public string Variant;      // "v1", "v2"
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(20);

        if (GUILayout.Button("Scan All Audio", GUILayout.Height(30)))
        {
            ScanAllAudio();
        }


    }
    private void ScanAllAudio()
    {
        AudioDatabase db = (AudioDatabase)target;

        // Clear existing arrays
        db.uiClips = ScanFolder("Assets/Audio/SFX/UI");
        db.musicTracks = ScanFolder("Assets/Audio/Music");

        // Scan variations and individual clips
        List<AudioVariation> sfxVariations = new List<AudioVariation>();
        List<AudioVariation> voiceVariations = new List<AudioVariation>();
        List<AudioClip> gameplayClips = new List<AudioClip>();
        List<AudioClip> characterVoices = new List<AudioClip>();

        // Process SFX
        ScanVariations("Assets/Audio/SFX/Variations", sfxVariations);
        gameplayClips.AddRange(ScanFolder("Assets/Audio/SFX/Gameplay"));
        // Add all variation clips to gameplayClips too
        foreach (var variation in sfxVariations)
        {
            gameplayClips.AddRange(variation.clips);
        }

        // Process Voice
        ScanVariations("Assets/Audio/Voice/Variations", voiceVariations);
        characterVoices.AddRange(ScanFolder("Assets/Audio/Voice"));
        // Add all variation clips to characterVoices too
        foreach (var variation in voiceVariations)
        {
            characterVoices.AddRange(variation.clips);
        }

        // Store results
        db.sfxVariations = sfxVariations.ToArray();
        db.voiceVariations = voiceVariations.ToArray();
        db.gameplayClips = gameplayClips.ToArray();
        db.characterVoices = characterVoices.ToArray();

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        Debug.Log($"Audio database updated with:\n" +
                 $"- {db.sfxVariations.Length} SFX variations\n" +
                 $"- {db.voiceVariations.Length} voice variations\n" +
                 $"- {db.gameplayClips.Length} gameplay clips\n" +
                 $"- {db.characterVoices.Length} character voice clips");

    }


    private AudioClip[] ScanFolder(string folderPath)
    {
        List<AudioClip> clips = new List<AudioClip>();
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip != null) clips.Add(clip);
        }

        return clips.ToArray();
    }

    private void ScanVariations(string folderPath, List<AudioVariation> variations)
    {
        var clipGroups = new Dictionary<string, List<AudioClip>>();
        var nameGroups = new Dictionary<string, List<string>>();
        var fullNameGroups = new Dictionary<string, List<string>>();

        foreach (var clip in ScanFolder(folderPath))
        {
            var parsed = ParseVariationName(clip.name);
            string groupKey = parsed.BaseID;

            if (!clipGroups.ContainsKey(groupKey))
            {
                clipGroups[groupKey] = new List<AudioClip>();
                nameGroups[groupKey] = new List<string>();
                fullNameGroups[groupKey] = new List<string>();
            }

            clipGroups[groupKey].Add(clip);
            nameGroups[groupKey].Add(GetDescriptivePart(clip.name));
            fullNameGroups[groupKey].Add(clip.name);
        }

        foreach (var group in clipGroups)
        {
            var sortedClips = group.Value
                .Select((clip, index) => new
                {
                    Clip = clip,
                    DescriptiveName = nameGroups[group.Key][index],
                    FullName = fullNameGroups[group.Key][index],
                    Version = int.Parse(ParseVariationName(clip.name).Variant)
                })
                .OrderBy(x => x.Version)
                .ToList();

            variations.Add(new AudioVariation
            {
                variationID = group.Key,
                clips = sortedClips.Select(x => x.Clip).ToArray(),
                descriptiveNames = sortedClips.Select(x => x.DescriptiveName).ToArray(),
                fullNames = sortedClips.Select(x => x.FullName).ToArray()
            });
        }
    }

    private ParsedVariationName ParseVariationName(string fullName)
    {

        var match = Regex.Match(fullName, @"^(.*?)(?:_([a-zA-Z]+))?(?:_v(\d+))?$");
        return new ParsedVariationName
        {
            BaseID = match.Groups[1].Value,
            Description = match.Groups[2].Value,
            Variant = match.Groups[3].Value
        };
        
    }


    // Optional: Extract descriptive middle part
    private string GetDescriptivePart(string fullName)
    {
        var parsed = ParseVariationName(fullName);
        return !string.IsNullOrEmpty(parsed.Description) ?
               $"{parsed.BaseID}_{parsed.Description}" :
               parsed.BaseID;
    }

}
#endif