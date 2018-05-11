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
//    public Transform trans;
    public LoadedMeshHolder[] loadedMeshes;

    public int x_area = 1;
    public int y_area = 1;
    public int z_area = 1;
    public int x_mesh = 2;
    public int y_mesh = 1;
    public int z_mesh = 1;

    private RoomSaveLogic rs;

    // Use this for initialization
    void Start ()
    {
        //rs = (RoomSaveLogic)EditorWindow.GetWindow(typeof(RoomSaveLogic));
        getRoomSaveLogic();

        specificMeshes = new List<GameObject>();
        indicator = GameObject.Find("IndicatorObject");

        areaBound = new BoundingBoxSystem(rs.GetList(), x_area, y_area, z_area);
        areaBound.SplitBounds(false);
    }

    private void getRoomSaveLogic()
    {
        rs = GameObject.FindObjectOfType<RoomSaveLogic>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (rs.GetList() != null)
        {
            areaBound = new BoundingBoxSystem(rs.GetList(), x_area, y_area, z_area);
            areaBound.SplitBounds(false);
        }

        //areaBound.SetSplitter(rs.GetList(), x_area, y_area, z_area);
        //areaBound.SplitBounds();

        //if (loadedBound != null)
        //{
        //    loadedBound = new BoundingBoxSystem(loadedBBox, x_area, y_area, z_area);
        //    loadedBound.SplitBounds();
        //}

        if (loadedBounds != null)
        {
            loadedBounds.ActivateSubBoundsByIndicator(indicator);
            for (int i = 0; i < loadedBounds.GetSubBounds().Length; i++)
            {
                if (loadedBounds.GetSubBounds()[i].GetStatus())
                {
                    if (GameObject.Find("NewMesh" + i) == null)
                    {
                        if (loadedMeshes[i] == null)
                            LoadMesh(i);

                        GameObject obj = new GameObject();
                        MeshFilter mf = obj.AddComponent<MeshFilter>();
                        mf.name = "NewMesh" + i;
                        mf.mesh = loadedMeshes[i].mesh;
                        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                        mr.material = new Material(Shader.Find("Custom/UnlitVertexColor"));
                    }
                }
                else
                {
                    if (GameObject.Find("NewMesh" + i) != null)
                    {
                        GameObject obj = GameObject.Find("NewMesh" + i);
                        Destroy(obj);
                    }
                }
            }
        }

    }

    void OnDrawGizmos()
    {
        if (areaBound != null)
        {
            for (int i = 0; i < areaBound.GetSubBounds().Length; i++)
            {
                Gizmos.DrawWireCube(areaBound.GetSubBounds()[i].subBound.center, areaBound.GetSubBounds()[i].subBound.size);
            }
        }

        if (meshBounds != null)
        {
            for (int boundsIndex = 0; boundsIndex < meshBounds.Count; boundsIndex++)
            {
                for (int i = 0; i < meshBounds[boundsIndex].GetSubBounds().Length; i++)
                {
                    Gizmos.DrawWireCube(meshBounds[boundsIndex].GetSubBounds()[i].subBound.center, meshBounds[boundsIndex].GetSubBounds()[i].subBound.size);
                }
            }
            
        }

        if (loadedBounds != null)
        {
            //Debug.Log("Loaded bound is not null");
            //Debug.Log("length of subBounds: " + loadedBound.GetSubBounds().Length);
            //Gizmos.DrawWireCube(loadedBound.GetBound().center, loadedBound.GetBound().size);
            //for (int boundsIndex = 0; boundsIndex < loadedBounds.Count; boundsIndex++)
            //{
            //for (int i = 0; i < loadedBounds/*[boundsIndex]*/.GetSubBounds().Length; i++)
            //{
                Gizmos.DrawWireCube(loadedBounds/*[boundsIndex]*/.GetSubBounds()[0].subBound.center, loadedBounds/*[boundsIndex]*/.GetSubBounds()[0].subBound.size);
            //}
            //}
        }
    }

    public void Test()
    {
        meshBounds = new List<BoundingBoxSystem>();
        int count = 0;
        foreach (GameObject room in rs.GetList())
        {
            //if (room.GetComponent<MeshFilter>().mesh.vertexCount == 0)
                //continue;
            specificMeshes.Add(room);
            meshBounds.Add(new BoundingBoxSystem(specificMeshes, x_mesh, y_mesh, z_mesh, room.transform, room.GetComponent<MeshFilter>().mesh));
            meshBounds[count].SplitBounds(true);
            meshBounds[count].SplitMesh();
            specificMeshes.Clear();
            count++;
        }
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
