using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHolder {
    private Vector3[] vertices;
    private Vector3[] normals;
    private Color32[] colors;
    private int[] triangleIndices;
    private Vector2[] uvs;
    private Vector2[] uv2s;

    private Transform trans;

    public MeshHolder(GameObject g)
    {
        vertices = g.GetComponent<MeshFilter>().mesh.vertices;
        normals = g.GetComponent<MeshFilter>().mesh.normals;
        colors = g.GetComponent<MeshFilter>().mesh.colors32;
        triangleIndices = g.GetComponent<MeshFilter>().mesh.triangles;
        uvs = g.GetComponent<MeshFilter>().mesh.uv;
        uv2s = g.GetComponent<MeshFilter>().mesh.uv2;
        trans = g.transform;
    }

    public Vector3[] GetVertices()
    {
        return transformToWorldPoint(vertices);
    }

    public Vector3[] GetNormals()
    {
        return transformToWorldPoint(normals);
    }

    public Color32[] GetColors()
    {
        return colors;
    }

    public int[] GetTriangleIndices()
    {
        return triangleIndices;
    }

    public Vector2[] GetUVs()
    {
        return uvs;
    }

    public Vector2[] GetUV2s()
    {
        return uv2s;
    }

    private Vector3[] transformToWorldPoint(Vector3[] array)
    {
        Vector3[] worldPosArray = new Vector3[array.Length];
        for (int index = 0; index < array.Length; index++)
        {
            worldPosArray[index] = trans.TransformPoint(array[index]);
        }

        return worldPosArray;
    }
}
