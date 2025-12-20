using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class LoadSaveManager : MonoBehaviour
{
    public static LoadSaveManager Instance { get; private set; }

    // Save game data
    [XmlRoot("GameStateData")]
	public class GameStateData
	{
		public struct DataTransform
		{
			public float posX;
			public float posY;
			public float posZ;
			public float rotX;
			public float rotY;
			public float rotZ;
			public float scaleX;
			public float scaleY;
			public float scaleZ;
        }
			
		// Data for enemy
		public class DataEnemy
		{
			//Enemy Transform Data
			public DataTransform transform;

            //Enemy ID
			public int enemyID;

            //Health
			public int health;
        }
			
		// Data for player
		public class DataPlayer
		{
			//Transform Data
			public DataTransform transform;

			//Collected cash
			public float cash;

			//Has Collected Gun 01?
			public bool hasWeapon;

            //Health
			public int health;
        }
			
		// Instance variables
		public List<DataEnemy> enemies = new List<DataEnemy>();
		public DataPlayer player = new DataPlayer();
    }

	// Game data to save/load
	public GameStateData gameStateData = new GameStateData();


    // Saves game data to XML file
    public void Save(string fileName = "GameData.xml")
	{
		// Save game data
		XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
		FileStream fileStream = new FileStream(fileName, FileMode.Create);
		serializer.Serialize(fileStream, gameStateData);

		fileStream.Flush();
        fileStream.Close();
		fileStream.Dispose();
    }

	// Load game data from XML file
	public void Load(string fileName = "GameData.xml")
	{ 
		XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
		FileStream fileStream = new FileStream(fileName, FileMode.Open);
		gameStateData = serializer.Deserialize(fileStream) as GameStateData;

		fileStream.Flush();
		fileStream.Close();
		fileStream.Dispose();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Optionally: DontDestroyOnLoad(gameObject);
    }
}