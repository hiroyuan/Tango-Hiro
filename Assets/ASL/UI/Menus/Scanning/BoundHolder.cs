using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BoundHolder {
    public Bounds subBound;
    private List<Vector3> partialVertices;
    private List<Vector3> partialNormals;
    private List<Color> partialColoar;
    private List<Vector2> partialUVs;
    private List<int> newTriangles;
    private VertexLookUp map;
    private bool isActive;
    public Mesh mesh;

    public BoundHolder()
    {
        isActive = false;
    }

    public BoundHolder(Vector3 center, Vector3 size)
    {
        subBound = new Bounds(center, size);
        partialVertices = new List<Vector3>();
        partialNormals = new List<Vector3>();
        partialColoar = new List<Color>();
        partialUVs = new List<Vector2>();
        newTriangles = new List<int>();
        map = new VertexLookUp();
        isActive = false;
        mesh = new Mesh();
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

    public Mesh SetThenGetMesh()
    {
        mesh.vertices = partialVertices.ToArray();
        mesh.normals = partialNormals.ToArray();
        mesh.colors = partialColoar.ToArray();
        mesh.uv = partialUVs.ToArray();
        mesh.triangles = newTriangles.ToArray();

        return mesh;
    }

    public byte[] Serialize()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                writeBounds(bw, subBound);
                writeMesh(bw, mesh);
            }

            return ms.ToArray();
        }
    }

    public BoundHolder Desirialize(byte[] data)
    {
        BoundHolder result = new BoundHolder();
        using (MemoryStream ms = new MemoryStream(data))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                result.subBound = readBounds(br);
                result.mesh = readMesh(br);
            }
            return result;
        }
    }

    public BoundHolder DesirializeBounds(byte[] data)
    {
        BoundHolder result = new BoundHolder();
        using (MemoryStream ms = new MemoryStream(data))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                result.subBound = readBounds(br);
            }
            return result;
        }
    }

    private void writeBounds(BinaryWriter bw, Bounds b)
    {
        bw.Write(b.center.x);
        bw.Write(b.center.y);
        bw.Write(b.center.z);
        bw.Write(b.size.x);
        bw.Write(b.size.y);
        bw.Write(b.size.z);
    }

    private Bounds readBounds(BinaryReader br)
    {
        Bounds result = new Bounds();

        Vector3 center = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        Vector3 size = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

        result.center = center;
        result.size = size;

        return result;
    }

    private void writeMesh(BinaryWriter bw, Mesh m)
    {
        // write the length of arrays first so that I can generate proper array length when I read back
        bw.Write(m.vertices.Length);
        bw.Write(m.normals.Length);
        bw.Write(m.colors.Length);
        bw.Write(m.triangles.Length);

        int index = 0;
        foreach (Vector3 vertex in m.vertices)
        { 
            bw.Write(m.vertices[index].x);
            bw.Write(m.vertices[index].y);
            bw.Write(m.vertices[index].z);
            bw.Write(m.normals[index].x);
            bw.Write(m.normals[index].y);
            bw.Write(m.normals[index].z);
            bw.Write(m.colors[index].r);
            bw.Write(m.colors[index].g);
            bw.Write(m.colors[index].b);
            bw.Write(m.colors[index].a);
            index++;
        }

        index = 0;
        foreach (int triangleIndex in m.triangles)
        {
            bw.Write(m.triangles[index]);
            index++;
        }
    }

    private Mesh readMesh(BinaryReader br)
    {
        Mesh result = new Mesh();

        Vector3[] vertices = new Vector3[br.ReadInt32()];
        Vector3[] normals = new Vector3[br.ReadInt32()];
        Color[] colors = new Color[br.ReadInt32()];
        int[] triangles = new int[br.ReadInt32()];

        for (int index = 0; index < vertices.Length; index++)
        {
            vertices[index] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            normals[index] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            colors[index] = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }
        for(int index =0; index < triangles.Length; index++)
        {
            triangles[index] = br.ReadInt32();
        }

        result.vertices = vertices;
        result.normals = normals;
        result.colors = colors;
        result.triangles = triangles;

        return result;
    }
}
