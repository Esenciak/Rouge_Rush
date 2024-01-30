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


	private Vector2 graphOffset;
	private Vector2 graphDrag;

	// Szerokoœæ kratek
	private const float gridLarge = 100f;
	private const float gridSmall = 25f;

	// Node layout 
	private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // Linie ³¹cz¹ce Nody
	private const float connectingLineWidth = 3f;
	private const float connectingLineArrowSize = 6f;

	// œcie¿ka gdzie mo¿na to w³¹czyæ
	[MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]

    private void OnEnable()
    {
		Selection.selectionChanged += InspectorSelectionChanged;	// dziêki temu po zmianie siê od razu zmieni 

		// wygl¹d 
		roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		// Podœwietlenie i wygl¹d jak siê kliknie w noda
		roomNodeSelectedStyle = new GUIStyle();
		roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
		roomNodeSelectedStyle.normal.textColor = Color.white;
		roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
		roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		//³aduje room node type
		roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

	private void OnDisable()
	{
		// jak siê wy³¹czy to nie zaznacza które nody ( te w inspektorze)
		Selection.selectionChanged -= InspectorSelectionChanged;
	}

	[OnOpenAsset(0)]	// to robi ¿e to zostaje wywo³ane zawsze jako peirwsze, 1,2,3 jakbym stworzy³ to by w takiej kolejnosci siê wywo³ywa³y
    public static bool OnDoubleClickAsset(int instanceID, int line)	// jak siê podwójnie kliknie to siê w³¹czy
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


    private static void OpenWindow()	// wiadomo
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }



    private void OnGUI()
    {
        if (currentRoomNodeGraph != null)
        {
			// rysuje Grid
			DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
			DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);

			// Rysuje linie
			DrawDraggedLine();

			// Procesowanie eventów
			ProcessEvents(Event.current);

			// Rysowanie strza³ek
			DrawRoomConnections();

			// Rysuje noody
			DrawRoomNodes();
        }
        if (GUI.changed)
            Repaint();
    }
	private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)	// rysuje t³o
	{
		int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
		int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

		Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

		graphOffset += graphDrag * 0.5f;

		Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

		for (int i = 0; i < verticalLineCount; i++)
		{
			Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
		}

		for (int j = 0; j < horizontalLineCount; j++)
		{
			Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset, new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
		}

		Handles.color = Color.white;

	}


	private void DrawDraggedLine()
	{
		if (currentRoomNodeGraph.linePosition != Vector2.zero)
		{
			//Rysuje linie z noda do pozycji wybranej
			Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
		}
	}

	private void ProcessEvents(Event currentEvent)
    {
		// Reset graphu
		graphDrag = Vector2.zero;

		// pobiera graph nad którym jest myszka jak jest null to nie ³aczy
		if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
		{
			currentRoomNode = IsMouseOverRoomNode(currentEvent);
		}

		// Jak nie jset myszka nad graphem to kontunuuj bez ,,zaczepiania,,
		if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
		{
			ProcessRoomNodeGraphEvents(currentEvent);
		}
		
		else
		{
			// kontunuuje dzia³anie
			currentRoomNode.ProcessEvents(currentEvent);
		}
	}

	private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)	// sprawdza czy mysz jest nad nodem
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
            // jak siê kliknie to myszk¹
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
			// jak siê odklika
			case EventType.MouseUp:
				ProcessMouseUpEvent(currentEvent);
				break;

			// jak siê przesuwa
			case EventType.MouseDrag:
				ProcessMouseDragEvent(currentEvent);
				break;


			default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // jak siê kliknie prawy przycisk myszki (0 - lewy, 1 - prawy) to otwiera siê menu
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

    private void ShowContextMenu(Vector2 mousePosition)		// menu z prawego przycisku myszki
    {
        GenericMenu menu = new GenericMenu();	// ¿eby nie wycieka³y dane

		menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);	// false bo nie jest zaznaczony trzeba zmieniæ  stan
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
		menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

		menu.ShowAsContext();
	}

    private void CreateRoomNode(object mousePositionObject)
    {
		// Jak dodaje 1 node to od razu dodaje kolejny który bêdzie wejœciem do lochu, ustawi³em ¿e jak bêdzie 0 to ¿eby siê tylko pojawia³ bo nie doda³em opcji samemu go generowania w edytorze
		if (currentRoomNodeGraph.roomNodeList.Count == 0)
		{
			CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));	// pojawia siê wejœcie
		}

		CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

		// Utworzy obiekt skryptu pokoju 
		RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

		// Dodaj node pokoju do bie¿¹cej listy
		currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // ustawia wartoœæ noda
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

		// Dodaj noda pokoju do bazy danych zasobów
		AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

		// odœwierza graph
		currentRoomNodeGraph.OnValidate();
	}

	private void DeleteSelectedRoomNodes()
	{
		Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

		// Loop wszyskich nodow
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
			{
				roomNodeDeletionQueue.Enqueue(roomNode);

				// iteracja przez child nodes
				foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
				{
					// Ustawia child room node
					RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);

					if (childRoomNode != null)
					{
						// usuwa parentID z child room node
						childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
					}
				}

				// Iteracja przez parent room node id
				foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
				{
					// pobiera parent node
					RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);

					if (parentRoomNode != null)
					{
						// usuwa childID z rodzica
						parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
					}
				}
			}
		}

		// Usuñ nody pokoju z kolejki
		while (roomNodeDeletionQueue.Count > 0)
		{
			// Pobiera room node z kolejki
			RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

			// usuwa node z ,,s³ownika"
			currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);

			// usuwa node z list
			currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

			// usuwa node z Asset database
			DestroyImmediate(roomNodeToDelete, true);

			// zapisuje asset database
			AssetDatabase.SaveAssets();

		}
	}

	private void DeleteSelectedRoomNodeLinks()
	{
		// Iteracja przez liste
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
			{
				for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
				{
					// pobiera child room node
					RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

					// Je¿eli ,,dziecko,, jest wybrane
					if (childRoomNode != null && childRoomNode.isSelected)
					{
						// usuwa childID z parent room node
						roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

						// usuwa parentID z child room node
						childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
					}
				}
			}
		}

		// Czyœci
		ClearAllSelectedRoomNodes();
	}

	private void ClearAllSelectedRoomNodes()    // usuwa zaznaczone nody z listy
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)	// iteracja
		{
			if (roomNode.isSelected)	// jak s¹ zaznaczone
			{
				roomNode.isSelected = false;	// to je odznacza

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
		// jak siê puœci przycisk przed najechaniem podczas rysowania linii
		if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
		{
			// sprawcza czy jest nad nodem
			RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

			if (roomNode != null)
			{
				//	Jeœli tak, ustaw go jako dziecko wêz³a rodzica, jeœli mo¿e byæ dodany
				if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
				{
					//	Ustaw ID rodzica w nodzie dziecka
					roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
				}
			}

			ClearLineDrag();
		}
	}

	private void ProcessMouseDragEvent(Event currentEvent)
	{
		//  przeci¹ganie prawym przyciskiem rysuje liniê
		if (currentEvent.button == 1)
		{
			ProcessRightMouseDragEvent(currentEvent);
		}
		// przeci¹ganie lewym przyciskiem przeci¹ga node
		else if (currentEvent.button == 0)
		{
			ProcessLeftMouseDragEvent(currentEvent.delta);
		}
	}

	private void ProcessRightMouseDragEvent(Event currentEvent)	// rysuje 
	{
		if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
		{
			DragConnectingLine(currentEvent.delta);
			GUI.changed = true;
		}
	}

	private void ProcessLeftMouseDragEvent(Vector2 dragDelta) 
	{
		graphDrag = dragDelta;

		for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
		{
			currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
		}

		GUI.changed = true;
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
		// Loop 
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
		{
			if (roomNode.childRoomNodeIDList.Count > 0)
			{
				// Loop przez dzieci
				foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
				{
					// bierze child room node 
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
		// start i koniec (pozycja)
		Vector2 startPosition = parentRoomNode.rect.center;
		Vector2 endPosition = childRoomNode.rect.center;

		// Srodek strza³ki
		Vector2 midPosition = (endPosition + startPosition) / 2f;

		// Vector z startu do koñca
		Vector2 direction = endPosition - startPosition;

		// oblicza znormalizowan¹ pozycjê prostopadle do œrodka ¿eby by³a taka strza³eczka
		Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
		Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

		//Oblicza przesuniêcie œrodka na koniec strza³ki
		Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

		// rysuje strza³kê
		Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
		Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

		// rysuje linie
		Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

		GUI.changed = true;
	}

	private void DrawRoomNodes()
    {
        // Loop
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
