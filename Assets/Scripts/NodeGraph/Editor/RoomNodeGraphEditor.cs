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

	// Szeroko�� kratek
	private const float gridLarge = 100f;
	private const float gridSmall = 25f;

	// Node layout 
	private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // Linie ��cz�ce Nody
	private const float connectingLineWidth = 3f;
	private const float connectingLineArrowSize = 6f;

	// �cie�ka gdzie mo�na to w��czy�
	[MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]

    private void OnEnable()
    {
		Selection.selectionChanged += InspectorSelectionChanged;	// dzi�ki temu po zmianie si� od razu zmieni 

		// wygl�d 
		roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		// Pod�wietlenie i wygl�d jak si� kliknie w noda
		roomNodeSelectedStyle = new GUIStyle();
		roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
		roomNodeSelectedStyle.normal.textColor = Color.white;
		roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
		roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

		//�aduje room node type
		roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

	private void OnDisable()
	{
		// jak si� wy��czy to nie zaznacza kt�re nody ( te w inspektorze)
		Selection.selectionChanged -= InspectorSelectionChanged;
	}

	[OnOpenAsset(0)]	// to robi �e to zostaje wywo�ane zawsze jako peirwsze, 1,2,3 jakbym stworzy� to by w takiej kolejnosci si� wywo�ywa�y
    public static bool OnDoubleClickAsset(int instanceID, int line)	// jak si� podw�jnie kliknie to si� w��czy
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

			// Procesowanie event�w
			ProcessEvents(Event.current);

			// Rysowanie strza�ek
			DrawRoomConnections();

			// Rysuje noody
			DrawRoomNodes();
        }
        if (GUI.changed)
            Repaint();
    }
	private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)	// rysuje t�o
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

		// pobiera graph nad kt�rym jest myszka jak jest null to nie �aczy
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
			// kontunuuje dzia�anie
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
            // jak si� kliknie to myszk�
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
			// jak si� odklika
			case EventType.MouseUp:
				ProcessMouseUpEvent(currentEvent);
				break;

			// jak si� przesuwa
			case EventType.MouseDrag:
				ProcessMouseDragEvent(currentEvent);
				break;


			default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // jak si� kliknie prawy przycisk myszki (0 - lewy, 1 - prawy) to otwiera si� menu
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
        GenericMenu menu = new GenericMenu();	// �eby nie wycieka�y dane

		menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);	// false bo nie jest zaznaczony trzeba zmieni�  stan
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
		menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

		menu.ShowAsContext();
	}

    private void CreateRoomNode(object mousePositionObject)
    {
		// Jak dodaje 1 node to od razu dodaje kolejny kt�ry b�dzie wej�ciem do lochu, ustawi�em �e jak b�dzie 0 to �eby si� tylko pojawia� bo nie doda�em opcji samemu go generowania w edytorze
		if (currentRoomNodeGraph.roomNodeList.Count == 0)
		{
			CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));	// pojawia si� wej�cie
		}

		CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

		// Utworzy obiekt skryptu pokoju 
		RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

		// Dodaj node pokoju do bie��cej listy
		currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // ustawia warto�� noda
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

		// Dodaj noda pokoju do bazy danych zasob�w
		AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

		// od�wierza graph
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

		// Usu� nody pokoju z kolejki
		while (roomNodeDeletionQueue.Count > 0)
		{
			// Pobiera room node z kolejki
			RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

			// usuwa node z ,,s�ownika"
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

					// Je�eli ,,dziecko,, jest wybrane
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

		// Czy�ci
		ClearAllSelectedRoomNodes();
	}

	private void ClearAllSelectedRoomNodes()    // usuwa zaznaczone nody z listy
	{
		foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)	// iteracja
		{
			if (roomNode.isSelected)	// jak s� zaznaczone
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
		// jak si� pu�ci przycisk przed najechaniem podczas rysowania linii
		if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
		{
			// sprawcza czy jest nad nodem
			RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

			if (roomNode != null)
			{
				//	Je�li tak, ustaw go jako dziecko w�z�a rodzica, je�li mo�e by� dodany
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
		//  przeci�ganie prawym przyciskiem rysuje lini�
		if (currentEvent.button == 1)
		{
			ProcessRightMouseDragEvent(currentEvent);
		}
		// przeci�ganie lewym przyciskiem przeci�ga node
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

		// Srodek strza�ki
		Vector2 midPosition = (endPosition + startPosition) / 2f;

		// Vector z startu do ko�ca
		Vector2 direction = endPosition - startPosition;

		// oblicza znormalizowan� pozycj� prostopadle do �rodka �eby by�a taka strza�eczka
		Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
		Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

		//Oblicza przesuni�cie �rodka na koniec strza�ki
		Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

		// rysuje strza�k�
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
