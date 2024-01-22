using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="RoomNodeType_",menuName ="Scriptable Object/Dungeon/Room Node Type")]

public class RoomNodeTypeSO : ScriptableObject 
{
    public string roomNodeTypeName; // ka¿dy room type

	// s³u¿¹ do identyfikacji co to za pokój
	#region Header
	[Header("Tylko to co powinno byæ widaæ w edytorze")]
	#endregion Header
	public bool displayInNodeGraphEditor = true;
	#region Header
	[Header("Jeden typ powinien byæ korytarzem")]
	#endregion Header
	public bool isCorridor;
	#region Header
	[Header("Jeden typ powinien byæ korytarzemNS")] // north soith
	#endregion Header
	public bool isCorridorNS;
	#region Header
	[Header("Jeden typ powinien byæ korytarzemEW")] // east west
	#endregion Header
	public bool isCorridorEW;
	#region Header
	[Header("Jeden typ powinien byæ Wejœciem")] // ten nie musi byæ bo i tak zrostanie zrobiony
	#endregion Header
	public bool isEntrance;
	#region Header
	[Header("Jeden typ powinien byæ Boss Room")]
	#endregion Header
	public bool isBossRoom;
	#region Header
	[Header("Jeden typ powinien byæ None (Unassigned)")] //defult zanim wybierzemy jaki bêdzie
	#endregion Header
	public bool isNone;


	#region Validation
#if UNITY_EDITOR // compiler directive, ten kod bêdzie siê odpalæ tylko w unity editor
	private void OnValidate() // znajduje zmiany w inspekcji
	{
		HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName),roomNodeTypeName); 
	}
#endif
	#endregion
}
