using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// œcie¿ka gdzie to ma byc
[CreateAssetMenu(fileName = "CurrentPlayer", menuName = "Scriptable Objects/Player/Current Player")]

public class CurrentPlayerSO : ScriptableObject
{
	public PlayerDetailsSO playerDetails;	// które jest wybrane

	public string playerName;	// imie
}
