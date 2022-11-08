using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace {

    private ShapeGenerator shapeGenerator;
    private Mesh mesh;
    private int resolution;
    private Vector3 localUp;
    private Vector3 axisA;
    private Vector3 axisB;

    public TerrainFace(ShapeGenerator _shapeGenerator, Mesh _mesh, int _res, Vector3 _localUp) {

        shapeGenerator = _shapeGenerator;
        mesh = _mesh;
        resolution = _res;
        localUp = _localUp;

        axisA = new Vector3(_localUp.y, _localUp.z, _localUp.x);
        axisB = Vector3.Cross(_localUp, axisA);

    }

    public void ConstructMesh() {

        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        int i = 0;
        int triIndex = 0;

        for(int y = 0; y < resolution; y++) {
            for(int x = 0; x < resolution; x++) {
                
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + ((percent.x - 0.5f) * 2 * axisA) + ((percent.y - 0.5f) * 2 * axisB);
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                if(x != resolution - 1 && y != resolution - 1) {

                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;

                    triIndex += 6;

                }

                i++;

            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
    }

}
