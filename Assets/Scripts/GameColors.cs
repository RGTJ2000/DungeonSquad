using UnityEngine;

/// <summary>
/// Contains a collection of predefined colors for the game.
/// All colors are of the 'Color' type for direct use with most Unity components.
/// </summary>
public static class GameColors
{
    // Green
    public static readonly Color GreenCRT = new Color32(30, 190, 5, 255);

    // Amber
    public static readonly Color AmberCRTSolid = new Color32(221, 147, 39, 255);
    public static readonly Color AmberCRTTransparent = new Color32(221, 147, 39, 150);

    // Blue
    public static readonly Color BlueCRTSolid = new Color32(85, 85, 255, 255);
    public static readonly Color BlueCRTTransparent = new Color32(85, 85, 255, 150);

    // Light Blue
    public static readonly Color LightBlueCRTSolid = new Color32(85, 255, 255, 255);
    public static readonly Color LightBlueCRTTransparent = new Color32(85, 255, 255, 150);

    // Red
    public static readonly Color RedCRTSolid = new Color32(196, 0, 0, 255);
    public static readonly Color RedCRTTransparent = new Color32(196, 0, 0, 150);
}