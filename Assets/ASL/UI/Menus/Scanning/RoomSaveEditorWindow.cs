using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UWBNetworkingPackage;


namespace ASL.UI.Menus.Scanning
{
	#if UNITY_EDITOR
    public class RoomSaveEditorWindow : EditorWindow
    {
        public ASL.Scanning.Tango.RoomSaveLogic roomSaveLogic_m;

        private int selected = 0;

        #region Methods
        /// <summary>
        /// Creates the UI for the Room Manager Window
        /// </summary>
        void OnGUI()
        {
            string RoomName = roomSaveLogic_m.RoomName;
            var roomList = roomSaveLogic_m.RoomList;
            var roomNameList = roomSaveLogic_m.RoomNameList;
            var directoryInfoList = roomSaveLogic_m.DirectoryInfoList;
            var ms = roomSaveLogic_m.SplitterController;

            // Create a label for save section
            GUI.Label(new Rect(10, 5, position.width - 20, 20), "Directory to Save Rooms to: ");
            // Create a text entry for Directory creation/saving
            RoomName = EditorGUI.TextField(new Rect(10, 25, position.width - 20, 20),
                    "Room Name: ",
                    RoomName);
            roomSaveLogic_m.RoomName = RoomName;
            // Create a save Rooms button and link it to the function
            if (GUI.Button(new Rect(10, 50, position.width - 20, 20), "Save Rooms"))
                roomSaveLogic_m.SaveRooms(roomList, directoryInfoList);

            if (GUI.Button(new Rect(10, 75, position.width - 20, 20), "Save split mesh with BBox"))
                roomSaveLogic_m.SaveBoundingBox();

            // If there are directories within the root directory
            if (directoryInfoList.Count > 0)
            {
                // Create the load lable
                // GUI.Label(new Rect(10, 85, position.width - 20, 20), "Directory to Load Rooms From: ");
                GUI.Label(new Rect(10, 95, position.width - 20, 20), "Room: ");
                
                // Get the currently selected item in the drop down
                //selected = EditorGUI.Popup(new Rect(10, 110, position.width - 20, 20), selected, DirNames.ToArray());
                selected = EditorGUI.Popup(new Rect(10, 110, position.width - 20, 20), selected, roomNameList.ToArray());
				roomSaveLogic_m.selected = selected;
                // Create load button
                if (GUI.Button(new Rect(10, 135, position.width - 20, 20), "Load Selected Room"))
                    roomSaveLogic_m.LoadRoom(directoryInfoList[selected]);

                // Create unload button
                if (GUI.Button(new Rect(10, 160, position.width - 20, 20), "Unload Selected Room"))
                    roomSaveLogic_m.UnloadRoom(directoryInfoList[selected]);

                if (GUI.Button(new Rect(10, 185, position.width - 20, 20), "Split by BBox"))
                {
                    ms.SplitMeshByBBox();
                }
                if (GUI.Button(new Rect(10, 210, position.width - 20, 20), "Load BBox"))
                {
                    ms.Load();
                }
            }

            // Create a delete all button
            if (GUI.Button(new Rect(10, 260, position.width - 20, 20), "Delete All Rooms"))
                roomSaveLogic_m.DeleteAllRooms();

            Repaint();
        }

        #region Helper Methods
        public ASL.Scanning.Tango.RoomSaveLogic SetRoomSaveLogicScript()
        {
            var rsl = GameObject.FindObjectOfType<ASL.Scanning.Tango.RoomSaveLogic>();
            roomSaveLogic_m = rsl;
            return rsl;
        }

        /// <summary>
        /// Toolbar Declaration and window creation
        /// </summary>
        [MenuItem("ASL/Room Scanning/Room Manager Window")]
        private static void showEditor()
        {
            GetWindow<RoomSaveEditorWindow>(false, "Room Manager");
        }

        #endregion
        #endregion
    }
	#endif
}