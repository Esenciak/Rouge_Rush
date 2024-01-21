using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Object/Dungeon/Room Node Type List")]

public class RoomNodeTypeListSO : ScriptableObject
{
	#region Header ROOM NODE TYPE LIST
	[Space(10)]
	[Header("ROOM NODE TYPE LIST")]
	#endregion
	#region Tooltip
	[Tooltip("Tu powinny siê zgraæ RoomNodeTypeListSO, uzywane zamiast enum")]
	#endregion
	public List<RoomNodeTypeListSO> list;



	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckENumerableValues(this, nameof(list), list);
	}
#endif
	#endregion
}
