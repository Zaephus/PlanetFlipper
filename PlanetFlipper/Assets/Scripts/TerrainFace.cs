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

        int triIndex = 0;

        Vector2[] uv = (mesh.uv.Length == vertices.Length) ? mesh.uv : new Vector2[vertices.Length];

        for(int y = 0; y < resolution; y++) {
            for(int x = 0; x < resolution; x++) {

                int i = x + y * resolution;
                
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + ((percent.x - 0.5f) * 2 * axisA) + ((percent.y - 0.5f) * 2 * axisB);
                Vector3 pointOnUnitSphere = PointOnCubeToPointOnSphere(pointOnUnitCube);
                float unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);
                vertices[i] = pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);
                uv[i].y = unscaledElevation;

                if(x != resolution - 1 && y != resolution - 1) {

                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;

                    triIndex += 6;

                }

            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
        
    }

    public void UpdateUVs(ColourGenerator _colourGenerator) {

        Vector2[] uv = mesh.uv;

        for(int y = 0; y < resolution; y++) {
            for(int x = 0; x < resolution; x++) {

                int i = x + y * resolution;
                
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + ((percent.x - 0.5f) * 2 * axisA) + ((percent.y - 0.5f) * 2 * axisB);
                Vector3 pointOnUnitSphere = PointOnCubeToPointOnSphere(pointOnUnitCube);

                uv[i].x = _colourGenerator.BiomePercentFromPoint(pointOnUnitSphere);

            }
        }

        mesh.uv = uv;

    }

    private Vector3 PointOnCubeToPointOnSphere(Vector3 _point) {

        float x2 = _point.x * _point.x;
        float y2 = _point.y * _point.y;
        float z2 = _point.z * _point.z;

        float x = _point.x * Mathf.Sqrt(1 - (y2 + z2) / 2 + (y2 * z2) / 3);
        float y = _point.y * Mathf.Sqrt(1 - (z2 + x2) / 2 + (z2 * x2) / 3);
        float z = _point.z * Mathf.Sqrt(1 - (x2 + y2) / 2 + (x2 * y2) / 3);

        return new Vector3(x, y, z);

    }

}
