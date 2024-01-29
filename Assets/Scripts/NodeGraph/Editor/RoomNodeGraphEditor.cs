using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{

    private GUIStyle roomNodeStyle;
	private GUIStyle roomNodeSelectedStyle;
	private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeTypeListSO roomNodeTypeList;
	private RoomNodeSO currentRoomNode = null;


	// Node layout values
	private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // Connecting line values
	private const float connectingLineWidth = 3f;
	private const float connectingLineArrowSize = 6f;


	[MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]

    private void OnEnable()
    {
		Selection.selectionChanged += InspectorSelectionChanged;

		roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		// Define selected node style
		roomNodeSelectedStyle = new GUIStyle();
		roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
		roomNodeSelectedStyle.normal.textColor = Color.white;
		roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
		roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		//load room node type
		roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

	private void OnDisable()
	{
		// Unsubscribe from the inspector selection changed event
		Selection.selectionChanged -= InspectorSelectionChanged;
	}

	[OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }


    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }



    private void OnGUI()
    {
        if (currentRoomNodeGraph != null)
        {
			// Draw Grid
			//DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
			//DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);

			// Draw line if being dragged
			DrawDraggedLine();

			// Process Events
			ProcessEvents(Event.current);

			// Draw Connections Between Room Nodes
			DrawRoomConnections();

			// Draw Room Nodes
			DrawRoomNodes();
        }
        if (GUI.changed)
            Repaint();
    }

	private void DrawDraggedLine()
	{
		if (currentRoomNodeGraph.linePosition != Vector2.zero)
		{
			//Draw line from node to line position
			Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
		}
	}

	private void ProcessEvents(Event currentEvent)
    {
		if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
		{
			currentRoomNode = IsMouseOverRoomNode(currentEvent);
		}

		if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
		{
			ProcessRoomNodeGraphEvents(currentEvent);
		}
		else
		{
			// process room node events
			currentRoomNode.ProcessEvents(currentEvent);
		}
	}

	private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
	{
		for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
		{
			if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
			{
				return currentRoomNodeGraph.roomNodeList[i];
			}
		}

		return null;
	}

	private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
			// Process Mouse Up Events
			case EventType.MouseUp:
				ProcessMouseUpEvent(currentEvent);
				break;

			// Process Mouse Up Events
			case EventType.MouseDrag:
				ProcessMouseDragEvent(currentEvent);
				break;


			default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // Process right click mouse down on graph event (show context menu)
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
		else if (currentEvent.button == 0)
		{
			ClearLineDrag();
			ClearAllSelectedRoomNodes();
		}

	}

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
		menu.AddSeparator("");
		menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePositionObject)
    {
		// If current node graph empty then add entrance room node first
		if (currentRoomNodeGraph.roomNodeList.Count == 0)
		{
			CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
		}

		CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // set room node values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        // add room node to room node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

		// odœwierza graph
		currentRoomNodeGraph.OnValidate();
	}

	private void ClearAllSelectedRoomNodes()
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.isSelected)
			{
				roomNode.isSelected = false;

				GUI.changed = true;
			}
		}
	}
	private void SelectAllRoomNodes()
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			roomNode.isSelected = true;
		}
		GUI.changed = true;
	}

	private void ProcessMouseUpEvent(Event currentEvent)
	{
		// if releasing the right mouse button and currently dragging a line
		if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
		{
			// Check if over a room node
			RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

			if (roomNode != null)
			{
			//	if so set it as a child of the parent room node if it can be added
				if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
				{
				//	Set parent ID in child room node
					roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
				}
			}

			ClearLineDrag();
		}
	}

	private void ProcessMouseDragEvent(Event currentEvent)
	{
		// process right click drag event - draw line
		if (currentEvent.button == 1)
		{
			ProcessRightMouseDragEvent(currentEvent);
		}
		// process left click drag event - drag node graph
		//else if (currentEvent.button == 0)
		//{
		//	ProcessLeftMouseDragEvent(currentEvent.delta);
		//}
	}

	private void ProcessRightMouseDragEvent(Event currentEvent)
	{
		if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
		{
			DragConnectingLine(currentEvent.delta);
			GUI.changed = true;
		}
	}

	public void DragConnectingLine(Vector2 delta)
	{
		currentRoomNodeGraph.linePosition += delta;
	}

	private void ClearLineDrag()
	{
		currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
		currentRoomNodeGraph.linePosition = Vector2.zero;
		GUI.changed = true;
	}

	private void DrawRoomConnections()
	{
		// Loop through all room nodes
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.childRoomNodeIDList.Count > 0)
			{
				// Loop through child room nodes
				foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
				{
					// get child room node from dictionary
					if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
					{
						DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);

						GUI.changed = true;
					}
				}
			}
		}
	}

	private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
	{
		// get line start and end position
		Vector2 startPosition = parentRoomNode.rect.center;
		Vector2 endPosition = childRoomNode.rect.center;

		// calculate midway point
		Vector2 midPosition = (endPosition + startPosition) / 2f;

		// Vector from start to end position of line
		Vector2 direction = endPosition - startPosition;

		// Calulate normalised perpendicular positions from the mid point
		Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
		Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

		// Calculate mid point offset position for arrow head
		Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

		// Draw Arrow
		Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
		Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

		// Draw line
		Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

		GUI.changed = true;
	}

	private void DrawRoomNodes()
    {
        // Loop through all room nodes and draw themm
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
			if (roomNode.isSelected)
			{
				roomNode.Draw(roomNodeSelectedStyle);
			}
			else
			{
				roomNode.Draw(roomNodeStyle);
			}
		}

        GUI.changed = true;
    }

	private void InspectorSelectionChanged()
	{
		RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

		if (roomNodeGraph != null)
		{
			currentRoomNodeGraph = roomNodeGraph;
			GUI.changed = true;
		}
	}


}
