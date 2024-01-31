using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{

	public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
	private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
	private List<RoomTemplateSO> roomTemplateList = null;
	private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

	private void OnEnable()
	{
		// ustawia (dimmed material)
		GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
	}

	private void OnDisable()
	{
		// ustawia dimmed material 
		GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
	}

	protected override void Awake()
	{
		base.Awake();

		// ��duje room node type list
		LoadRoomNodeTypeList();

	}

	private void LoadRoomNodeTypeList()
	{
		roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
	}

	public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
	{
		roomTemplateList = currentDungeonLevel.roomTemplateList;

		// �aduje SO room template
		LoadRoomTemplatesIntoDictionary();

		dungeonBuildSuccessful = false;
		int dungeonBuildAttempts = 0;

		while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)	//(1000 pr�b)
		{
			dungeonBuildAttempts++;

			// Wybiera losowe rozmieszczenie graf�w wcze�niej przygotowane
			RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

			int dungeonRebuildAttemptsForNodeGraph = 0;	//proby budowy dungeona z grafu (max 10)
			dungeonBuildSuccessful = false;

			// P�tla pr�b budowy dungeonu dla danego grafu (mo�e pr�bowa� kilka razy az do maxa)
			while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
			{
				// czy�ci i dodaje �e by�a pr�ba
				ClearDungeon();

				dungeonRebuildAttemptsForNodeGraph++;

				// kolejna pr�ba
				dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
			}


			if (dungeonBuildSuccessful)
			{
				// inicjuje pokoje
				InstantiateRoomGameobjects();
			}
		}

		return dungeonBuildSuccessful;
	}


	private void LoadRoomTemplatesIntoDictionary()
	{
		
		roomTemplateDictionary.Clear();

		// ��duje room template list
		foreach (RoomTemplateSO roomTemplate in roomTemplateList)
		{
			if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
			{
				roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
			}
			else
			{
				Debug.Log("Duplicate Room Template Key In " + roomTemplateList);
			}
		}
	}

	private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
	{

		
		Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

		// doadje wej�cie (node) do roomnodequeue z roomnode graphu
		RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

		if (entranceNode != null)
		{
			openRoomNodeQueue.Enqueue(entranceNode);
		}
		else
		{
			Debug.Log("No Entrance Node");
			return false;  // nie uda�o si� zbudowa�
		}

		bool noRoomOverlaps = true;


		noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

		// je�eli wsyzstko si� przeprocesuje i zaden z pokoi na siebie nie nachodzi to jest true
		if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
		{
			return true;
		}
		else
		{
			return false;
		}

	}

	private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
	{

		while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
		{
			// bierze kolejny pok�j z kolejki
			RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

			// dodaje child node do koleiki 
			foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
			{
				openRoomNodeQueue.Enqueue(childRoomNode);	// enqueue dodaje na koniec listy
			}

			// je�eli pok�j jest wej�ciem to dajemy �e jest udsawtione i do dictionary
			if (roomNode.roomNodeType.isEntrance)
			{
				RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

				Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

				room.isPositioned = true;

				// dodaje room do dictionary
				dungeonBuilderRoomDictionary.Add(room.id, room);
			}

			// else jak nie jest wej�ciem
			else
			{
				// to bierze parent room
				Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

				// i sprawdza czy da si� postawi� bez nak�adania si� pokoi
				noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
			}

		}

		return noRoomOverlaps;

	}

	private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
	{

		// zaczynamy od za�o�enia �e si� na�o�y pok�j
		bool roomOverlaps = true;

		//wykonaj gdy nak�adaj� si�
		while (roomOverlaps)
		{
			// wybiera losowo niepo��czone drzwi dost�pne od rodzica
			List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

			if (unconnectedAvailableParentDoorways.Count == 0)
			{
				// je�eli nie ma �adnych drzwi dost�pnych to si� nak�adaj�
				return false; // pokoje si� nak�adaj� 
			}

			Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

			// bierze losowo toom template z room nooda (np du�y pok�j to bierze large room)
			RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

			// tworzy pok�j
			Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

			// i go k�adzie, 
			if (PlaceTheRoom(parentRoom, doorwayParent, room))
			{
				// jak si� nie nak�adaj�to false konczy p�tle
				roomOverlaps = false;

				// pok�j jako postawiony
				room.isPositioned = true;

				// dodaje id i nazw� pokoju do dictionary
				dungeonBuilderRoomDictionary.Add(room.id, room);

			}
			else
			{
				roomOverlaps = true;
			}

		}

		return true;  // �aden si� nie nak�ada

	}

	private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
	{
		RoomTemplateSO roomtemplate = null;

		if (roomNode.roomNodeType.isCorridor)
		{
			switch (doorwayParent.orientation)
			{
				case Orientation.north:
				case Orientation.south:
					roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
					break;


				case Orientation.east:
				case Orientation.west:
					roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
					break;


				case Orientation.none:
					break;

				default:
					break;
			}
		}
		
		else
		{
			roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
		}


		return roomtemplate;
	}

	private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
	{

		Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

		
		if (doorway == null)
		{
			
			doorwayParent.isUnavailable = true;

			return false;
		}

		// Oblicza pozycj� wej�cia na mapie gry
		Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

		Vector2Int adjustment = Vector2Int.zero;

		
		switch (doorway.orientation)
		{
			case Orientation.north:
				adjustment = new Vector2Int(0, -1);
				break;

			case Orientation.east:
				adjustment = new Vector2Int(-1, 0);
				break;

			case Orientation.south:
				adjustment = new Vector2Int(0, 1);
				break;

			case Orientation.west:
				adjustment = new Vector2Int(1, 0);
				break;

			case Orientation.none:
				break;

			default:
				break;
		}

		//Oblicz doln� i g�rn� granic� pomieszczenia w oparciu o po�o�enie dopasowane do drzwi nadrz�dnych
		room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
		room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

		Room overlappingRoom = CheckForRoomOverlap(room);

		if (overlappingRoom == null)
		{
			// oznacza wej�cia jako po��czone i nie dost�pne
			doorwayParent.isConnected = true;
			doorwayParent.isUnavailable = true;

			doorway.isConnected = true;
			doorway.isUnavailable = true;

			// zwraca true zeby pokaza� �e s� po��czone bez nak�adania si�
			return true;
		}
		else
		{
			// po��czone Parent drzwi oznacza jako nie dostepn� �eby nei pr�bowa� wi�cej
			doorwayParent.isUnavailable = true;

			return false;
		}

	}

	private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
	{

		foreach (Doorway doorwayToCheck in doorwayList)
		{
			if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
			{
				return doorwayToCheck;
			}
			else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
			{
				return doorwayToCheck;
			}
			else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
			{
				return doorwayToCheck;
			}
			else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
			{
				return doorwayToCheck;
			}
		}

		return null;

	}

	private Room CheckForRoomOverlap(Room roomToTest)
	{
		// iteracja przez wszyskie pokoje
		foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
		{
			Room room = keyvaluepair.Value;

			// pomi� jak ten sam pok�j co pok�j do testowania lub pok�j nie zosta� ustawiony 
			if (room.id == roomToTest.id || !room.isPositioned)
				continue;

			// jak si� nak�adaj�
			if (IsOverLappingRoom(roomToTest, room))
			{
				return room;
			}
		}


		return null;

	}


	private bool IsOverLappingRoom(Room room1, Room room2)
	{
		bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);

		bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

		if (isOverlappingX && isOverlappingY)
		{
			return true;
		}
		else
		{
			return false;
		}

	}

	private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
	{
		if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
	{
		List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

		foreach (RoomTemplateSO roomTemplate in roomTemplateList)
		{
			if (roomTemplate.roomNodeType == roomNodeType)
			{
				matchingRoomTemplateList.Add(roomTemplate);
			}
		}

		// walidacja zwraca null jak nie ma
		if (matchingRoomTemplateList.Count == 0)
			return null;

		//  wybiera losowo templatke i j� zwraca
		return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];

	}

	private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
	{
		// Loop przez list� drzwi
		foreach (Doorway doorway in roomDoorwayList)
		{
			if (!doorway.isConnected && !doorway.isUnavailable)
				yield return doorway; // IENumerable to iterator a yield podaje nast�pn� warto�� w iteracji
		}
	}

	private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
	{
		Room room = new Room();

		room.templateID = roomTemplate.guid;
		room.id = roomNode.id;
		room.prefab = roomTemplate.prefab;
		//room.battleMusic = roomTemplate.battleMusic;
		//room.ambientMusic = roomTemplate.ambientMusic;
		room.roomNodeType = roomTemplate.roomNodeType;
		room.lowerBounds = roomTemplate.lowerBounds;
		room.upperBounds = roomTemplate.upperBounds;
		room.spawnPositionArray = roomTemplate.spawnPositionArray;
		//room.enemiesByLevelList = roomTemplate.enemiesByLevelList;
		//room.roomLevelEnemySpawnParametersList = roomTemplate.roomEnemySpawnParametersList;
		room.templateLowerBounds = roomTemplate.lowerBounds;
		room.templateUpperBounds = roomTemplate.upperBounds;
		room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
		room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

		// ustawia parent id
		if (roomNode.parentRoomNodeIDList.Count == 0) // wejscie
		{
			room.parentRoomID = "";
			room.isPreviouslyVisited = true;

			GameManager.Instance.SetCurrentRoom(room);

		}
		else
		{
			room.parentRoomID = roomNode.parentRoomNodeIDList[0];
		}


		// If there are no enemies to spawn then default the room to be clear of enemies
		//if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
		//{
			//room.isClearedOfEnemies = true;
		//}


		return room;

	}


	private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
	{
		if (roomNodeGraphList.Count > 0)
		{
			return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
		}
		else
		{
			Debug.Log("No room node graphs in list");
			return null;
		}
	}

	private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
	{
		List<Doorway> newDoorwayList = new List<Doorway>();

		foreach (Doorway doorway in oldDoorwayList)
		{
			Doorway newDoorway = new Doorway();

			newDoorway.position = doorway.position;
			newDoorway.orientation = doorway.orientation;
			newDoorway.doorPrefab = doorway.doorPrefab;
			newDoorway.isConnected = doorway.isConnected;
			newDoorway.isUnavailable = doorway.isUnavailable;
			newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
			newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
			newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

			newDoorwayList.Add(newDoorway);
		}

		return newDoorwayList;
	}

	private List<string> CopyStringList(List<string> oldStringList)
	{
		List<string> newStringList = new List<string>();

		foreach (string stringValue in oldStringList)
		{
			newStringList.Add(stringValue);
		}

		return newStringList;
	}
	private void InstantiateRoomGameobjects()
	{
		foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
		{
			Room room = keyvaluepair.Value;

			Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

			GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

			//inicjalizacja pokoju 
			InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

			instantiatedRoom.room = room;
			instantiatedRoom.Initialise(roomGameobject);
			room.instantiatedRoom = instantiatedRoom;	// zapis referencji

			//// test code do clearowania pokoi- poza bossem
			//if (!room.roomNodeType.isBossRoom)
			//{
			//    room.isClearedOfEnemies = true;
			//}
		}

	}
	public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
	{
		if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
		{
			return roomTemplate;
		}
		else
		{
			return null;
		}
	}

	public Room GetRoomByRoomID(string roomID)
	{
		if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
		{
			return room;
		}
		else
		{
			return null;
		}
	}
	
	private void ClearDungeon()
	{
		if (dungeonBuilderRoomDictionary.Count > 0)
		{
			foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
			{
				Room room = keyvaluepair.Value;

				if (room.instantiatedRoom != null)
				{
					Destroy(room.instantiatedRoom.gameObject);
				}
			}

			dungeonBuilderRoomDictionary.Clear();
		}
	}


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
