using ASL.UI.Menus.Scanning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using ASL.Scanning.Tango;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshSplitterController : MonoBehaviour {
    public BoundingBoxSystem areaBound;
    public List<BoundingBoxSystem> meshBounds;
    public BoundingBoxSystem loadedBounds;
    public List<GameObject> specificMeshes;
    public GameObject indicator;
    public LoadedMeshHolder[] loadedMeshes;

    public int x_area = 1;
    public int y_area = 1;
    public int z_area = 1;
    public bool keepMeshes = false;
    public bool preLoadVer = false;

    private bool splitTriggered = false;

    private RoomSaveLogic rs;

    // Use this for initialization
    void Start ()
    {
        getRoomSaveLogic();

        specificMeshes = new List<GameObject>();
        indicator = GameObject.Find("IndicatorObject");

        areaBound = new BoundingBoxSystem(rs.GetList(), x_area, y_area, z_area);
        areaBound.SplitBounds();
    }

    private void getRoomSaveLogic()
    {
        rs = GameObject.FindObjectOfType<RoomSaveLogic>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!splitTriggered)
        {
            areaBound = new BoundingBoxSystem(rs.GetList(), x_area, y_area, z_area);
            areaBound.SplitBounds();
        }

        if (loadedBounds != null && !preLoadVer)
        {
            loadedBounds.ActivateSubBoundsByIndicator(indicator);
            for (int i = 0; i < loadedBounds.GetSubBounds().Length; i++)
            {
                if (loadedBounds.GetSubBounds()[i].GetStatus())
                {
                    if (GameObject.Find("MeshInBoundingBox_" + i) == null)
                    {
                        if (loadedMeshes[i] == null)
                            LoadMesh(i);

                        for (int j = 0; j < loadedMeshes[i].meshes.Length; j++)
                        {
                            GameObject obj = new GameObject();
                            MeshFilter mf = obj.AddComponent<MeshFilter>();
                            mf.name = "MeshInBoundingBox_" + i;
                            mf.mesh = loadedMeshes[i].meshes[j];
                            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                            mr.material = new Material(Shader.Find("Custom/UnlitVertexColor"));
                        }
                    }
                }
                else
                {
                    if (!keepMeshes && (GameObject.Find("MeshInBoundingBox_" + i) != null))
                    {
                        GameObject obj = GameObject.Find("MeshInBoundingBox_" + i);
                        Destroy(obj);
                    }
                }
            }
        }

        if (loadedBounds != null && preLoadVer)
        {
            loadedBounds.ActivateSubBoundsByIndicator(indicator);
            for (int i = 0; i < loadedBounds.GetSubBounds().Length; i++)
            {
                Vector3 directionIndicator2Bound = (loadedBounds.GetSubBounds()[i].subBound.center - indicator.transform.position).normalized;
                float dotProduct = Vector3.Dot(directionIndicator2Bound, indicator.transform.forward);

                if (dotProduct > 0.95)
                    loadedBounds.GetSubBounds()[i].SetPreLoadStatus(true);
                else
                    loadedBounds.GetSubBounds()[i].SetPreLoadStatus(false);

                if (loadedBounds.GetSubBounds()[i].GetStatus() || loadedBounds.GetSubBounds()[i].GetPreLoadStatus())
                {
                    if (GameObject.Find("MeshInBoundingBox_" + i) == null)
                    {
                        if (loadedMeshes[i] == null)
                            LoadMesh(i);

                        for (int j = 0; j < loadedMeshes[i].meshes.Length; j++)
                        {
                            GameObject obj = new GameObject();
                            MeshFilter mf = obj.AddComponent<MeshFilter>();
                            mf.name = "MeshInBoundingBox_" + i;
                            mf.mesh = loadedMeshes[i].meshes[j];
                            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                            mr.material = new Material(Shader.Find("Custom/UnlitVertexColor"));
                        }
                    }
                }
                else
                {
                    if (!keepMeshes && (GameObject.Find("MeshInBoundingBox_" + i) != null || !loadedBounds.GetSubBounds()[i].GetPreLoadStatus()))
                    {
                        GameObject obj = GameObject.Find("MeshInBoundingBox_" + i);
                        Destroy(obj);
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (areaBound != null && loadedBounds == null)
        {
            for (int i = 0; i < areaBound.GetSubBounds().Length; i++)
            {
                Gizmos.DrawWireCube(areaBound.GetSubBounds()[i].subBound.center, areaBound.GetSubBounds()[i].subBound.size);
            }
        }

        if (loadedBounds != null)
        {
            for (int i = 0; i < loadedBounds.GetSubBounds().Length; i++)
            {
                Gizmos.DrawWireCube(loadedBounds.GetSubBounds()[i].subBound.center, loadedBounds.GetSubBounds()[i].subBound.size);
            }
        }
    }

    public void SplitMeshByBBox()
    {
        splitTriggered = true;
        areaBound = new BoundingBoxSystem(rs.GetList(), x_area, y_area, z_area);
        areaBound.SplitBounds();
        areaBound.SplitMesh();
    }

    public void Load()
    {
        List<DirectoryInfo> directoryInfoList = rs.GetDirectoryList();
        int selected = rs.GetSelectedRoom();
        BoundHolder[] bhs = rs.LoadBoundingBox(directoryInfoList[selected]);
        //Debug.Log("length of bhs: " + bhs.Length);
        loadedBounds = new BoundingBoxSystem(bhs);
        loadedMeshes = new LoadedMeshHolder[bhs.Length];
    }

    public void LoadMesh(int index)
    {
        List<DirectoryInfo> directoryInfoList = rs.GetDirectoryList();
        int selected = rs.GetSelectedRoom();
        BoundHolder bh = rs.LoadMeshes(directoryInfoList[selected], index);
        loadedMeshes[index] = new LoadedMeshHolder(bh);
    }
}
