using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BoardCreator : MonoBehaviour
{
	public enum TileType
	{
		Wall, Floor,
	}


	public int columns = 100;                                 
	public int rows = 100;                                  
	public IntRange numRooms = new IntRange (15, 20);         
	public IntRange roomWidth = new IntRange (3, 10);         
	public IntRange roomHeight = new IntRange (3, 10);        
	public IntRange corridorLength = new IntRange (6, 10);    
	public GameObject[] floorTiles;                          
	public GameObject[] wallTiles;                            
	public GameObject[] outerWallTiles; 					  
	public GameObject[] enemyTiles;
	public GameObject[] foodTiles;
	public GameObject player;
	public GameObject exit;
	public GameManager gameManage;

	private TileType[][] tiles;                             
	private Room[] rooms;                                    
	private Corridor[] corridors;                            
	private GameObject boardHolder;                           
	private List <Vector3> gridPositions = new List <Vector3> ();

	void Awake () {
		gameManage = GetComponent <GameManager> ();
	}

	private void Start ()
	{
		
		if (GameObject.FindGameObjectWithTag ("BoardHolder") == null) {
			boardHolder = new GameObject ("BoardHolder");
			boardHolder.tag = "BoardHolder";


			SetupTilesArray ();

			CreateRoomsAndCorridors ();

			SetTilesValuesForRooms ();
			SetTilesValuesForCorridors ();

			InstantiateTiles ();
			InstantiateOuterWalls ();

			CreateAllObjectsInRoom ();
		}

	}


	void SetupTilesArray ()
	{

		tiles = new TileType[columns][];


		for (int i = 0; i < tiles.Length; i++)
		{
			
			tiles[i] = new TileType[rows];
		}
	}


	void CreateRoomsAndCorridors ()
	{
		
		rooms = new Room[numRooms.Random];


		corridors = new Corridor[rooms.Length - 1];


		rooms[0] = new Room ();
		corridors[0] = new Corridor ();


		rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows);


		corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);

		for (int i = 1; i < rooms.Length; i++)
		{
			
			rooms[i] = new Room ();


			rooms[i].SetupRoom (roomWidth, roomHeight, columns, rows, corridors[i - 1]);


			if (i < corridors.Length)
			{
				
				corridors[i] = new Corridor ();


				corridors[i].SetupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, columns, rows, false);
			}

			if (i == rooms.Length * .5f)
			{
				Vector3 playerPos = new Vector3 (rooms[i].xPos, rooms[i].yPos, 0);

			}
		}

	}


	void SetTilesValuesForRooms ()
	{
		
		for (int i = 0; i < rooms.Length; i++)
		{
			Room currentRoom = rooms[i];

			for (int j = 0; j < currentRoom.roomWidth; j++)
			{
				int xCoord = currentRoom.xPos + j;

				for (int k = 0; k < currentRoom.roomHeight; k++)
				{
					int yCoord = currentRoom.yPos + k;

					tiles[xCoord][yCoord] = TileType.Floor;
				}
			}
		}
	}


	void SetTilesValuesForCorridors ()
	{
		for (int i = 0; i < corridors.Length; i++)
		{
			Corridor currentCorridor = corridors[i];

			for (int j = 0; j < currentCorridor.corridorLength; j++)
			{
				int xCoord = currentCorridor.startXPos;
				int yCoord = currentCorridor.startYPos;

				switch (currentCorridor.direction)
				{
				case Direction.North:
					yCoord += j;
					break;
				case Direction.East:
					xCoord += j;
					break;
				case Direction.South:
					yCoord -= j;
					break;
				case Direction.West:
					xCoord -= j;
					break;
				}

				tiles[xCoord][yCoord] = TileType.Floor;
			}
		}
	}


	void InstantiateTiles ()
	{
		for (int i = 0; i < tiles.Length; i++)
		{
			for (int j = 0; j < tiles[i].Length; j++)
			{
				InstantiateFromArray (floorTiles, i, j);

				if (tiles [i] [j] == TileType.Wall) {
					InstantiateFromArray (wallTiles, i, j);
				} else if (tiles [i] [j] == TileType.Floor) {
					gridPositions.Add (new Vector3 (i, j));
				}
			}
		}
	}


	void InstantiateOuterWalls ()
	{
		float leftEdgeX = -1f;
		float rightEdgeX = columns + 0f;
		float bottomEdgeY = -1f;
		float topEdgeY = rows + 0f;

		InstantiateVerticalOuterWall (leftEdgeX, bottomEdgeY, topEdgeY);
		InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY);

		InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY);
		InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY);
	}


	void InstantiateVerticalOuterWall (float xCoord, float startingY, float endingY)
	{
		float currentY = startingY;

		while (currentY <= endingY)
		{
			InstantiateFromArray(outerWallTiles, xCoord, currentY);

			currentY++;
		}
	}


	void InstantiateHorizontalOuterWall (float startingX, float endingX, float yCoord)
	{
		float currentX = startingX;

		while (currentX <= endingX)
		{
			InstantiateFromArray (outerWallTiles, currentX, yCoord);

			currentX++;
		}
	}


	void InstantiateFromArray (GameObject[] prefabs, float xCoord, float yCoord)
	{
		int randomIndex = Random.Range(0, prefabs.Length);

		Vector3 position = new Vector3(xCoord, yCoord, 0f);

		GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;


		tileInstance.transform.parent = boardHolder.transform;
	}
		
	Vector3 RandomPosition () {
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectsAtRandom (GameObject[] tileArray, int minimum, int maximum) {
		int objectCount = Random.Range (minimum, maximum + 1);

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition ();
			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
			GameObject tileCreated = Instantiate (tileChoice, randomPosition, Quaternion.identity) as GameObject;
			tileCreated.transform.parent = boardHolder.transform;
		}
	}

	void LayoutObjectAtRandom (GameObject tile, int minimum, int maximum) {
		int objectCount = Random.Range (minimum, maximum);

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition ();
			GameObject tileCreated = Instantiate (tile, randomPosition, Quaternion.identity) as GameObject;
			tileCreated.transform.parent = boardHolder.transform;
		}
	}

	void CreateEnemies (int level) {
		LayoutObjectsAtRandom (enemyTiles, 2, 8);
	}

	void CreateFood () {
		LayoutObjectsAtRandom (foodTiles, 15, 30);
	}

	void CreatePlayer () {
		LayoutObjectAtRandom (player, 1, 1);
	}

	void CreateExit () {
		LayoutObjectAtRandom (exit, 1, 1);
	}

	void CreateAllObjectsInRoom () {
		CreatePlayer ();
		CreateEnemies (gameManage.level);
		CreateFood ();
		CreateExit ();
	}
}