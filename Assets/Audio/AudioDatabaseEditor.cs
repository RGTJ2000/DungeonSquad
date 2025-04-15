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

        // Scan regular clips
        db.uiClips = ScanFolder("Assets/Audio/SFX/UI");
        db.gameplayClips = ScanFolder("Assets/Audio/SFX/Gameplay");
        db.musicTracks = ScanFolder("Assets/Audio/Music");
        db.characterVoices = ScanFolder("Assets/Audio/Voice");

        // Scan variations
        List<AudioVariation> sfxVariations = new List<AudioVariation>();
        List<AudioVariation> voiceVariations = new List<AudioVariation>();

        ScanVariations("Assets/Audio/SFX/Variations", sfxVariations);
        ScanVariations("Assets/Audio/Voice/Variations", voiceVariations);

        //Store results
        db.sfxVariations = sfxVariations.ToArray();
        db.voiceVariations = voiceVariations.ToArray();

       

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();

        Debug.Log("Audio database scan completed successfully");

    }
    /*
    private void ScanAudioClips()
    {
        AudioDatabase db = (AudioDatabase)target;

        // Clear existing arrays
        db.uiClips = ScanFolder("Assets/Audio/SFX/UI");
        db.sfxClips = ScanFolder("Assets/Audio/SFX/Gameplay");
        db.musicTracks = ScanFolder("Assets/Audio/Music");
        db.characterVoices = ScanFolder("Assets/Audio/Voice");

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        Debug.Log($"Audio database updated with {db.uiClips.Length + db.sfxClips.Length + db.musicTracks.Length + db.characterVoices.Length} clips");
    }
    */

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

            // Initialize all dictionaries for this group if needed
            if (!clipGroups.ContainsKey(groupKey))
            {
                clipGroups[groupKey] = new List<AudioClip>();
                nameGroups[groupKey] = new List<string>();
                fullNameGroups[groupKey] = new List<string>();
            }

            // Add to all collections
            clipGroups[groupKey].Add(clip);
            nameGroups[groupKey].Add(GetDescriptivePart(clip.name));
            fullNameGroups[groupKey].Add(clip.name);
        }

        // Create variations
        foreach (var group in clipGroups)
        {
            // Sort by version number while keeping all collections in sync
            var sortedClips = group.Value
                .Select((clip, index) => new {
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
        // Match patterns like: 
        // "clericConfirm_v1" or "clericConfirm_hello_v12"
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