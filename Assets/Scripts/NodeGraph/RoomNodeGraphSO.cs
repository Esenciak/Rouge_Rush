using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//tworzy w assets mozliwoœæ tworzenia Room Node Graph
[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Skriptable Object/Dungeon/Room Node Graph")]

public class NewBehaviourScript : ScriptableObject
{
	[HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
	[HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
	[HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();
}
