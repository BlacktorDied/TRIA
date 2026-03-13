using System.IO;
using UnityEditor;
using UnityEngine;

public class AssemblyPlatformGenerator : EditorWindow
{
    [MenuItem("TRIA/Tools/Generate Advanced Assembly Platform")]
    public static void GenerateSprite()
    {
        int width = 192;
        int height = 32;
        int frames = 4;

        Texture2D tex = new Texture2D(width, height * frames, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        // Color Palette
        Color clear = new Color(0, 0, 0, 0);
        Color metalDark = new Color(0.12f, 0.15f, 0.18f, 1f);
        Color metalBase = new Color(0.2f, 0.25f, 0.3f, 1f);
        Color metalHighlight = new Color(0.35f, 0.4f, 0.45f, 1f);
        Color cautionYellow = new Color(0.8f, 0.7f, 0.1f, 1f);
        Color cautionDark = new Color(0.1f, 0.1f, 0.1f, 1f);
        Color glowCoreDark = new Color(0.1f, 0.3f, 0.6f, 1f);
        Color glowCoreBright = new Color(0.3f, 0.8f, 1f, 1f);

        for (int f = 0; f < frames; f++)
        {
            float pulseOffset = f * (Mathf.PI / 2f); // Animation phase

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int py = y + (f * height);
                    Color pixelColor = clear;

                    // --- ZONE 1: Underbelly Trusses (y: 0 to 9) ---
                    if (y < 10)
                    {
                        if (y == 0 || y == 9)
                            pixelColor = metalDark; // Truss borders
                        else if ((x + y) % 8 == 0 || (x - y) % 8 == 0)
                            pixelColor = metalBase; // Crossbeams
                        else
                            pixelColor = clear; // Empty space between trusses
                    }
                    // --- ZONE 2: Main Chassis & Energy Core (y: 10 to 25) ---
                    else if (y >= 10 && y <= 25)
                    {
                        // Border casing
                        if (y == 10 || y == 25)
                            pixelColor = metalDark;
                        else if (y == 11 || y == 24)
                            pixelColor = metalHighlight;
                        // Glowing animated core
                        else if (y >= 15 && y <= 20)
                        {
                            // Create a moving energy wave effect
                            float wave = Mathf.Sin((x * 0.1f) + pulseOffset);
                            float intensity = Mathf.Clamp01((wave + 1f) * 0.5f);

                            // Center of the core is brighter
                            if (y == 17 || y == 18)
                                pixelColor = Color.Lerp(
                                    glowCoreDark,
                                    glowCoreBright,
                                    intensity + 0.3f
                                );
                            else
                                pixelColor = Color.Lerp(metalDark, glowCoreDark, intensity);
                        }
                        // Rivets on the chassis panels
                        else if ((y == 13 || y == 22) && (x % 16 == 8))
                            pixelColor = metalHighlight;
                        else
                            pixelColor = metalBase;
                    }
                    // --- ZONE 3: Top Walking Surface (y: 26 to 31) ---
                    else if (y > 25)
                    {
                        if (y == 31)
                            pixelColor = metalHighlight; // Top edge shine
                        else if (y == 26)
                            pixelColor = metalDark; // Bottom edge shadow
                        else
                        {
                            // Caution stripes pattern
                            bool isStripe = ((x + (y * 2)) % 16) < 8;
                            pixelColor = isStripe ? cautionYellow : cautionDark;

                            // Add a little wear and tear noise
                            if (Mathf.PerlinNoise(x * 0.3f, y * 0.3f) > 0.7f)
                                pixelColor = metalBase; // Scratches exposing metal
                        }
                    }

                    // Trim the absolute edges to round out the platform slightly
                    if ((x == 0 || x == width - 1) && (y < 12 || y > 29))
                        pixelColor = clear;

                    tex.SetPixel(x, py, pixelColor);
                }
            }
        }

        tex.Apply();

        string dir = "Assets/_Project/Art/Environment/Props";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string path = dir + "/AssemblyPlatform_Advanced_Sheet.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.Refresh();

        Debug.Log("Generated Advanced Assembly Platform Sprite Sheet at: " + path);
    }
}
