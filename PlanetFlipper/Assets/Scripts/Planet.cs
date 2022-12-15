using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

     [Range(2, 256)]
    public int targetVertexResolution = 10;
     [Range(1, 5)]
    public int meshResolution = 2;

    public bool autoUpdate = true;

    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

     [HideInInspector]
    public bool shapeSettingsFoldout;
     [HideInInspector]
    public bool colourSettingsFoldout;

    private int vertexResolution;

    private ShapeGenerator shapeGenerator = new ShapeGenerator();
    private ColourGenerator colourGenerator = new ColourGenerator();

     [SerializeField, HideInInspector]
    private MeshFilter[] meshFilters;
    private TerrainFace[] terrainFaces;

    private void Initialize() {

        vertexResolution = Mathf.Clamp(targetVertexResolution / meshResolution, 2, 256);

        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        if(meshFilters == null || meshFilters.Length == 0) {
            meshFilters = new MeshFilter[6 * meshResolution * meshResolution];
        }

        terrainFaces = new TerrainFace[6 * meshResolution * meshResolution];

        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        int j = 0;

        for(int i = 0; i < 6; i++) {
            for(int x = 0; x < meshResolution; x++) {
                for(int y = 0; y < meshResolution; y++) {

                    if(meshFilters[j] == null) {

                        GameObject meshObj = new GameObject("mesh" + directions[i]);
                        meshObj.transform.parent = transform;

                        meshObj.AddComponent<MeshRenderer>();
                        meshFilters[j] = meshObj.AddComponent<MeshFilter>();
                        meshFilters[j].sharedMesh = new Mesh();

                    }

                    meshFilters[j].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

                    terrainFaces[j] = new TerrainFace(shapeGenerator, meshFilters[j].sharedMesh, vertexResolution, meshResolution,
                                                      directions[i], x * (vertexResolution - 1), y * (vertexResolution - 1));

                    bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask -1 == i;
                    meshFilters[j].gameObject.SetActive(renderFace);

                    j++;

                }
            }
        }
        
    }

    public void GeneratePlanet() {
        if(meshFilters != null) {
            ResetMeshes();
        }
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void ResetMeshes() {
        for(int i = 0; i < meshFilters.Length; i++) {
            if(Application.isEditor) {
                DestroyImmediate(meshFilters[i].gameObject);
            }
            else {
                Destroy(meshFilters[i].gameObject);
            }
        }
        meshFilters = new MeshFilter[0];
        meshFilters = null;
    }

    public void OnShapeSettingsUpdated() {
        if(autoUpdate) {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated() {
        if(autoUpdate) {
            Initialize();
            GenerateColours();
        }
    }

    private void GenerateMesh() {

        for(int i = 0; i < meshFilters.Length; i++) {
            if(meshFilters[i].gameObject.activeSelf) {
                terrainFaces[i].ConstructMesh();
            }
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);

    }

    private void GenerateColours() {

        colourGenerator.UpdateColours();

        for(int i = 0; i < meshFilters.Length; i++) {
            if(meshFilters[i].gameObject.activeSelf) {
                terrainFaces[i].UpdateUVs(colourGenerator);
            }
        }

    }

}
