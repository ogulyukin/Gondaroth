using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Diagnostics;

namespace TerrainEditor
{
    public class TextureCreatorWindow : EditorWindow
    {
        private string m_Filename = "myProceduralTexture";
        private float m_PerlinXScale;
        private float m_PerlinYScale;
        private int m_PerlinOctaves;
        private float m_PerlinPersistence;
        private float m_PerlinHeightScale;
        private int m_PerlinOffsetX;
        private int m_PerlinOffsetY;
        private bool m_AlphaToggle = false;
        private bool m_SeamlessToggle = false;
        private bool m_MapToggle = false;
        private Texture2D m_Texture;

        private float m_Brightness = 0.5f;
        private float m_Contrast = 0.5f;

        [MenuItem("Window/TextureCreatorWindow")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TextureCreatorWindow));

        }

        private void OnEnable()
        {
            m_Texture = new Texture2D(513, 513, TextureFormat.ARGB32, false);
        }

        private void OnGUI()
        {
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            m_Filename = EditorGUILayout.TextField("Texture Name", m_Filename);

            var wSize = (int)(EditorGUIUtility.currentViewWidth - 100);
            
            m_PerlinXScale = EditorGUILayout.Slider("X Scale", m_PerlinXScale, 0, 0.1f);
            m_PerlinYScale = EditorGUILayout.Slider("X Scale", m_PerlinYScale, 0, 0.1f);
            m_PerlinOctaves = EditorGUILayout.IntSlider("Octaves", m_PerlinOctaves, 0, 10);
            m_PerlinPersistence = EditorGUILayout.Slider("Persistence", m_PerlinPersistence, 0, 10f);
            m_PerlinHeightScale = EditorGUILayout.Slider("Height scale", m_PerlinHeightScale, 0, 1);
            m_PerlinOffsetX = EditorGUILayout.IntSlider("Offset X", m_PerlinOffsetX, 0, 10000);
            m_Brightness = EditorGUILayout.Slider("Brightness", m_Brightness, 0, 2);
            m_Contrast = EditorGUILayout.Slider("Contrast", m_Contrast, 0, 2);
            m_PerlinOffsetY = EditorGUILayout.IntSlider("Offset Y", m_PerlinOffsetY, 0, 10000);
            m_AlphaToggle = EditorGUILayout.Toggle("Alpha?", m_AlphaToggle);
            m_MapToggle = EditorGUILayout.Toggle("Map?", m_MapToggle);
            m_SeamlessToggle = EditorGUILayout.Toggle("Seamless", m_SeamlessToggle);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var minColor = 1f;
            var maxColor = 0f;

            if (GUILayout.Button("Generate", GUILayout.Width(wSize)))
            {
                int w = 513;
                int h = 513;
                float pValue;
                Color pixCol = Color.white;
                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        if (m_SeamlessToggle)
                        {
                            var u = (float)x / (float)w;
                            var v = (float)y / (float)h;
                            float noise00 = Tools.FractalBrowningMotion((x + m_PerlinOffsetX) * m_PerlinXScale,
                                                (y + m_PerlinOffsetY) * m_PerlinYScale, m_PerlinOctaves, m_PerlinPersistence) *
                                            m_PerlinHeightScale;
                            float noise01 = Tools.FractalBrowningMotion((x + m_PerlinOffsetX) * m_PerlinXScale,
                                                (y + m_PerlinOffsetY + h) * m_PerlinYScale, m_PerlinOctaves, m_PerlinPersistence) *
                                            m_PerlinHeightScale;
                            float noise10 = Tools.FractalBrowningMotion((x + m_PerlinOffsetX + w) * m_PerlinXScale,
                                                (y + m_PerlinOffsetY) * m_PerlinYScale, m_PerlinOctaves, m_PerlinPersistence) *
                                            m_PerlinHeightScale;
                            float noise11 = Tools.FractalBrowningMotion((x + m_PerlinOffsetX + w) * m_PerlinXScale,
                                                (y + m_PerlinOffsetY + h) * m_PerlinYScale, m_PerlinOctaves, m_PerlinPersistence) *
                                            m_PerlinHeightScale;
                            var noiseTotal = u * v * noise00 +
                                             u * (1 - v) * noise01 +
                                             (1 - u) * v * noise10 +
                                             (1 - u) * (1 - v) * noise11;
                            float value = (int)(256 * noiseTotal) + 50;
                            float r = Mathf.Clamp((int)noise00, 0, 255);
                            float g = Mathf.Clamp(value, 0, 255);
                            float b = Mathf.Clamp(value + 50, 0, 255);
                            float a = Mathf.Clamp(value + 100, 0, 255);
                            pValue = (r + g + b) / (3 * 255.0f);
                        }
                        else
                        {
                            pValue = Tools.FractalBrowningMotion((x + m_PerlinOffsetX) * m_PerlinXScale,
                                         (y + m_PerlinOffsetY) * m_PerlinYScale, m_PerlinOctaves, m_PerlinPersistence) *
                                     m_PerlinHeightScale;    
                        }
                        
                        var colValue = m_Contrast * (pValue - 0.5f) + 0.5f * m_Brightness;
                        if (minColor > colValue) minColor = colValue;
                        if (maxColor < colValue) maxColor = colValue;
                        pixCol = new Color(colValue, colValue, colValue, m_AlphaToggle ? colValue : 1);
                        m_Texture.SetPixel(x, y, pixCol);
                    }
                }

                if (m_MapToggle)
                {
                    for (var y = 0; y < h; y++)
                    {
                        for (var x = 0; x < w; x++)
                        {
                            pixCol = m_Texture.GetPixel(x, y);
                            var colValue = pixCol.r;
                            colValue = Tools.Map(colValue, minColor, maxColor, 0, 1);
                            pixCol.r = colValue;
                            pixCol.g = colValue;
                            pixCol.b = colValue;
                            m_Texture.SetPixel(x, y, pixCol);
                        }
                    }
                }
                m_Texture.Apply(false, false);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(m_Texture, GUILayout.Width(wSize), GUILayout.Height(wSize));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", GUILayout.Width(wSize)))
            {
                var data = m_Texture.EncodeToPNG();
                System.IO.Directory.CreateDirectory($"{Application.dataPath}/SavedTextures");
                File.WriteAllBytes($"{Application.dataPath}/SavedTextures/{m_Filename}.png",data );
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
