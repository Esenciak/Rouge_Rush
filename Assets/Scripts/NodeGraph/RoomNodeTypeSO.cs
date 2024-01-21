using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="RoomNodeType_",menuName ="Scriptable Object/Dungeon/Room Node Type")]

public class RoomNodeTypeSO : ScriptableObject 
{
    public string roomNodeTypeName; // ka�dy room type

	// s�u�� do identyfikacji co to za pok�j
	#region Header
	[Header("Tylko to co powinno by� wida� w edytorze")]
	#endregion Header
	public bool displayInNodeGraphEditor = true;
	#region Header
	[Header("Jeden typ powinien by� korytarzem")]
	#endregion Header
	public bool isCorridor;
	#region Header
	[Header("Jeden typ powinien by� korytarzemNS")] // north soith
	#endregion Header
	public bool isCorridorNS;
	#region Header
	[Header("Jeden typ powinien by� korytarzemEW")] // east west
	#endregion Header
	public bool isCorridorEW;
	#region Header
	[Header("Jeden typ powinien by� Wej�ciem")] // ten nie musi by� bo i tak zrostanie zrobiony
	#endregion Header
	public bool isEntrance;
	#region Header
	[Header("Jeden typ powinien by� Boss Room")]
	#endregion Header
	public bool isBossRoom;
	#region Header
	[Header("Jeden typ powinien by� None (Unassigned)")] //defult zanim wybierzemy jaki b�dzie
	#endregion Header
	public bool isNone;


	#region Validation
#if UNITY_EDITOR // compiler directive, ten kod b�dzie si� odpal� tylko w unity editor
	private void OnValidate() // znajduje zmiany w inspekcji
	{
		HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName),roomNodeTypeName); 
	}
#endif
	#endregion
}
