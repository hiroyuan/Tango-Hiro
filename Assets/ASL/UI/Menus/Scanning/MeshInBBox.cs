using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInBBox {
    public List<Vector3> partialVertices;
    public List<Vector3> partialNormals;
    public List<Color> partialColor;
    public List<int> newTriangles;
    public int meshID;
    private VertexLookUp map;

    public MeshInBBox(int id)
    {
        meshID = id;
        partialVertices = new List<Vector3>();
        partialNormals = new List<Vector3>();
        partialColor = new List<Color>();
        newTriangles = new List<int>();
        map = new VertexLookUp();
    }

    public void AddArrays(int[] triangle, Vector3[] vs, Vector3[] ns, Color[] cs, int[] ts)
    {
        for (int index = 0; index < triangle.Length; index++)
        {
            int vertexIndex = triangle[index];
            if (!map.IsOldIndexInLookUp(vertexIndex))
            {
                // vertexIndex is new to this BoundHolder
                map.AddOldIndex(vertexIndex);
                partialVertices.Add(vs[vertexIndex]);
                partialNormals.Add(ns[vertexIndex]);
                if (cs.Length != 0)
                    partialColor.Add(cs[vertexIndex]);
            }
            int newIndex = map.FindValue(triangle[index]);
            newTriangles.Add(newIndex);
        }
    }
}
