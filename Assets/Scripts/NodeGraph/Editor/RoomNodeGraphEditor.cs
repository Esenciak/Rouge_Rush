using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{   // menu z dokumentacji unity
    [MenuItem("Room Node Graph Editor",menuItem = "Windows/Dungeon Editor/Room Node Graph Editor")]
    // otwieranie edytora
    private static void OpenWindow()
    {
        // przez dziedziczenei nie muszê pisaæ prefixów
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

}
