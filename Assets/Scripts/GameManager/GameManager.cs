using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{

	#region Header GAMEOBJECT REFERENCES
	[Space(10)]
	[Header("GAMEOBJECT REFERENCES")]
	#endregion Header GAMEOBJECT REFERENCES

	#region Tooltip

	[Tooltip("Populate with pause menu gameobject in hierarchy")]

	#endregion Tooltip

	[SerializeField] private GameObject pauseMenu;

	#region Tooltip
	[Tooltip("Populate with the MessageText textmeshpro component in the FadeScreenUI")]
	#endregion Tooltip
	[SerializeField] private TextMeshProUGUI messageTextTMP;

	#region Tooltip
	[Tooltip("Populate with the FadeImage canvasgroup component in the FadeScreenUI")]
	#endregion Tooltip
	[SerializeField] private CanvasGroup canvasGroup;

	#region Header DUNGEON LEVELS

	[Space(10)]
	[Header("DUNGEON LEVELS")]

	#endregion Header DUNGEON LEVELS

	#region Tooltip

	[Tooltip("Populate with the dungeon level scriptable objects")]

	#endregion Tooltip

	[SerializeField] private List<DungeonLevelSO> dungeonLevelList;

	#region Tooltip

	[Tooltip("Populate with the starting dungeon level for testing , first level = 0")]

	#endregion Tooltip

	[SerializeField] private int currentDungeonLevelListIndex = 0;

	[HideInInspector] public GameState gameState;

	// Start is called before the first frame update
	private void Start()
    {
		gameState = GameState.gameStarted;
	}

    // Update is called once per frame
    private void Update()
    {
		HandleGameState();

		// For testing
		if (Input.GetKeyDown(KeyCode.R))
		{
		    gameState = GameState.gameStarted;
		}
	}

	private void HandleGameState()
	{
		// Handle game state
		switch (gameState)
		{
			case GameState.gameStarted:
				// Play first level
				PlayDungeonLevel(currentDungeonLevelListIndex);
				gameState = GameState.playingLevel;

				break;

		}
	}


	private void PlayDungeonLevel(int dungeonLevelListIndex)
	{
		// Build dungeon for level
		bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

		if (!dungeonBuiltSucessfully)
		{
			Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
		}


	}



	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{

		HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
	}

#endif

	#endregion Validation

}
