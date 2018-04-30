using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundHolder {
    public Bounds subBound;
    private List<Vector3> partialVertices;
    private List<Vector3> partialNormals;
    private List<Color> partialColoar;
    private List<Vector2> partialUVs;
    private List<int> partialTriangles;
    private List<int> newTriangles;
    private VertexLookUp map;
    private bool isActive;
    //private List<int> checkDuplicates;

    public BoundHolder(Vector3 center, Vector3 size)
    {
        subBound = new Bounds(center, size);
        partialVertices = new List<Vector3>();
        partialNormals = new List<Vector3>();
        partialColoar = new List<Color>();
        partialUVs = new List<Vector2>();
        partialTriangles = new List<int>();
        newTriangles = new List<int>();
        map = new VertexLookUp();
        isActive = false;
        //checkDuplicates = new List<int>();
    }

    public bool GetStatus()
    {
        return isActive;
    }

    public void SetStatus(bool status)
    {
        isActive = status;
    }

    public bool CheckIntersects(Vector3 point)
    {
        if (subBound.Contains(point))
        {
            return true;
        }
        return false;
    }

    public void AddTriangle(int[] triangle, Mesh m)
    {
        for (int index = 0; index < triangle.Length; index++)
        {
            int vertexIndex = triangle[index];
            if (!map.IsOldIndexInLookUp(vertexIndex))
            {
                // vertexIndex is new to this BoundHolder
                map.AddOldIndex(vertexIndex);
                //checkDuplicates.Add(vertexIndex);
                partialVertices.Add(m.vertices[vertexIndex]);
                partialNormals.Add(m.normals[vertexIndex]);
                if (m.colors.Length != 0)
                    partialColoar.Add(m.colors[vertexIndex]);
                if (m.uv.Length != 0)
                    partialUVs.Add(m.uv[vertexIndex]);
            }
            int newIndex = map.FindValue(triangle[index]);
            newTriangles.Add(newIndex);
        }
    }

    public List<Vector3> GetVertices()
    {
        return partialVertices;
    }

    public List<int> GetTriangles()
    {
        return newTriangles;
    }

    public List<Vector3> GetNormals()
    {
        return partialNormals;
    }

    public List<Color> GetColors()
    {
        return partialColoar;
    }

    public List<Vector2> GetUVs()
    {
        return partialUVs;
    }
}
