using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Header DUNGEON
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    #endregion

    public RoomNodeTypeListSO roomNodeTypeList; // lista pokoi

	#region Header PLAYER
	[Space(10)]
	[Header("PLAYER")]
	#endregion Header PLAYER
	#region Tooltip
	[Tooltip("The current player scriptable object - this is used to reference the current player between scenes")]
	#endregion Tooltip
	public CurrentPlayerSO currentPlayer;   // to zachwouje statystyki gracza �eby go przenie�� na 2 poziom 

	#region Header MATERIALS
	[Space(10)]
	[Header("MATERIALS")]
	#endregion
	#region Tooltip
	[Tooltip("Dimmed Material")]
	#endregion
	public Material dimmedMaterial; // materia� ,,�wiat�a,,

	#region Tooltip
	[Tooltip("Sprite-Lit-Default Material")]
	#endregion
	public Material litMaterial;

	#region Tooltip
	[Tooltip("Populate with the Variable Lit Shader")]
	#endregion
	public Shader variableLitShader;

	#region Validation
#if UNITY_EDITOR
	// Validate the scriptable object details entered
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
		HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
		HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
		HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
		HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
	}

#endif
	#endregion
}

