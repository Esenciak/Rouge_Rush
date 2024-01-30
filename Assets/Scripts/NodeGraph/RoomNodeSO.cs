using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;													//public 
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();	// mog� da� je w public �eby widzie� kt�re dziedzicz� po kt�rych 
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();		// to te�
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

	#region Editor Code

	// ten kod powinien by� uruchamiany tylko w Unity Editorze
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

        // ��duje typy pokoi
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle nodeStyle)
	{
		//  Rysuje kwadrat tego noda
		GUILayout.BeginArea(rect, nodeStyle);

		// Sprawwdza czy s� zmiany
		EditorGUI.BeginChangeCheck();

		// je�eli jest po��czona z rodzicem (starszym pokojem tzn po��czonym pierwszym) albo jest wej�ciem to ma si� pokaza� miejsce na wyb�r 
		if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
		{
			// nazwa kt�rej nie mo�na zmieni�
			EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
		}
		else
		{
			// Wy�wietl okno wyskakuj�ce z nazw� warto�ci RoomNodeType kt�re mo�na wybiera� domy�lnie jest none
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

		
		// Zmienia po klikni�ciu
		if (isSelected == true)
		{
			isSelected = false;
		}
		else
		{
			isSelected = true;
		}
	}

	private void ProcessRightClickDownEvent(Event currentEvent)	// zaczyna rysowa�
	{
		roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
	}

	private void ProcessMouseUpEvent(Event currentEvent)
	{
		// je�eli lewy przycisk
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
		// sprawdza walidacj� czy da si� doda�
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
		// czy istnieje ju� po��czony pok�j typu boss w grafie
		foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
		{
			if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
				isConnectedBossNodeAlready = true;
		}

		// jak dziecko jest bossem i jest po��czone to false czyli sie nie da po��czy�
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
			return false;

		// jak ma none to nie mo�na ��czy�
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
			return false;

		// �eby nie mo�na by�o 2 razy po��czy� tego samego
		if (childRoomNodeIDList.Contains(childID))
			return false;

		// �eby nie mo�na by�o stworzy� z tego samego co si� zaczyna
		if (id == childID)
			return false;

		// jak jest ju� po��czone (jest rodzicem) to �eby nie da�o si� wr�ci� innym wej�ciem do pokoju
		if (parentRoomNodeIDList.Contains(childID))
			return false;

		// �eby nie da�o si� wej�� tzn ma rodzica
		if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
			return false;

		// 2 korytarze nie mog� si� po��czy�
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
			return false;

		// 2 pokoje bez korytarza nie mog� si� po��czy�
		if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
			return false;

		// Je�li dodawany jest korytarz sprawdz w opcjach ile maksymalnie mo�e mie� odn�g (3)
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
			return false;

		// nie mo�na wej�� do wej�cia, z wej�cia mo�na tylko wyj�� (�e pok�j 1 pok�j)
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
			return false;

		// 1 pok�j wej�ciowy i 1 wyj�ciowy dla korytarza
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
		// jezeli ma przypisane childId to usu�
		if (childRoomNodeIDList.Contains(childID))
		{
			childRoomNodeIDList.Remove(childID);
			return true;
		}
		return false;
	}

	public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
	{
		// jezeli ma przypisane parentId to usu�
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
