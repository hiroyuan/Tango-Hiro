using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour {
    /// <summary>
    /// Combines all meshes for room game objects.
    /// Referenced from Unity Scripting API.
    /// https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
    /// </summary>
    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combines = new CombineInstance[meshFilters.Length];
        Mesh result = new Mesh();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combines[i].mesh = meshFilters[i].sharedMesh;
            combines[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        //result.CombineMeshes(combines);
        //GetComponent<MeshFilter>().sharedMesh = result;
        gameObject.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        gameObject.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combines);
        gameObject.GetComponent<MeshFilter>().sharedMesh = gameObject.transform.GetComponent<MeshFilter>().mesh;
    }
}
