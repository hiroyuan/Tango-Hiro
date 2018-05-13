using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexLookUp {
    private Dictionary<int, int> dictionary;
    private int newIndex;

	public VertexLookUp(Mesh m, int amtOfSubBounds)
    {
        dictionary = new Dictionary<int, int>(2 * (m.vertices.Length / amtOfSubBounds));
        //dictionary = new Dictionary<int, int>();
        newIndex = 0;
    }

    public VertexLookUp(Mesh m)
    {
        dictionary = new Dictionary<int, int>(m.vertexCount);
        newIndex = 0;
    }

    public VertexLookUp()
    {
        dictionary = new Dictionary<int, int>();
        newIndex = 0;
    }

    public bool IsOldIndexInLookUp(int oldVertex)
    {
        return dictionary.ContainsKey(oldVertex);
    }

    public void AddOldIndex(int oldIndex)
    {
        dictionary.Add(oldIndex, getNewIndex());
    }

    public int FindValue(int index)
    {
        return dictionary[index];
    }

    private int getNewIndex()
    {
        int retVal = newIndex;
        newIndex++;
        return retVal;
    }
}
