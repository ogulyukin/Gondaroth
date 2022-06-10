using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Linq;
using TMPro;
using Random = UnityEngine.Random;

namespace TerrainEditor
{
    [ExecuteInEditMode]
    public class CustomTerrain : MonoBehaviour
    {
        [SerializeField] private int terrainLayer = 0;
        public Terrain terrain;
        public bool resetTerrain;       
        public Vector2 randomHeightRange;
        public Texture2D heightMapImage;
        public Vector3 heightMapScale;       
        public TerrainData terrainData;

        public float perlinXScale;
        public float perlinYScale;
        public int perlinOffsetX;
        public int perlinOffsetY;
        public int perlinOctaves;
        public float perlinPersistence;
        public float perlinHeightScale;
        
        public float randomPeakMinRange;
        public float randomPeakMaxRange;
        public float randomPeakFallOff;
        public float randomPeakDropOff;
        public int randomPeakNumber;
        public enum VoronoiType {Linear = 0, Power = 1, Combined = 2, SinPow = 3 }
        public VoronoiType voronoiType;
        
        public float mdMax;
        public float mdMin;
        public float mdRoughness;
        public float mdHeightDamper;

        public int smoothNum;

        private float[,] m_HeightMapBackup;
        private float[,] m_CopyHeightMap;
        
        public float waterHeight = 0.5f;
        public GameObject waterGo = null;
        public Material shoreLineMaterial;
        public int shoreLineWidth = 10;

        public int detailsAmount;
        public int detailsSpacing;

        public float riseTerrain;
        
        public enum ErosionType {Rain = 0, Thermal = 1, Tidal = 2, River = 3, Wind = 4, Canyon = 5}
        public enum SerializeType {Splatmap, Vegetation, Details}

        public ErosionType erosionType = ErosionType.Rain;
        public float erosionStrength = 0.1f;
        public float erosionAmount = 0.01f;
        public int springsPerRiver = 5;
        public float solubility = 0.01f;
        public int droplets = 10;
        public int erosionSmoothAmount = 5;

        public int numClouds = 1;
        public int particlesPerCloud = 50;
        public Vector3 cloudScaleMin = new Vector3(1, 1, 1);
        public Vector3 cloudScaleMax = new Vector3(1, 1, 1);
        public Material cloudMaterial;
        public Material cloudShadowMaterial;
        public float cloudStartSize = 5;
        public Color cloudColor = Color.white;
        public Color cloudLining = Color.gray;
        public float cloudMinSpeed = 0.2f;
        public float cloudMaxSpeed = 0.5f;
        public float cloudRange = 500.0f;
        
        [System.Serializable]
        public class DetailItem
        {
            public GameObject prototype = null;
            public bool isGrass = false;
            public Texture2D prototypeTexture = null;
            public float minHeight = 0.0f;
            public float maxHeight = 0.1f;
            public float minSlope = 0f;
            public float maxSlope = 1.0f;
            public Color dryColor = Color.white;
            public Color healthyColor = Color.white;
            public Vector2 heightRange = new Vector2(1, 1);
            public Vector2 widthRange = new Vector2(1, 1);
            public float noiseSpread = 0.5f;
            public float overlap = 0.01f;
            public float feather = 0.05f;
            public float density = 0.01f;
            public bool remove = false;
        }

        public List<DetailItem> myDetails = new List<DetailItem>(){new DetailItem()};

        public int treesAmount;
        public int treesSpacing;
        [System.Serializable]
        public class Vegetation
        {
            public GameObject treeMesh = null;
            public float minHeight = 0.1f;
            public float maxHeight = 0.2f;
            public float minSlope = 0f;
            public float maxSlope = 90f;
            public Color color01 = Color.white;
            public Color color02 = Color.white;
            public Color lightColor = Color.white;
            public float minScale = 0.5f;
            public float maxScale = 1.0f;
            public float minRotation = 0;
            public float maxRotation = 360;
            public float density = 0.5f;
            public bool remove = false;
        }

        public List<Vegetation> treeMeshes = new List<Vegetation>() { new Vegetation() };
        
        [System.Serializable]
        public class SplatHeight
        {
            public Texture2D texture = null;
            public TerrainLayer layer = null;
            public float minHeight = 0.1f;
            public float maxHeight = 0.2f;
            public float minSlope = 0f;
            public float maxSlope = 90f;
            public Vector2 tileSize = new Vector2(20, 20);
            public Vector2 tileOffset = new Vector2(0, 0);
            public float offset = 0.1f;
            public float noiseXScale = 0.01f;
            public float noiseYScale = 0.01f;
            public float noseScaler = 0.1f;
            public bool remove = false;
        }

        public List<SplatHeight> splatHeights = new List<SplatHeight>(){new SplatHeight()};
        
        [System.Serializable]
        public class PerlinParameter
        {
            public float perlinXScale = 0.01f;
            public float perlinYScale = 0.01f;
            public int perlinOffsetX = 0;
            public int perlinOffsetY = 0;
            public int perlinOctaves = 3;
            public float perlinPersistence = 8f;
            public float perlinHeightScale = 0.09f;
            public bool remove = false;
        }
        public List<PerlinParameter> perlinParameters = new List<PerlinParameter>() {new PerlinParameter()};
        private void OnEnable()
        {
            resetTerrain = true;
            randomHeightRange = new Vector2(0, 0.1f);
            randomPeakMinRange = 0; randomPeakMaxRange = 0.3f;
            heightMapScale = new Vector3(1,1,1);
            perlinXScale = perlinYScale = 0.01f;
            perlinOffsetX = perlinOffsetY = 0;
            perlinOctaves = 3;
            perlinPersistence = 8f;
            perlinHeightScale = 0.09f;
            Debug.Log("Initialising Terrain Data");
            randomPeakFallOff = 0.2f;
            randomPeakDropOff = 0.6f;
            randomPeakNumber = 1;
            voronoiType = VoronoiType.Linear;
            mdMax = 2f;
            mdMin = -2f;
            mdRoughness = 2.0f;
            mdHeightDamper = 2.0f;
            smoothNum = 1;
            treesAmount = 5000;
            treesSpacing = 5;
            detailsSpacing = 5;
            detailsAmount = 5000;
            riseTerrain = 0.1f;
            terrainData = terrain.terrainData;  //Terrain.activeTerrain.terrainData;
            m_HeightMapBackup = GetHeightMap(true);
        }

        private void Awake()
        {
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            AddTag(tagsProp, "Terrain");
            AddTag(tagsProp, "Cloud");
            AddTag(tagsProp, "Shore");

            SerializedProperty layerProp = tagManager.FindProperty("layers");
            terrainLayer = AddLayer(layerProp, "Terrain");
            
            tagManager.ApplyModifiedProperties();
            this.gameObject.tag = "Terrain";
            this.gameObject.layer = terrainLayer;
        }

        public void GenerateClouds()
        {
            var cloudManager = GameObject.Find("CloudManager");
            if (!cloudManager)
            {
                cloudManager = new GameObject();
                cloudManager.name = "CloudManager";
                cloudManager.AddComponent<CloudManager>();
                cloudManager.transform.position = this.transform.position;
            }

            GameObject[] allClouds = GameObject.FindGameObjectsWithTag("Cloud");
            for (int i = 0; i < allClouds.Length; i++)
            {
                DestroyImmediate(allClouds[i]);
            }

            for (var c = 0; c < numClouds; c++)
            {
                var cloudGO = new GameObject()
                {
                    name = $"Cloud {c}",
                    tag = "Cloud",
                    transform =
                    {
                        rotation = cloudManager.transform.rotation,
                        position = cloudManager.transform.position,
                        parent = cloudManager.transform,
                        localScale = new Vector3(1,1,1)
                    }
                };

                var cloudSystem = cloudGO.AddComponent<ParticleSystem>();
                var cloudRend = cloudGO.GetComponent<Renderer>();
                cloudRend.material = cloudMaterial;

                cloudGO.layer = LayerMask.NameToLayer("Sky");
                if (Random.Range(0, 10) < 5)
                {
                    var cloudProjector = new GameObject()
                    {
                        name = "Shadow",
                        transform =
                        {
                            position = cloudGO.transform.position,
                            forward = Vector3.down,
                            parent = cloudGO.transform
                        }
                    };

                    var skyLayerMask = 1 << LayerMask.NameToLayer("Sky");
                    var waterLayerMask = 1 << LayerMask.NameToLayer("Water");
                    var cp = cloudProjector.AddComponent<Projector>();
                    cp.material = cloudShadowMaterial;
                    cp.farClipPlane = terrainData.size.y;
                    cp.ignoreLayers = skyLayerMask | waterLayerMask;
                    cp.fieldOfView = 20.0f;
                }
                
                cloudRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                cloudRend.receiveShadows = false;
                var main = cloudSystem.main;
                main.loop = false;
                main.startLifetime = Mathf.Infinity;
                main.startSpeed = 0;
                main.startSize = cloudStartSize;
                main.startColor = cloudColor;
                var emission = cloudSystem.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[]{ new ParticleSystem.Burst(0.0f, (short)particlesPerCloud)});
                var shape = cloudSystem.shape;
                shape.shapeType = ParticleSystemShapeType.Sphere;
                shape.scale = new Vector3(Random.Range(cloudScaleMin.x, cloudScaleMax.x), Random.Range(cloudScaleMin.y, cloudScaleMax.y), Random.Range(cloudScaleMin.z, cloudScaleMax.z));
                var cc = cloudGO.AddComponent<CloudController>();
                cc.color = cloudColor;
                cc.lining = cloudLining;
                cc.distance = cloudRange;
                cc.minSpeed = cloudMinSpeed;
                cc.maxSpeed = cloudMaxSpeed;
                cc.numberOfParticles = particlesPerCloud;
            }
        }

        public void Erode()
        {
            switch (erosionType)
            {
                case ErosionType.Rain: 
                    Rain();
                    break;
                case ErosionType.Tidal:
                    Tidal();
                    break;
                case ErosionType.Thermal:
                    Thermal();
                    break;
                case ErosionType.River:
                    River();
                    break;
                case ErosionType.Wind:
                    Wind();
                    break;
                case ErosionType.Canyon:
                    DigCanyon();
                    break;
            }

            smoothNum = erosionSmoothAmount;
            SmoothTerrain(false);
        }

        private void DigCanyon()
        {
            m_CopyHeightMap = GetHeightMap(true);
            var digDepth = 0.05f;
            var bankSlope = 0.001f;
            var maxDepth = 0f;
            var cx = 1;
            var cy = Random.Range(10, terrainData.heightmapResolution - 10);
            while (cy >= 0 && cy < terrainData.heightmapResolution && cx >= 0 &&
                   cx < terrainData.heightmapResolution)
            {
                CanyonCrawler(cx, cy, m_CopyHeightMap[cx, cy] - digDepth, bankSlope, maxDepth);
                cx += Random.Range(1, 3);
                cy += Random.Range(-2, 3);
            }
            terrainData.SetHeights(0, 0, m_CopyHeightMap);
        }

        private void CanyonCrawler(int x, int y, float height, float slope, float maxDepth)
        {
            if (x < 0 || x >= terrainData.heightmapResolution) return;
            if (y < 0 || y >= terrainData.heightmapResolution) return;
            if(height <= maxDepth) return;
            if(m_CopyHeightMap[x, y] <= height) return;

            m_CopyHeightMap[x, y] = height;
            CanyonCrawler(x + 1, y, height + Random.Range(slope, slope + 0.01f), slope, maxDepth);
            CanyonCrawler(x - 1, y, height + Random.Range(slope, slope + 0.01f), slope, maxDepth);
            CanyonCrawler(x + 1, y + 1, height + Random.Range(slope, slope + 0.01f), slope, maxDepth);
            CanyonCrawler(x - 1, y + 1, height + Random.Range(slope, slope + 0.01f), slope, maxDepth);
            CanyonCrawler(x, y + 1, height + Random.Range(slope, slope + 0.01f), slope, maxDepth);
            CanyonCrawler(x, y - 1, height + Random.Range(slope, slope + 0.01f), slope, maxDepth);
        }
        private void Rain()
        {
            var heightMap = GetHeightMap(true);
            m_HeightMapBackup = GetHeightMap(true);
            
            for (var i = 0; i < droplets; i++)
            {
                heightMap[Random.Range(0, terrainData.heightmapResolution),
                    Random.Range(0, terrainData.heightmapResolution)] -= erosionStrength;
            }
            
            terrainData.SetHeights(0,0, heightMap);
        }

        private void Tidal()
        {
            var heightMap = GetHeightMap(true);
            m_HeightMapBackup = GetHeightMap(true);
            
            for (var y = 0; y < terrainData.heightmapResolution; y++)
            {
                for (var x = 0; x < terrainData.heightmapResolution; x++)
                {
                    Vector2 thisLocation = new Vector2(x, y);
                    List<Vector2> neighbours = GenerateNeighbours(thisLocation, terrainData.heightmapResolution,
                        terrainData.heightmapResolution);
                    foreach (var n in neighbours)
                    {
                        if (heightMap[x, y] < waterHeight && heightMap[(int)n.x, (int)n.y] > waterHeight)
                        {
                            heightMap[x, y] = waterHeight;
                            heightMap[(int)n.x, (int)n.y] = waterHeight;
                        }
                    }
                }
            }
            
            terrainData.SetHeights(0,0, heightMap);
        }

        private void Thermal()
        {
            var heightMap = GetHeightMap(true);
            m_HeightMapBackup = GetHeightMap(true);

            for (var y = 0; y < terrainData.heightmapResolution; y++)
            {
                for (var x = 0; x < terrainData.heightmapResolution; x++)
                {
                    Vector2 thisLocation = new Vector2(x, y);
                    List<Vector2> neighbours = GenerateNeighbours(thisLocation, terrainData.heightmapResolution,
                        terrainData.heightmapResolution);
                    foreach (var n in neighbours)
                    {
                        if (heightMap[x, y] > heightMap[(int)n.x, (int)n.y] + erosionStrength)
                        {
                            var currentHeight = heightMap[x, y];
                            heightMap[x, y] -= currentHeight * erosionAmount;
                            heightMap[(int)n.x, (int)n.y] += currentHeight * erosionAmount;
                        }
                    }
                }
            }
            
            terrainData.SetHeights(0,0, heightMap);
        }

        private void River()
        {
            var heightMap = GetHeightMap(true);
            var erosionMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
            m_HeightMapBackup = GetHeightMap(true);

            for (int i = 0; i < droplets; i++)
            {
                Vector2 dropletPosition = new Vector2(UnityEngine.Random.Range(0, terrainData.heightmapResolution),
                    UnityEngine.Random.Range(0, terrainData.heightmapResolution));
                erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] = erosionStrength;
                for (var j = 0; j < springsPerRiver; j++)
                {
                    erosionMap = RunRiver(dropletPosition, heightMap, erosionMap);
                }
            }

            for (var y = 0; y < terrainData.heightmapResolution; y++)
            {
                for (var x = 0; x < terrainData.heightmapResolution; x++)
                {
                    if (erosionMap[x, y] > 0)
                    {
                        heightMap[x, y] -= erosionMap[x, y];
                    }
                }
            }
            
            terrainData.SetHeights(0,0, heightMap);
        }

        private float[,] RunRiver(Vector2 dropletPosition, float[,] heighMap, float[,] erosionMap)
        {
            while (erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] > 0)
            {
                List<Vector2> neighbours = GenerateNeighbours(dropletPosition, terrainData.heightmapResolution,
                    terrainData.heightmapResolution);
                //neighbours.Shuffle();
                neighbours = neighbours.OrderBy(x => Random.value).ToList();
                var foundLower = false;
                foreach (var n in neighbours)
                {
                    if (heighMap[(int)n.x, (int)n.y] < heighMap[(int)dropletPosition.x, (int)dropletPosition.y])
                    {
                        erosionMap[(int)n.x, (int)n.y] = erosionMap[(int)dropletPosition.x, (int)dropletPosition.y];
                        dropletPosition = n;
                        foundLower = true;
                        break;
                    }
                }

                if (!foundLower)
                {
                    erosionMap[(int)dropletPosition.x, (int)dropletPosition.y] -= solubility;
                }
            }
            return erosionMap;
        }

        private void Wind()
        {
            var heightMap = GetHeightMap(true);
            m_HeightMapBackup = GetHeightMap(true);
            var width = terrainData.heightmapResolution;

            float WindDirection = 0;
            var sinAngle = -Mathf.Sin(Mathf.Deg2Rad * WindDirection);
            var cosAngle = Mathf.Cos(Mathf.Deg2Rad * WindDirection);
            
            for (var y = -(width - 1) * 2; y <= width * 2; y += 10 )
            {
                for (var x = -(width - 1) * 2; x <= width * 2; x++ )
                {
                    float thisNoise = (float)Mathf.PerlinNoise(x * 0.06f, y * 0.06f) * 20 * erosionStrength;
                    var nx = x;
                    var ny = y + 5 + (int)thisNoise;
                    var digy = y + (int)thisNoise;
                    
                    var digCoords = new Vector2(x * cosAngle - digy * sinAngle, digy * cosAngle + x * sinAngle);
                    var pileCoords = new Vector2(nx * cosAngle - ny * sinAngle, ny * cosAngle + nx * sinAngle);
                    
                    if (!(pileCoords.x < 0 || pileCoords.x > (width - 1) || pileCoords.y < 0 || pileCoords.y > (width - 1)
                            || digCoords.x < 0 || digCoords.x > (width - 1) || digCoords.y < 0 || digCoords.y > (width - 1)))
                    {
                        heightMap[(int)digCoords.x, (int)digCoords.y] -= 0.001f;
                        heightMap[(int)pileCoords.x, (int)pileCoords.y] += 0.001f;
                    }
                }
            }
            
            terrainData.SetHeights(0,0, heightMap);
        }
        public void RiseTerrain()
        {
            var heightMap = GetHeightMap(true);
            m_HeightMapBackup = GetHeightMap(true);
            for (var x = 0; x < terrainData.heightmapResolution; x++)
            {
                for (var y = 0; y < terrainData.heightmapResolution; y++)
                {
                    heightMap[x, y] += riseTerrain;
                    if (heightMap[x, y] > 1) heightMap[x, y] = 1;
                }
            }
            terrainData.SetHeights(0, 0, heightMap);
        }
        public void AddWater()
        {
            var water = GameObject.Find("Water");
            if (!water)
            {
                water = Instantiate(waterGo, transform.position, transform.rotation);
                water.name = "Water";
            }

            water.transform.position = transform.position +
                                       new Vector3(terrainData.size.x / 2, waterHeight * terrainData.size.y, terrainData.size.z / 2);
            water.transform.localScale = new Vector3(terrainData.size.x, 1, terrainData.size.z);
        }

        public void DrawShoreLine()
        {
            var heightMap = GetHeightMap(true);
            for (var y = 0; y < terrainData.heightmapResolution; y++)
            {
                for (var x = 0; x < terrainData.heightmapResolution; x++)
                {
                    Vector2 thisLocation = new Vector2(x, y);
                    List<Vector2> neighbours = GenerateNeighbours(thisLocation, terrainData.heightmapResolution,
                        terrainData.heightmapResolution);
                    foreach (var n in neighbours)
                    {
                        if (heightMap[x, y] < waterHeight && heightMap[(int)n.x, (int)n.y] > waterHeight)
                        {
                            
                                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                                go.transform.localScale *= shoreLineWidth;
                                go.transform.position = transform.position +
                                                        new Vector3(
                                                            y / (float)terrainData.heightmapResolution *
                                                            terrainData.size.z, waterHeight * terrainData.size.y,
                                                            x / (float)terrainData.heightmapResolution *
                                                            terrainData.size.x);
                                go.transform.LookAt(new Vector3(n.y / (float)terrainData.heightmapResolution * 
                                                                terrainData.size.z, waterHeight * terrainData.size.y, 
                                    n.x / (float)terrainData.heightmapResolution * terrainData.size.x));
                                go.transform.Rotate(90, 0, 0);
                                go.tag = "Shore";
                        }
                    }
                }
            }

            var shoreQuads = GameObject.FindGameObjectsWithTag("Shore");
            var meshFilters = new MeshFilter[shoreQuads.Length];
            for (var m = 0; m < shoreQuads.LongLength; m++)
            {
                meshFilters[m] = shoreQuads[m].GetComponent<MeshFilter>();
            }

            var combine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }

            var currentShoreLine = GameObject.Find("ShoreLine");
            if (currentShoreLine)
            {
                DestroyImmediate(currentShoreLine);
            }

            var shoreLine = new GameObject();
            shoreLine.name = "ShoreLine";
            shoreLine.transform.position = transform.position;
            shoreLine.transform.rotation = transform.rotation;
            shoreLine.AddComponent<WaveAnimation>();
            var thisMF = shoreLine.AddComponent<MeshFilter>();
            thisMF.mesh = new Mesh();
            shoreLine.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
            var r = shoreLine.AddComponent<MeshRenderer>();
            r.sharedMaterial = shoreLineMaterial;
            for (var i = 0; i < shoreQuads.Length; i++)
            {
                DestroyImmediate(shoreQuads[i]);
            }
        }
        
        public void AddDetailItem()
        {
            myDetails.Add(new DetailItem());
        }

        public void RemoveDetailItem()
        {
            for (var i = myDetails.Count - 1; i >= 0; i--)
            {
                if(myDetails[i].remove) myDetails.RemoveAt(i);
            }
            if(myDetails.Count == 0) AddDetailItem();
        }

        public void ApplyDetails()
        {
            var newDetailPrototypes = new DetailPrototype[myDetails.Count];
            var index = 0;
            var heightMap = GetHeightMap(true);
            
            foreach (var d in myDetails)
            {
                newDetailPrototypes[index] = new DetailPrototype()
                {
                    prototype = d.prototype,
                    prototypeTexture = d.prototypeTexture,
                    healthyColor = d.healthyColor,
                    dryColor = d.dryColor,
                    minHeight = d.heightRange.x,
                    maxHeight = d.heightRange.y,
                    minWidth = d.widthRange.x,
                    maxWidth = d.widthRange.y,
                    noiseSpread = d.noiseSpread,
                };

                if (newDetailPrototypes[index].prototype && !d.isGrass)
                {
                    newDetailPrototypes[index].usePrototypeMesh = true;
                    newDetailPrototypes[index].renderMode = DetailRenderMode.VertexLit;
                }else if (newDetailPrototypes[index].prototype && d.isGrass)
                {
                    newDetailPrototypes[index].usePrototypeMesh = true;
                    newDetailPrototypes[index].renderMode = DetailRenderMode.Grass;
                }else
                {
                    newDetailPrototypes[index].usePrototypeMesh = false;
                    newDetailPrototypes[index].renderMode = DetailRenderMode.GrassBillboard;
                }

                index++;
            }

            terrainData.detailPrototypes = newDetailPrototypes;
              
            for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
            {
                var detailMap = new int[terrainData.detailWidth, terrainData.detailHeight];
                for (var y = 0; y < terrainData.detailHeight; y += detailsSpacing)
                {
                    for (var x = 0; x < terrainData.detailWidth; x += detailsSpacing)
                    {
                        if(Random.Range(0.0f, 1.0f) > myDetails[i].density) continue;
                        var xHM = (int)(x / (float)terrainData.detailWidth * terrainData.heightmapResolution);
                        var yHM = (int)(y / (float)terrainData.detailHeight * terrainData.heightmapResolution);
                        var thisNoise = Tools.Map(Mathf.PerlinNoise(x * myDetails[i].feather, y * myDetails[i].feather),
                            0, 1, 0.5f, 1);
                        var thisHeightStart = myDetails[i].minHeight * thisNoise - myDetails[i].overlap * thisNoise;
                        var nextHeightStart = myDetails[i].maxHeight * thisNoise + myDetails[i].overlap * thisNoise;

                        var thisHeight = heightMap[yHM, xHM];
                        var stepness = terrainData.GetSteepness(xHM / (float)terrainData.size.x,
                            yHM / (float)terrainData.size.z);
                        if((thisHeight >= thisHeightStart && thisHeight <= nextHeightStart) && 
                           stepness >= myDetails[i].minSlope && stepness <= myDetails[i].maxSlope )
                            detailMap[y, x] = 1;
                    }
                }
                terrainData.SetDetailLayer(0,0,i,detailMap);
            }
        }
        
        public void AddTreeMesh()
        {
            treeMeshes.Add(new Vegetation());
        }

        public void RemoveTreeMesh()
        {
            for (var i = treeMeshes.Count - 1; i >= 0; i--)
            {
                if(treeMeshes[i].remove) treeMeshes.RemoveAt(i);
            }
            if(treeMeshes.Count == 0) AddTreeMesh();
        }

        public void ApplyVegetation()
        {
            var newTreePrototypes = new List<TreePrototype>();
            foreach (var t in treeMeshes)
            {
                if(t.treeMesh== null) continue;
                newTreePrototypes.Add( new TreePrototype
                {
                    prefab = t.treeMesh
                });
            }

            terrainData.treePrototypes = newTreePrototypes.ToArray();
            var allVegetation = new List<TreeInstance>();
            for (int z = 0; z < terrainData.size.z; z += treesSpacing)
            {
                for (int x = 0; x < terrainData.size.x; x += treesSpacing)
                {
                    for (int i = 0; i < terrainData.treePrototypes.Length; i++)
                    {
                        if (UnityEngine.Random.Range(0.0f, 1.0f) > treeMeshes[i].density) continue;
                        var thisHeight = terrainData.GetHeight(x, z) / terrainData.size.y;
                        var steepness = terrainData.GetSteepness((float)x / terrainData.size.x,
                            (float)z / terrainData.size.z);
                        if (thisHeight > treeMeshes[i].maxHeight || thisHeight < treeMeshes[i].minHeight) continue;
                        if (treeMeshes[i].maxSlope >= steepness && treeMeshes[i].minSlope <= steepness)
                        {
                            var treeScale = UnityEngine.Random.Range(treeMeshes[i].minScale, treeMeshes[i].maxScale);
                            var instance = new TreeInstance
                            {
                                position = new Vector3((x + UnityEngine.Random.Range(-5.0f, 5.0f)) / terrainData.size.x, thisHeight, 
                                    (z + UnityEngine.Random.Range(-5.0f, 5.0f)) / terrainData.size.z),
                                rotation = UnityEngine.Random.Range(treeMeshes[i].minRotation, treeMeshes[i].maxRotation),
                                prototypeIndex = i,
                                color = Color.Lerp(treeMeshes[i].color01, treeMeshes[i].color02, UnityEngine.Random.Range(0.0f, 1.0f)),
                                lightmapColor = treeMeshes[i].lightColor,
                                heightScale = treeScale,
                                widthScale = treeScale
                            };

                            var treeWorldPos = new Vector3(instance.position.x * terrainData.size.x,
                            instance.position.y * terrainData.size.y,
                            instance.position.z * terrainData.size.z) + this.transform.position;
                        
                            int layerMask = 1 << terrainLayer;
                            if (Physics.Raycast(treeWorldPos + new Vector3(0, 10, 0), -Vector3.up, out var hit, 100, layerMask) ||
                                Physics.Raycast(treeWorldPos - new Vector3(0, 10, 0), Vector3.up, out hit, 100, layerMask))
                            {
                                var treeHeight = (hit.point.y - this.transform.position.y) / terrainData.size.y;
                                instance.position = new Vector3(instance.position.x, treeHeight, instance.position.z);
                                //instance.position = new Vector3(instance.position.x * terrainData.size.x / terrainData.alphamapWidth,
                                //    treeHeight, instance.position.z * terrainData.size.z / terrainData.alphamapHeight);
                            }
                            else
                            {
                                continue;
                            }
                            allVegetation.Add(instance);
                            if (allVegetation.Count >= treesAmount)
                            {
                                terrainData.treeInstances = allVegetation.ToArray();
                                return;
                            }
                        }
                    }
                }
            }
            terrainData.treeInstances = allVegetation.ToArray();
        }

        public void ClearTrees()
        {
            if (terrainData.treeInstances.Length > 0)
            {
                terrainData.treeInstances = Array.Empty<TreeInstance>();
            }
        }
        public void SplatMaps()
        {
            var newSplatPrototypes = new TerrainLayer[splatHeights.Count];
            var spIndex = 0;
            foreach (var sp in splatHeights)
            {
                if (sp.layer == null)
                {
                    newSplatPrototypes[spIndex] = new TerrainLayer
                    {
                        diffuseTexture = sp.texture,
                        tileOffset = sp.tileOffset,
                        tileSize = sp.tileSize
                    };
                    newSplatPrototypes[spIndex].diffuseTexture.Apply(true);
                    var path = $"Assets/Game/Textures/NewLayer{spIndex}.terrainlayer";
                    AssetDatabase.CreateAsset(newSplatPrototypes[spIndex], path);
                }
                else
                {
                    newSplatPrototypes[spIndex] = sp.layer;
                }
                spIndex++;
                Selection.activeObject = this.gameObject;
            }
            terrainData.terrainLayers = newSplatPrototypes;
            var heightMap = GetHeightMap(true);
            var splatMapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight,
                terrainData.alphamapLayers];
            for (var y = 0; y < terrainData.alphamapHeight; y++)
            {
                for (var x = 0; x < terrainData.alphamapWidth; x++)
                {
                    var splat = new float[terrainData.alphamapLayers];
                    for (var i = 0; i < splatHeights.Count; i++)
                    {
                        float noise = Mathf.PerlinNoise(x * splatHeights[i].noiseXScale, y * splatHeights[i].noiseYScale) * 
                                      splatHeights[i].noseScaler;
                        var offset = splatHeights[i].offset + noise;
                        var thisHeightStart = splatHeights[i].minHeight - offset;
                        var thisHeightStop = splatHeights[i].maxHeight + offset;
                        var steepness = terrainData.GetSteepness((float)y / terrainData.alphamapHeight,
                            (float)x / terrainData.alphamapWidth);
                        if (heightMap[x, y] >= thisHeightStart && heightMap[x, y] <= thisHeightStop &&
                            steepness >= splatHeights[i].minSlope && steepness <= splatHeights[i].maxSlope)
                        {
                            splat[i] = 1;
                        }
                    }
                    NormalizeVector(splat);
                    for (var j = 0; j < splatHeights.Count; j++)
                    {
                        splatMapData[x, y, j] = splat[j];
                    }
                }
            }
            terrainData.SetAlphamaps(0,0, splatMapData);
        }

        private void NormalizeVector(float[] v)
        {
            float total = 0;
            foreach (var t in v)
            {
                total += t;
            }

            for (var i = 0; i < v.Length; i++)
            {
                v[i] /= total;
            }
        }
        
        public void AddNewTerrainLayer()
        {
            splatHeights.Add(new SplatHeight());
        }

        public void RemoveTerrainLayer()
        {
            for (var i = splatHeights.Count - 1; i >= 0; i--)
            {
                if(splatHeights[i].remove) splatHeights.RemoveAt(i);
            }
            if(splatHeights.Count == 0) AddNewTerrainLayer();
        }

        public void SmoothTerrain(bool backup = true)
        {
            var heightMap = GetHeightMap(true);
            if(backup) m_HeightMapBackup = GetHeightMap(true);
            float smoothProgress = 0;
            EditorUtility.DisplayProgressBar("Smoothing terrain", "Progress", smoothProgress);
            
            for (int k = 0; k < smoothNum; k++)
            {
                for (var i = 0; i < terrainData.heightmapResolution; i++)
                {
                    for (var j = 0; j < terrainData.heightmapResolution; j++)
                    {
                        float avgHeight = heightMap[i, j];
                        var neighbours = GenerateNeighbours(new Vector2(i, j), terrainData.heightmapResolution,
                            terrainData.heightmapResolution);
                        foreach (var n in neighbours)
                        {
                            avgHeight += heightMap[(int)n.x, (int)n.y];
                        }

                        heightMap[i, j] = avgHeight / ((float)neighbours.Count + 1);
                    }
                }

                smoothProgress++;
                EditorUtility.DisplayProgressBar("Smoothing terrain", "Progress", smoothProgress/smoothNum);
            }
            
            terrainData.SetHeights(0,0, heightMap);
            EditorUtility.ClearProgressBar();
        }

        private List<Vector2> GenerateNeighbours(Vector2 pos, int width, int height)
        {
            var neighbours = new List<Vector2>();
            for (var y = -1; y < 2; y++)
            {
                for (var x = -1; x < 2; x++)
                {
                    if (!(x == 0 && y == 0))
                    {
                        var nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1),
                            Mathf.Clamp(pos.y + y, 0, height - 1));
                        if(!neighbours.Contains(nPos)) neighbours.Add(nPos);
                    }
                }
            }
            return neighbours;
        }
        public void MidpointDisplacement()
        {
            var heightMap = GetHeightMap();
            m_HeightMapBackup = GetHeightMap(true);
            var width = terrainData.heightmapResolution - 1;
            var squareSize = width;
            int cornerX, cornerY;
            int midX, midY;
            int pmidXL, pmidXR, pmidYU, pmidYD;
            var heightMin = mdMin;
            var heightMax = mdMax;
            var heightDampener = (float)Mathf.Pow(mdHeightDamper, -1 * mdRoughness);
 
            while (squareSize > 0)
            {
                for (var i = 0; i < width; i  += squareSize)
                {
                    for (var  j = 0;  j < width;  j += squareSize)
                    {
                        cornerX = (i + squareSize);
                        cornerY = (j + squareSize);
                        midX = (int)(i + squareSize / 2.0f);
                        midY = (int)(j + squareSize / 2.0f);
                        heightMap[midX, midY] = (float)((heightMap[i, j] + heightMap[cornerX, j] + 
                                                         heightMap[i, cornerY] + heightMap[cornerX, cornerY])
                                                        / 4.0f + Random.Range(heightMin, heightMax));
                    }
                }
                
                for (var i = 0; i < width; i  += squareSize)
                {
                    for (var  j = 0;  j < width;  j += squareSize)
                    {
                        cornerX = (i + squareSize);
                        cornerY = (j + squareSize);
                        midX = (int)(i + squareSize / 2.0f);
                        midY = (int)(j + squareSize / 2.0f);
                        pmidXR = (int)(midX + squareSize);
                        pmidYU = (int)(midY + squareSize);
                        pmidXL = (int)(midX - squareSize);
                        pmidYD = (int)(midY - squareSize);

                        if (pmidXL <= 0 || pmidYD <= 0 || pmidXR >= width - 1 || pmidYU >= width - 1) continue;
                        heightMap[midX, j] = (float)((heightMap[midX, midY] + heightMap[i, j] +
                                                      heightMap[midX, pmidYD] + heightMap[cornerX, j])
                            / 4.0f + Random.Range(heightMin, heightMax));
                        heightMap[midX, cornerY] = (float)((heightMap[midX, midY] + heightMap[i, cornerY] +
                                                            heightMap[midX, pmidYU] + heightMap[cornerX, cornerY])
                            / 4.0f + Random.Range(heightMin, heightMax));
                        heightMap[i, midY] = (float)((heightMap[i, cornerY] + heightMap[i, j] +
                                                      heightMap[midX, midY] + heightMap[pmidXL, midY])
                            / 4.0f + Random.Range(heightMin, heightMax));
                        heightMap[cornerX, midY] = (float)((heightMap[cornerX, cornerY] + heightMap[midX, j] +
                                                            heightMap[midX, midY] + heightMap[pmidXR, midY])
                            / 4.0f + Random.Range(heightMin, heightMax));
                    }
                }
                squareSize = (int)(squareSize / 2.0f);
                heightMax *= heightDampener;
                heightMin *= heightDampener;
            }
            
            terrainData.SetHeights(0, 0, heightMap);
        }
        public void UndoLastAction()
        {
            var heightMap = GetHeightMap();
            for (int i = 0; i < terrainData.heightmapResolution; i++)
            {
                for (int  j = 0;  j < terrainData.heightmapResolution;  j++)
                {
                    heightMap[i, j] = m_HeightMapBackup[i, j];
                }
            }
            terrainData.SetHeights(0, 0, heightMap);
        }
        public void SaveHeightMap(string path)
        {
            var heightMap = GetHeightMap(true);
            var map = new StringBuilder();
            for (int i = 0; i < terrainData.heightmapResolution; i++)
            {
                for (int  j = 0;  j < terrainData.heightmapResolution;  j++)
                {
                    map.Append($"{heightMap[i, j].ToString()};");
                }
            }
            
            try
            {
                using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.CreateNew), Encoding.UTF8, 512, false))
                { 
                    writer.WriteLine(map.ToString()); //Async do not work (((
                }
            }catch(Exception exc)
            {
                print(exc.Message); 
            }
        }

        public void LoadHeightMap(string path)
        {
            var heightMap = GetHeightMap(true);
            if(!System.IO.File.Exists(path))
            {
               print("Error: Incorrect file name!");
               return;
            }
            
            try
            {
                var sr = new StreamReader(path);
                var data = sr.ReadToEnd();
                var heights = data.Split(';');
                int count = 0;
                for (int i = 0; i < terrainData.heightmapResolution; i++)
                {
                    for (int  j = 0;  j < terrainData.heightmapResolution;  j++)
                    {
                        heightMap[i, j] = Convert.ToSingle(heights[count]);
                        count++;
                    }
                }
            }
            catch (Exception exc)
            {
                print(exc.ToString());
            }
            terrainData.SetHeights(0, 0, heightMap);
        }
        
        
        
        public void Voronoi()
        {
            var heightMap = GetHeightMap();
            m_HeightMapBackup = GetHeightMap(true);
            for (var i = 0; i < randomPeakNumber; i++)
            {
                var peakLocation = new KeyValuePair<int, int>(UnityEngine.Random.Range(1, terrainData.heightmapResolution),
                    UnityEngine.Random.Range(0, terrainData.heightmapResolution));
                var peakHeight = UnityEngine.Random.Range(randomPeakMinRange, randomPeakMaxRange);
                if(heightMap[peakLocation.Key, peakLocation.Value] > peakHeight) continue;
                heightMap[peakLocation.Key, peakLocation.Value] += peakHeight;
                var maxDistance = Vector2.Distance(new Vector2(0, 0),
                    new Vector2(terrainData.heightmapResolution, terrainData.heightmapResolution));
                for (var y = 0; y < terrainData.heightmapResolution; y++)
                {
                    for (var x = 0; x < terrainData.heightmapResolution; x++)
                    {
                        if (x == peakLocation.Key && y == peakLocation.Value) continue;
                        var distanceToPeak = Vector2.Distance(new Vector2(peakLocation.Key, peakLocation.Value),
                            new Vector2(x, y))/ maxDistance;
                        float  addedHeight = 0;
                        if(voronoiType == VoronoiType.Combined)
                            addedHeight = peakHeight - distanceToPeak * randomPeakFallOff - Mathf.Pow(distanceToPeak, randomPeakDropOff);
                        if (voronoiType == VoronoiType.Power)
                            addedHeight = peakHeight - Mathf.Pow(distanceToPeak, randomPeakDropOff) * randomPeakFallOff;
                        if (voronoiType == VoronoiType.Linear)
                            addedHeight = peakHeight - distanceToPeak * randomPeakFallOff;
                        if (voronoiType == VoronoiType.SinPow)
                            addedHeight = peakHeight - Mathf.Pow(distanceToPeak * 3, randomPeakFallOff) - 
                                          Mathf.Sin(distanceToPeak * 2 * Mathf.PI) / randomPeakDropOff;
                        
                        if (addedHeight > heightMap[x, y]) heightMap[x, y] = addedHeight;
                    }
                }
            }
            terrainData.SetHeights(0, 0, heightMap);
        }
        public void AddNewPerlin()
        {
            perlinParameters.Add(new PerlinParameter());
        }

        public void RemovePerlin()
        {
            for (var i = perlinParameters.Count - 1; i > -1; i--)
            {
                if(perlinParameters[i].remove)
                    perlinParameters.RemoveAt(i);
            }
            if (perlinParameters.Count == 0) perlinParameters.Add(new PerlinParameter());
        }

        public void MultiplePerlinTerrain()
        {
            var heightMap = GetHeightMap();
            m_HeightMapBackup = GetHeightMap(true);
            for (var i = 0; i < terrainData.heightmapResolution; i++)
            {
                for (var j = 0; j < terrainData.heightmapResolution; j++)
                {
                    foreach (var p in perlinParameters)
                    {
                        heightMap[i, j] += Tools.FractalBrowningMotion((i + p.perlinOffsetX) * p.perlinXScale, 
                            (j + p.perlinOffsetY) * p.perlinYScale, p.perlinOctaves, p.perlinPersistence) * p.perlinHeightScale;    
                    }
                }
            }
            terrainData.SetHeights(0, 0, heightMap);
        }

        public void PerLin()
        {
            var heightMap = GetHeightMap();
            m_HeightMapBackup = GetHeightMap(true);
            for (var i = 0; i < terrainData.heightmapResolution; i++)
            {
                for (var j = 0; j < terrainData.heightmapResolution; j++)
                {
                    heightMap[i, j] += Tools.FractalBrowningMotion((i + perlinOffsetX) * perlinXScale, 
                        (j + perlinOffsetY) * perlinYScale, perlinOctaves, perlinPersistence) * perlinHeightScale;
                }
            }
            terrainData.SetHeights(0, 0, heightMap);
        }

        public void LoadTexture()
        {
            var heightMap = GetHeightMap();
            m_HeightMapBackup = GetHeightMap(true);
            for (var i = 0; i < terrainData.heightmapResolution; i++)
            {
                for (var j = 0; j < terrainData.heightmapResolution; j++)
                {
                    heightMap[i, j] += heightMapImage.GetPixel((int)(i * heightMapScale.x),
                        (int)(j * heightMapScale.y)).grayscale * heightMapScale.y;
                }
            }

            terrainData.SetHeights(0, 0, heightMap);
        }

        public void RandomTerrain()
        {
            var heightMap = GetHeightMap();
            m_HeightMapBackup = GetHeightMap(true);
            for (int i = 0; i < terrainData.heightmapResolution; i++)
            {
                for (int j = 0; j < terrainData.heightmapResolution; j++)
                {
                    heightMap[i, j] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
                }
            }
            terrainData.SetHeights(0, 0, heightMap);
        }
        public void ResetHeightMap()
        {
            m_HeightMapBackup = GetHeightMap(true);
            var heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution,
                terrainData.heightmapResolution);
            for (var i = 0; i < terrainData.heightmapResolution; i++)
            {
                for (var j = 0; j < terrainData.heightmapResolution; j++)
                {
                    heightMap[i, j] = 0;
                }
            }
            terrainData.SetHeights(0, 0, heightMap);
        }

        private float[,] GetHeightMap(bool loadMap = false)
        {
            terrainData = terrain.terrainData;
            if (resetTerrain && !loadMap)
            {
                return new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
            }
            else
            {
                return terrainData.GetHeights(0, 0, terrainData.heightmapResolution,
                    terrainData.heightmapResolution);
            }
        }
        private void AddTag(SerializedProperty tagsProp, string newTag)
        {
            var found = false;

            foreach (SerializedProperty it in tagsProp)
            {
                if (it.stringValue.Equals(newTag))
                {
                    found = true;
                    break;
                }
            }
            if (found) return;
            tagsProp.InsertArrayElementAtIndex(0);
            var newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = newTag;
        }

        private int AddLayer(SerializedProperty tagProperty, string newLayer)
        {
            var foundFree = -1;
            for (var i = 0; i < tagProperty.arraySize; i++)
            {
                if (tagProperty.GetArrayElementAtIndex(i).stringValue == "" && foundFree < 0)
                    foundFree = i;
                if (tagProperty.GetArrayElementAtIndex(i).stringValue.Equals(newLayer))
                {
                    return i;
                }
            }
            if(foundFree != 0)
                tagProperty.InsertArrayElementAtIndex(foundFree);
            return foundFree;
        }
    }
}
