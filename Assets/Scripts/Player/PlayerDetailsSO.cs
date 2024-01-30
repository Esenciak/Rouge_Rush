using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// tworzy œcie¿kê w menu do wywo³ania stworzenia postaci
[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]

public class PlayerDetailsSO : ScriptableObject
{
	#region Tooltip
	[Tooltip("Player character name.")]
	#endregion
	public string playerCharacterName; // nazwa postaci
	#region Tooltip
	[Tooltip("Prefab postaci")]
	#endregion
	public GameObject playerPrefab; // prefab
	#region Tooltip
	[Tooltip("Player runtime animator controller")]
	#endregion
	public RuntimeAnimatorController runtimeAnimatorController; //animator
	#region Tooltip
	[Tooltip("Startowe HP gracza")]
	#endregion
	public int playerHealthAmount;  //hp
	#region Tooltip
	[Tooltip("czy ma idpornoœæ na obra¿enia")]
	#endregion
	public bool isImmuneAfterHit = false;   // np podczas rolla
	#region Tooltip
	[Tooltip("Jak d³ugo w sekundach ma byæ odporny na obra¿enia")]
	#endregion
	public float hitImmunityTime;   // jak d³ugo
	#region Tooltip
	[Tooltip("ikonka na minimapie")]
	#endregion
	public Sprite playerMiniMapIcon;    // ikonka na mapce nie wiem czy dodam
	#region Tooltip
	[Tooltip("sprite rêki")]
	#endregion
	public Sprite playerHandSprite;     // sprite rêki trzymania broni


	// walidacja czy nie s¹ puste
	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
		HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
		HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
		HelperUtilities.ValidateCheckNullValue(this, nameof(playerMiniMapIcon), playerMiniMapIcon);
		HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
		HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
	}
#endif
	#endregion



}
