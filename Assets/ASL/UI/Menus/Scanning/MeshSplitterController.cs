using ASL.UI.Menus.Scanning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshSplitterController : MonoBehaviour {
    public BoundingBoxSystem areaBound;
    public BoundingBoxSystem meshBound;
    public BoundingBoxSystem loadedBound;
    public List<GameObject> specificMeshes;
    public GameObject indicator;
    public Transform trans;

    public int x_area = 1;
    public int y_area = 1;
    public int z_area = 1;
    public int x_mesh = 2;
    public int y_mesh = 1;
    public int z_mesh = 1;

    Bounds loadedBBox;

    //private RoomSave roomSave = new RoomSave();
    private RoomSave rs;

    // Use this for initialization
    void Start ()
    {
        rs = (RoomSave)EditorWindow.GetWindow(typeof(RoomSave));
        specificMeshes = new List<GameObject>();
        indicator = GameObject.Find("IndicatorObject");

        //areaBound = new BoundingBoxSystem(rs.GetList(), x_area, y_area, z_area);
        //areaBound.SplitBounds();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (rs.GetList() != null)
        //{
        //    areaBound = new BoundingBoxSystem(rs.GetList(), x_area, y_area, z_area);
        //    areaBound.SplitBounds();
        //}

        //areaBound.SetSplitter(rs.GetList(), x_area, y_area, z_area);
        //areaBound.SplitBounds();

        //if (loadedBound != null)
        //{
        //    loadedBound = new BoundingBoxSystem(loadedBBox, x_area, y_area, z_area);
        //    loadedBound.SplitBounds();
        //}

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

        if (meshBound != null)
        {
            for (int i = 0; i < meshBound.GetSubBounds().Length; i++)
            {
                Gizmos.DrawWireCube(meshBound.GetSubBounds()[i].subBound.center, meshBound.GetSubBounds()[i].subBound.size);
            }
        }

        if (loadedBound != null)
        {
            //Debug.Log("Loaded bound is not null");
            //Debug.Log("length of subBounds: " + loadedBound.GetSubBounds().Length);
            //Gizmos.DrawWireCube(loadedBound.GetBound().center, loadedBound.GetBound().size);
            for (int i = 0; i < loadedBound.GetSubBounds().Length; i++)
            {
                Gizmos.DrawWireCube(loadedBound.GetSubBounds()[i].subBound.center, loadedBound.GetSubBounds()[i].subBound.size);
            }
        }
    }

    public void Test()
    {
        //foreach (GameObject room in rs.GetList())
        //{
        //    List<GameObject> r = new List<GameObject>();
        //    r.Add(room);
        //    meshBound = new BoundingBoxSystem(r, x_mesh, y_mesh, z_mesh, room.transform, room.GetComponent<Mesh>());
        //    Debug.Log(room);
        //    //meshBound.SplitMesh();
        //}

        GameObject roomMesh = GameObject.Find("3_12_4_2017_11_23_11_AM");
        specificMeshes.Add(roomMesh);
        meshBound = new BoundingBoxSystem(specificMeshes, x_mesh, y_mesh, z_mesh, roomMesh.transform, roomMesh.GetComponent<MeshFilter>().mesh);
        meshBound.SplitBounds();
        //meshBound.SplitMesh();
    }

    public void SplitMeshByBounds()
    {
        meshBound.SplitMesh();
    }

    public void Load()
    {
        List<DirectoryInfo> directoryInfoList = rs.GetDirectoryList();
        int selected = rs.GetSelectedRoom();
        BoundHolder[] bhs = rs.LoadBoundingBox(directoryInfoList[selected]);
        //Debug.Log("length of bhs: " + bhs.Length);
        loadedBound = new BoundingBoxSystem(bhs);
    }
}
