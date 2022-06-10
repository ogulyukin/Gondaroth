using EditorGUITable;
using UnityEditor;
using UnityEngine;

namespace TerrainEditor
{
    [CustomEditor(typeof(CustomTerrain))]
    [CanEditMultipleObjects]
    public class CustomTerrainEditor : Editor
    {
#if UNITY_EDITOR
        private SerializedProperty m_CurrentTerrain;
        private SerializedProperty m_ResetTerrain;
        private SerializedProperty m_RandomHeightRange;
        private SerializedProperty m_HeightMapScale;
        private SerializedProperty m_HeightMapImage;
        
        private SerializedProperty m_PerlinXScale;
        private SerializedProperty m_PerlinYScale;
        private SerializedProperty m_PerlinOffsetX;
        private SerializedProperty m_PerlinOffsetY;
        private SerializedProperty m_PerlinOctaves;
        private SerializedProperty m_PerlinPersistence;
        private SerializedProperty m_PerlinHeightScale;

        private GUITableState m_PerlinParameterTable;

        private SerializedProperty m_PeakMinRange;
        private SerializedProperty m_PeakMaxRange;
        private SerializedProperty m_PeakFallOf;
        private SerializedProperty m_PeakDropOff;
        private SerializedProperty m_PeakNumber;
        private SerializedProperty m_VoronoiType;

        private SerializedProperty m_MDMax;
        private SerializedProperty m_MDMin;
        private SerializedProperty m_MDRoughness;
        private SerializedProperty m_MDHeightDamper;

        private SerializedProperty m_SmoothNum;

        private bool m_ShowHeightMapControl;
        private bool m_ShowRandom;
        private bool m_ShowLoadHeights;
        private bool m_ShowPerlin;
        private bool m_ShowMultiplePerlin;
        private bool m_ShowVoronoi;
        private bool m_ShowIOMenu;
        private bool m_ShowMidpointDisplacement;
        private bool m_ShowSmooth;
        private bool m_ShowWater;
        private bool m_ShowTexturingControl;
        private bool m_ShowVegetation;
        private bool m_ShowDetails;
        private bool m_ShowErosion;
        private bool m_ShowClouds;

        private GUITableState m_TerrainLayerTable;
        
        private GUITableState m_TreeMeshesTable;
        private SerializedProperty m_TreesAmount;
        private SerializedProperty m_TreesSpacing;
        
        private GUITableState m_DetailsTable;
        private SerializedProperty m_DetailsAmount;
        private SerializedProperty m_DetailSpacing;
        
        private SerializedProperty m_WaterHeight;
        private SerializedProperty m_WaterGo;
        private SerializedProperty m_ShoreLineMaterial;
        private SerializedProperty m_ShoreLineWidth;

        private SerializedProperty m_RiseTerrain;
        private SerializedProperty m_ErosionType;
        private SerializedProperty m_ErosionStrength;
        private SerializedProperty m_SpringsPerRiver;
        private SerializedProperty m_Solubility;
        private SerializedProperty m_Droplets;
        private SerializedProperty m_ErosionSmoothAmount;
        private SerializedProperty m_ErosionAmount;

        private SerializedProperty m_NumClouds;
        private SerializedProperty m_ParticlesPerCloud;
        private SerializedProperty m_CloudStartSize;
        private SerializedProperty m_CloudScaleMin;
        private SerializedProperty m_CloudScaleMax;
        private SerializedProperty m_CloudMaterial;
        private SerializedProperty m_CloudShadowMaterial;
        private SerializedProperty m_CloudColor;
        private SerializedProperty m_CloudLining;
        private SerializedProperty m_CloudMinSpeed;
        private SerializedProperty m_CloudMaxSped;
        private SerializedProperty m_CloudRange;
#endif
        private void OnEnable()
        {
#if UNITY_EDITOR
            m_ShowHeightMapControl = false;
            m_ShowRandom = false;
            m_ShowLoadHeights = false;
            m_ShowPerlin = false;
            m_ShowMultiplePerlin = false;
            m_ShowVoronoi = false;
            m_ShowIOMenu = false;
            m_ShowMidpointDisplacement = false;
            m_ShowSmooth = false;
            m_ShowWater = false;
            m_ShowTexturingControl = false;
            m_ShowVegetation = false;
            m_ShowDetails = false;
            m_ShowErosion = false;
            m_ShowClouds = false;
            m_CurrentTerrain = serializedObject.FindProperty("terrain");
            m_ResetTerrain = serializedObject.FindProperty("resetTerrain");
            m_RandomHeightRange = serializedObject.FindProperty("randomHeightRange");
            m_HeightMapImage = serializedObject.FindProperty("heightMapImage");
            m_HeightMapScale = serializedObject.FindProperty("heightMapScale");
            m_PerlinXScale = serializedObject.FindProperty("perlinXScale");
            m_PerlinYScale = serializedObject.FindProperty("perlinYScale");
            m_PerlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
            m_PerlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
            m_PerlinOctaves = serializedObject.FindProperty("perlinOctaves");
            m_PerlinPersistence = serializedObject.FindProperty("perlinPersistence");
            m_PerlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
            m_PerlinParameterTable = new GUITableState("PerlinParametersTable");
            m_PeakMinRange = serializedObject.FindProperty("randomPeakMinRange");
            m_PeakMaxRange = serializedObject.FindProperty("randomPeakMaxRange");
            m_PeakFallOf = serializedObject.FindProperty("randomPeakFallOff");
            m_PeakDropOff = serializedObject.FindProperty("randomPeakDropOff");
            m_PeakNumber = serializedObject.FindProperty("randomPeakNumber");
            m_VoronoiType = serializedObject.FindProperty("voronoiType");
            m_MDMax = serializedObject.FindProperty("mdMax");
            m_MDMin = serializedObject.FindProperty("mdMin");
            m_MDRoughness = serializedObject.FindProperty("mdRoughness");
            m_MDHeightDamper = serializedObject.FindProperty("mdHeightDamper");
            m_SmoothNum = serializedObject.FindProperty("smoothNum");
            m_TerrainLayerTable = new GUITableState("splatHeightTable");
            m_TreeMeshesTable = new GUITableState("treeMeshesTable");
            m_TreesAmount = serializedObject.FindProperty("treesAmount");
            m_TreesSpacing = serializedObject.FindProperty("treesSpacing");
            m_DetailsTable = new GUITableState("detailsItemTable");
            m_DetailsAmount = serializedObject.FindProperty("detailsAmount");
            m_DetailSpacing = serializedObject.FindProperty("detailsSpacing");
            m_WaterHeight = serializedObject.FindProperty("waterHeight");
            m_WaterGo = serializedObject.FindProperty("waterGo");
            m_RiseTerrain = serializedObject.FindProperty("riseTerrain");
            m_ShoreLineMaterial = serializedObject.FindProperty("shoreLineMaterial");
            m_ShoreLineWidth = serializedObject.FindProperty("shoreLineWidth");
            m_ErosionType = serializedObject.FindProperty("erosionType");
            m_ErosionStrength = serializedObject.FindProperty("erosionStrength");
            m_SpringsPerRiver = serializedObject.FindProperty("springsPerRiver");
            m_Solubility = serializedObject.FindProperty("solubility");
            m_Droplets = serializedObject.FindProperty("droplets");
            m_ErosionSmoothAmount = serializedObject.FindProperty("erosionSmoothAmount");
            m_ErosionAmount = serializedObject.FindProperty("erosionAmount");
            m_NumClouds = serializedObject.FindProperty("numClouds");
            m_ParticlesPerCloud = serializedObject.FindProperty("particlesPerCloud");
            m_CloudStartSize = serializedObject.FindProperty("cloudStartSize");
            m_CloudScaleMin = serializedObject.FindProperty("cloudScaleMin");
            m_CloudScaleMax = serializedObject.FindProperty("cloudScaleMax");
            m_CloudMaterial = serializedObject.FindProperty("cloudMaterial");
            m_CloudShadowMaterial = serializedObject.FindProperty("cloudShadowMaterial");
            m_CloudColor = serializedObject.FindProperty("cloudColor");
            m_CloudLining = serializedObject.FindProperty("cloudLining");
            m_CloudMinSpeed = serializedObject.FindProperty("cloudMinSpeed");
            m_CloudMaxSped = serializedObject.FindProperty("cloudMaxSpeed");
            m_CloudRange = serializedObject.FindProperty("cloudRange");
#endif
        }
#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CustomTerrain terrain = (CustomTerrain)target;
            EditorGUILayout.PropertyField(m_CurrentTerrain, new GUIContent("Current Terrain"));
            m_ShowHeightMapControl = EditorGUILayout.Foldout(m_ShowHeightMapControl, "Terrain Generation");
            if (m_ShowHeightMapControl)
            {
                EditorGUILayout.PropertyField(m_ResetTerrain);

                EditorGUILayout.Slider(m_RiseTerrain, 0, 1, new GUIContent("Rise Terrain"));
                if (GUILayout.Button("Rise Terrain"))
                {
                    terrain.RiseTerrain();
                }
            
                m_ShowLoadHeights = EditorGUILayout.Foldout(m_ShowLoadHeights, "Load heights from Image");
                if (m_ShowLoadHeights)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("Load heights from Texture Image", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(m_HeightMapImage);
                    EditorGUILayout.PropertyField(m_HeightMapScale);
                    if (GUILayout.Button("Load Texture"))
                    {
                        terrain.LoadTexture();
                    }
                }
                m_ShowRandom = EditorGUILayout.Foldout(m_ShowRandom, "Random");
                if (m_ShowRandom)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(m_RandomHeightRange);
                    if (GUILayout.Button("Random Heights"))
                    {
                        terrain.RandomTerrain();
                    }
                }

                m_ShowPerlin = EditorGUILayout.Foldout(m_ShowPerlin, "Single Perlin Noise");
                if(m_ShowPerlin)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("Perlin Noise", EditorStyles.boldLabel);
                    EditorGUILayout.Slider(m_PerlinXScale, 0, 1, new GUIContent("X Scale"));
                    EditorGUILayout.Slider(m_PerlinYScale, 0, 1, new GUIContent("Y Scale"));
                    EditorGUILayout.IntSlider(m_PerlinOffsetX, 0, 10000, new GUIContent("X Offset"));
                    EditorGUILayout.IntSlider(m_PerlinOffsetY, 0, 10000, new GUIContent("Y Offset"));
                    EditorGUILayout.IntSlider(m_PerlinOctaves, 1, 10, new GUIContent("Octaves"));
                    EditorGUILayout.Slider(m_PerlinPersistence, 0.1f, 10, new GUIContent("Persistence"));
                    EditorGUILayout.Slider(m_PerlinHeightScale, 0, 1, new GUIContent("Height Scale"));
                    if (GUILayout.Button("Generate Terrain"))
                    {
                        terrain.PerLin();
                    }
                }

                m_ShowMultiplePerlin = EditorGUILayout.Foldout(m_ShowMultiplePerlin, "Multiple Perlin Noise");
                if (m_ShowMultiplePerlin)
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    GUILayout.Label("Multiple Perlin Noise", EditorStyles.boldLabel);
                    m_PerlinParameterTable = GUITableLayout.DrawTable(m_PerlinParameterTable, 
                        serializedObject.FindProperty("perlinParameters"));
                    GUILayout.Space(20);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        terrain.AddNewPerlin();
                    }

                    if (GUILayout.Button("-"))
                    {
                        terrain.RemovePerlin();
                    }
                    EditorGUILayout.EndHorizontal();
                    if (GUILayout.Button("Apply Multiple Perlin"))
                    {
                        terrain.MultiplePerlinTerrain();
                    }
                }

                m_ShowVoronoi = EditorGUILayout.Foldout(m_ShowVoronoi, "Voronoi");
                if (m_ShowVoronoi)
                {
                    EditorGUILayout.IntSlider(m_PeakNumber, 1, 10, new GUIContent("Peak Number"));
                    EditorGUILayout.Slider(m_PeakFallOf, 0, 10f, new GUIContent("Falloff"));
                    EditorGUILayout.Slider(m_PeakDropOff, 0, 10f, new GUIContent("Dropoff"));
                    EditorGUILayout.Slider(m_PeakMinRange, 0, 1, new GUIContent("Min Height"));
                    EditorGUILayout.Slider(m_PeakMaxRange, 0, 1, new GUIContent("Max Height"));
                    EditorGUILayout.PropertyField(m_VoronoiType);
                    if (GUILayout.Button("Add Peak"))
                    {
                        terrain.Voronoi();
                    }
                }
            
                m_ShowMidpointDisplacement = EditorGUILayout.Foldout(m_ShowMidpointDisplacement, "Midpoint Displacement");
                if (m_ShowMidpointDisplacement)
                {
                    EditorGUILayout.Slider(m_MDMin, -30, 0, new GUIContent("Min Height"));
                    EditorGUILayout.Slider(m_MDMax, 0, 30, new GUIContent("Max Height"));
                    EditorGUILayout.Slider(m_MDRoughness, 0, 10, new GUIContent("Roughness"));
                    EditorGUILayout.PropertyField(m_MDHeightDamper,new GUIContent("HeightDamper"));
                if (GUILayout.Button("MPD"))
                    {
                        terrain.MidpointDisplacement();
                    }
                }
                
                m_ShowErosion = EditorGUILayout.Foldout(m_ShowErosion, "Erosion");
                if (m_ShowErosion)
                {
                    EditorGUILayout.PropertyField(m_ErosionType);
                    EditorGUILayout.Slider(m_ErosionStrength, 0, 1, new GUIContent("Strength"));
                    EditorGUILayout.Slider(m_ErosionAmount, 0.01f, 1, new GUIContent("Erosion Amount"));
                    EditorGUILayout.IntSlider(m_Droplets, 0, 500, new GUIContent("Droplets"));
                    EditorGUILayout.Slider(m_Solubility, 0.001f, 1, new GUIContent("Solubility"));
                    EditorGUILayout.IntSlider(m_SpringsPerRiver, 1, 20, new GUIContent("Springs per river"));
                    EditorGUILayout.IntSlider(m_ErosionSmoothAmount, 0, 10, new GUIContent("Smooth amount"));
                    if (GUILayout.Button("Erode"))
                    {
                        terrain.Erode();
                    }
                }
                m_ShowSmooth = EditorGUILayout.Foldout(m_ShowSmooth, "Smooth Terrain");
                if (m_ShowSmooth)
                {
                    EditorGUILayout.IntSlider(m_SmoothNum, 1, 10, new GUIContent("Smooth Amount"));
                    if (GUILayout.Button("Smooth"))
                    {
                        terrain.SmoothTerrain();
                    }
                }
            
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                if (GUILayout.Button("Undo Last Action"))
                {
                    terrain.UndoLastAction();
                }

                m_ShowIOMenu = EditorGUILayout.Foldout(m_ShowIOMenu, "Save/Load");
                if (m_ShowIOMenu)
                {
                    if (GUILayout.Button("Save Height Map"))
                    {
                        var path = EditorUtility.SaveFilePanel("Save Height Map", "",
                        "terrain.csv", "csv");
                        terrain.SaveHeightMap(path); 
                    }

                    if (GUILayout.Button("Load Height Map"))
                    {
                        var path = EditorUtility.OpenFilePanel("Load Height Map", "", "csv");
                        terrain.LoadHeightMap(path);
                    }    
                }
            
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                if (GUILayout.Button("Reset Terrain HeightMap"))
                {
                    terrain.ResetHeightMap();
                }
                serializedObject.ApplyModifiedProperties();
            }

            m_ShowTexturingControl = EditorGUILayout.Foldout(m_ShowTexturingControl, "Texturing");
            if (m_ShowTexturingControl)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Label("Terrain Layers", EditorStyles.boldLabel);
                m_TerrainLayerTable = GUITableLayout.DrawTable(m_TerrainLayerTable,
                    serializedObject.FindProperty("splatHeights"));
                GUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+"))
                {
                    terrain.AddNewTerrainLayer();
                }

                if (GUILayout.Button("-"))
                {
                    terrain.RemoveTerrainLayer();
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Apply Splat Map"))
                {
                    terrain.SplatMaps();
                }
            }
            
            m_ShowVegetation = EditorGUILayout.Foldout(m_ShowVegetation, "Vegetation System");
            if (m_ShowVegetation)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Label("Vegetation", EditorStyles.boldLabel);
                EditorGUILayout.IntSlider(m_TreesAmount, 1, 20000, new GUIContent("Maximum Trees"));
                EditorGUILayout.IntSlider(m_TreesSpacing, 2, 20, new GUIContent("Tree Spacing"));
                m_TreeMeshesTable = GUITableLayout.DrawTable(m_TreeMeshesTable,
                    serializedObject.FindProperty("treeMeshes"));
                GUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+"))
                {
                    terrain.AddTreeMesh();
                }

                if (GUILayout.Button("-"))
                {
                    terrain.RemoveTreeMesh();
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Apply Vegetation"))
                {
                    terrain.ApplyVegetation();
                }
                if (GUILayout.Button("Clear Vegetation"))
                {
                    terrain.ClearTrees();
                }
            }
            
            m_ShowWater = EditorGUILayout.Foldout(m_ShowWater, "Water");
            if (m_ShowWater)
            {
                EditorGUILayout.Slider(m_WaterHeight, 0.0f, 1.0f, new GUIContent("Water Height"));
                EditorGUILayout.PropertyField(m_WaterGo);

                if (GUILayout.Button("Add Water"))
                {
                    terrain.AddWater();
                }

                EditorGUILayout.PropertyField(m_ShoreLineMaterial);
                EditorGUILayout.IntSlider(m_ShoreLineWidth, 1, 25, new GUIContent("Shore Line Width"));
                if (GUILayout.Button("Add Shoreline"))
                {
                    terrain.DrawShoreLine();
                }
            }
            
            m_ShowClouds = EditorGUILayout.Foldout(m_ShowClouds, "Clouds");
            if (m_ShowClouds)
            {
                EditorGUILayout.PropertyField(m_NumClouds, new GUIContent("Number of Clouds"));
                EditorGUILayout.PropertyField(m_ParticlesPerCloud, new GUIContent("Particles Per Clouds"));
                EditorGUILayout.PropertyField(m_CloudStartSize, new GUIContent("Cloud Particle Size"));
                EditorGUILayout.PropertyField(m_CloudScaleMin, new GUIContent("Cloud Size Min"));
                EditorGUILayout.PropertyField(m_CloudScaleMax, new GUIContent("Cloud Size Max"));
                EditorGUILayout.PropertyField(m_CloudMaterial, true);
                EditorGUILayout.PropertyField(m_CloudShadowMaterial, true);
                EditorGUILayout.PropertyField(m_CloudColor, new GUIContent("Color"));
                EditorGUILayout.PropertyField(m_CloudLining, new GUIContent("Lining"));
                EditorGUILayout.PropertyField(m_CloudMinSpeed, new GUIContent("Min Speed"));
                EditorGUILayout.PropertyField(m_CloudMaxSped, new GUIContent("Max Speed"));
                EditorGUILayout.PropertyField(m_CloudRange, new GUIContent("Distance Travelled"));
                if (GUILayout.Button("Generate Clouds"))
                {
                    terrain.GenerateClouds();
                }
            }
            
            m_ShowDetails = EditorGUILayout.Foldout(m_ShowDetails, "Details");
            if (m_ShowDetails)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Label("Details", EditorStyles.boldLabel);
                EditorGUILayout.IntSlider(m_DetailsAmount, 1, 10000, new GUIContent("Maximum Details"));
                EditorGUILayout.IntSlider(m_DetailSpacing, 1, 20, new GUIContent("Details Spacing"));
                
                m_DetailsTable = GUITableLayout.DrawTable(m_DetailsTable,
                    serializedObject.FindProperty("myDetails"));
                GUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+"))
                {
                    terrain.AddDetailItem();
                }

                if (GUILayout.Button("-"))
                {
                    terrain.RemoveDetailItem();
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Apply Details"))
                {
                    terrain.ApplyDetails();
                }
            }
        }
        #endif
    }
}
