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
	private Room currentRoom;
	private Room previousRoom;
	private PlayerDetailsSO playerDetails;
	private Player player;

	[HideInInspector] public GameState gameState;

	protected override void Awake()
	{
		// wywo³anie klay bazowej
		base.Awake();


		playerDetails = GameResources.Instance.currentPlayer.playerDetails;


		InstantiatePlayer();

	}


	private void InstantiatePlayer()
	{

		GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);


		player = playerGameObject.GetComponent<Player>();

		player.Initialize(playerDetails);

	}


	// Start is called before the first frame update
	private void Start()
    {
		gameState = GameState.gameStarted;
	}

	private void OnEnable()
	{
		// room changed event.
		StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
	}

	private void OnDisable()
	{

		StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

	}

	private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
	{
		SetCurrentRoom(roomChangedEventArgs.room);
	}

	// Update is called once per frame
	private void Update()
    {
		HandleGameState();

		// testowanie generowania
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
				// 1 level
				PlayDungeonLevel(currentDungeonLevelListIndex);
				gameState = GameState.playingLevel;

				break;

		}
	}

	public Sprite GetPlayerMiniMapIcon()
	{
		return playerDetails.playerMiniMapIcon;
	}
	public Room GetCurrentRoom()
	{
		return currentRoom;
	}
	public void SetCurrentRoom(Room room)
	{
		previousRoom = currentRoom;
		currentRoom = room;

	}

	private void PlayDungeonLevel(int dungeonLevelListIndex)
	{
		// buduje dungeon dla levelu
		bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

		if (!dungeonBuiltSucessfully)
		{
			Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
		}
		StaticEventHandler.CallRoomChangedEvent(currentRoom);

		// ustawia gracza w pokoju
		player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

		player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);

	}

	public Player GetPlayer()
	{
		return player;
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
