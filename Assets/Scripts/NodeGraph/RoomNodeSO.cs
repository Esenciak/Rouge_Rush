using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
	[HideInInspector] public string id;
	[HideInInspector] public List<string> parentRoomNodeIdList = new List<string>();
	[HideInInspector] public List<string> childRoomNodeIdList = new List<string>();
	public RoomNodeTypeSO roomNodeType;
	[HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
}
