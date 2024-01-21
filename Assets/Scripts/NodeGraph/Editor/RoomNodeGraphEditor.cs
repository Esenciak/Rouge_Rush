using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle; // tworze styl gui
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // menu z dokumentacji unity
    [MenuItem("Room Node Graph Editor",menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    // otwieranie edytora
    private static void OpenWindow()
    {
        // przez dziedziczenei nie muszê pisaæ prefixów
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

	private void OnEnable()
	{
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
	}


	private void OnGUI()
	{
        //Debug.Log("wywo³ano OnGui"); sprawdzam czy jak mam uruchomione to czy dzia³a
        //1 nood box
        GUILayout.BeginArea(new Rect(new Vector2(100f,100f),new Vector2(nodeWidth,nodeHeight)),roomNodeStyle);
        EditorGUILayout.LabelField("Node 1");
        GUILayout.EndArea();
        // 2 nood box
		GUILayout.BeginArea(new Rect(new Vector2(300f, 300f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);
		EditorGUILayout.LabelField("Node 1");
		GUILayout.EndArea();

	}

}
