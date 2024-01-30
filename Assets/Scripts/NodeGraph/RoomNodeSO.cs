using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;													//public 
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();	// mogê daæ je w public ¿eby widzieæ które dziedzicz¹ po których 
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();		// to te¿
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

	#region Editor Code

	// ten kod powinien byæ uruchamiany tylko w Unity Editorze
#if UNITY_EDITOR

	[HideInInspector] public Rect rect;
	[HideInInspector] public bool isLeftClickDragging = false;
	[HideInInspector] public bool isSelected = false;
	public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        // ³¹duje typy pokoi
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle nodeStyle)
	{
		//  Rysuje kwadrat tego noda
		GUILayout.BeginArea(rect, nodeStyle);

		// Sprawwdza czy s¹ zmiany
		EditorGUI.BeginChangeCheck();

		// je¿eli jest po³¹czona z rodzicem (starszym pokojem tzn po³¹czonym pierwszym) albo jest wejœciem to ma siê pokazaæ miejsce na wybór 
		if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
		{
			// nazwa której nie mo¿na zmieniæ
			EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
		}
		else
		{
			// Wyœwietl okno wyskakuj¹ce z nazw¹ wartoœci RoomNodeType które mo¿na wybieraæ domyœlnie jest none
			int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

			int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

			roomNodeType = roomNodeTypeList.list[selection];

			// If the room type selection has changed making child connections potentially invalid
			if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
			{
				// If a room node type has been changed and it already has children then delete the parent child links since we need to revalidate any
				if (childRoomNodeIDList.Count > 0)
				{
					for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
					{
						// Get child room node
						RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

						// If the child room node is not null
						if (childRoomNode != null)
						{
							// Remove childID from parent room node
							RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

							// Remove parentID from child room node
							childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
						}
					}
				}
			}
		}

		if (EditorGUI.EndChangeCheck())
			EditorUtility.SetDirty(this);

		GUILayout.EndArea();
	}

	public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

	public void ProcessEvents(Event currentEvent)
	{
		switch (currentEvent.type)
		{
			
			case EventType.MouseDown:
				ProcessMouseDownEvent(currentEvent);
				break;

			
			case EventType.MouseUp:
				ProcessMouseUpEvent(currentEvent);
				break;

			
			case EventType.MouseDrag:
				ProcessMouseDragEvent(currentEvent);
				break;

			default:
				break;
		}
	}

	private void ProcessMouseDownEvent(Event currentEvent)
	{
		// left click down
		if (currentEvent.button == 0)
		{
			ProcessLeftClickDownEvent();
		}
		//right click down
		else if (currentEvent.button == 1)
		{
			ProcessRightClickDownEvent(currentEvent);
		}

	}

	private void ProcessLeftClickDownEvent()
	{
		Selection.activeObject = this;

		
		// Zmienia po klikniêciu
		if (isSelected == true)
		{
			isSelected = false;
		}
		else
		{
			isSelected = true;
		}
	}

	private void ProcessRightClickDownEvent(Event currentEvent)	// zaczyna rysowaæ
	{
		roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
	}

	private void ProcessMouseUpEvent(Event currentEvent)
	{
		// je¿eli lewy przycisk
		if (currentEvent.button == 0)
		{
			ProcessLeftClickUpEvent();
		}
	}

	private void ProcessLeftClickUpEvent()
	{
		if (isLeftClickDragging)
		{
			isLeftClickDragging = false;
		}
	}

	private void ProcessMouseDragEvent(Event currentEvent)
	{
		// process left click drag event
		if (currentEvent.button == 0)
		{
			ProcessLeftMouseDragEvent(currentEvent);
		}
	}

	private void ProcessLeftMouseDragEvent(Event currentEvent)
	{
		isLeftClickDragging = true;

		DragNode(currentEvent.delta);
		GUI.changed = true;
	}

	public void DragNode(Vector2 delta)
	{
		rect.position += delta;
		EditorUtility.SetDirty(this);
	}

	public bool AddChildRoomNodeIDToRoomNode(string childID)
	{
		// sprawdza walidacjê czy da siê dodaæ
		if (IsChildRoomValid(childID))
		{
			childRoomNodeIDList.Add(childID);
			return true;
		}

		return false;
	}

	public bool IsChildRoomValid(string childID)
	{
		bool isConnectedBossNodeAlready = false;
		// czy istnieje ju¿ po³¹czony pokój typu boss w grafie
		foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
		{
			if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
				isConnectedBossNodeAlready = true;
		}

		// jak dziecko jest bossem i jest po³¹czone to false czyli sie nie da po³¹czyæ
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
			return false;

		// jak ma none to nie mo¿na ³¹czyæ
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
			return false;

		// ¿eby nie mo¿na by³o 2 razy po³¹czyæ tego samego
		if (childRoomNodeIDList.Contains(childID))
			return false;

		// ¿eby nie mo¿na by³o stworzyæ z tego samego co siê zaczyna
		if (id == childID)
			return false;

		// jak jest ju¿ po³¹czone (jest rodzicem) to ¿eby nie da³o siê wróciæ innym wejœciem do pokoju
		if (parentRoomNodeIDList.Contains(childID))
			return false;

		// ¿eby nie da³o siê wejœæ tzn ma rodzica
		if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
			return false;

		// 2 korytarze nie mog¹ siê po³¹czyæ
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
			return false;

		// 2 pokoje bez korytarza nie mog¹ siê po³¹czyæ
		if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
			return false;

		// Jeœli dodawany jest korytarz sprawdz w opcjach ile maksymalnie mo¿e mieæ odnóg (3)
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
			return false;

		// nie mo¿na wejœæ do wejœcia, z wejœcia mo¿na tylko wyjœæ (¿e pokój 1 pokój)
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
			return false;

		// 1 pokój wejœciowy i 1 wyjœciowy dla korytarza
		if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
			return false;

		return true;
	}

	public bool AddParentRoomNodeIDToRoomNode(string parentID)
	{
		parentRoomNodeIDList.Add(parentID);
		return true;
	}

	public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
	{
		// jezeli ma przypisane childId to usuñ
		if (childRoomNodeIDList.Contains(childID))
		{
			childRoomNodeIDList.Remove(childID);
			return true;
		}
		return false;
	}

	public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
	{
		// jezeli ma przypisane parentId to usuñ
		if (parentRoomNodeIDList.Contains(parentID))
		{
			parentRoomNodeIDList.Remove(parentID);
			return true;
		}
		return false;
	}

#endif
	#endregion




}
