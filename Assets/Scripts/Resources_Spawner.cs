using UnityEngine;
using System.Collections;

public class Resources_Spawner : MonoBehaviour {

	[SerializeField] private GameObject resourcePaperRectanglePrefab;
	[SerializeField] private GameObject resourceGlasCirclePrefab;
	[SerializeField] private GameObject resourceMetalTrianglePrefab;
	[SerializeField] private Animator[] transportBelts;

	[SerializeField] private BoxCollider2D[] spawn_Areas;
	private LevelManager levelMan;
	private SoundManager soundMan;

	// Upgradable variables
	private float[] upgradesTransportspeed = new float[]{0.85f, 1.1f, 1.4f, 1.6f, 1.8f, 2f};
	private int[] upgradesTransportspeedCost = new int[]{600, 1000, 1500, 2200, 3000};
	private int currentTransportspeed = 0;

	private float[] upgradesUnloadSpeed = new float[]{1.3f, 1.1f, 0.8f, 0.7f, 0.6f, 0.5f};
	private int[] upgradesUnloadSpeedCost = new int[]{600, 1000, 1500, 2200, 3000};
	private int currentUnloadSpeed = 0;



	// Spawner
	private float spawnPosX;
	private float spawnPosY;

	// TIME LOGIC HERE

	void Update(){
		//SpawnResource(Random.Range(1,4));
	}

	void Start(){
		soundMan = GameObject.Find("_SoundManager").GetComponent<SoundManager>();
		levelMan = GameObject.Find("_GameManager").GetComponent<LevelManager>();
	}


	public void UnloadTruck(Truck_Loaded truck, int type){
		StartCoroutine(UnloadTruckRepeter(truck, type));
	}

	void SpawnResource(int res, int type){
		// Get spawn pos
		if(type == 1){
			spawnPosX = spawn_Areas[0].transform.position.x + Random.Range(-spawn_Areas[0].size.x/2, spawn_Areas[0].size.x/2);
			spawnPosY = spawn_Areas[0].transform.position.y + Random.Range(-spawn_Areas[0].size.y/2, spawn_Areas[0].size.y/2);
		} else{
			spawnPosX = spawn_Areas[1].transform.position.x + Random.Range(-spawn_Areas[1].size.x/2, spawn_Areas[1].size.x/2);
			spawnPosY = spawn_Areas[1].transform.position.y + Random.Range(-spawn_Areas[1].size.y/2, spawn_Areas[1].size.y/2);
		}



		if(res == 0){
			GameObject res_GO = (GameObject)GameObject.Instantiate(resourcePaperRectanglePrefab, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.identity);
			res_GO.name = "resource_Paper_Rectangle";
			res_GO.GetComponent<ResourceScript>().SetMovementDirection(1);
			res_GO.GetComponent<ResourceScript>().spawner = GetComponent<Resources_Spawner>();
			res_GO.GetComponent<ResourceScript>().type = 1;
		}else if (res == 1){
			
			GameObject res_GO = (GameObject)GameObject.Instantiate(resourceGlasCirclePrefab, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.identity);
			res_GO.name = "resource_Glas_Circle";
			res_GO.GetComponent<ResourceScript>().SetMovementDirection(1);
			res_GO.GetComponent<ResourceScript>().spawner = GetComponent<Resources_Spawner>();
			res_GO.GetComponent<ResourceScript>().type = 2;
		}else if (res == 2){
			
			GameObject res_GO = (GameObject)GameObject.Instantiate(resourceMetalTrianglePrefab, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.identity);
			res_GO.name = "resource_Metal_Triangle";
			res_GO.GetComponent<ResourceScript>().SetMovementDirection(1);
			res_GO.GetComponent<ResourceScript>().spawner = GetComponent<Resources_Spawner>();
			res_GO.GetComponent<ResourceScript>().type = 3;
		}else{
			Debug.LogError("Unknown resource to spawn!");
		}
		soundMan.PlayEffect(6);
	}

	void UpdateTransportBeltSpeed(){
		foreach(Animator belt in transportBelts){
			belt.speed = (1f/0.75f)*upgradesTransportspeed[currentTransportspeed];
		}
	}

	public float GetCurrentTransportSpeed(){
		return upgradesTransportspeed[currentTransportspeed];
	}

	public float GetNextTransportSpeed(){
		if((currentTransportspeed+2) <= upgradesTransportspeed.Length){
			return upgradesTransportspeed[currentTransportspeed+1];
		}else{
			return 0;
		}
	}

	public float GetCurrentUnloadSpeed(){
		return upgradesUnloadSpeed[currentUnloadSpeed];
	}

	public float GetNextUnloadSpeed(){
		if((currentUnloadSpeed+2) <= upgradesUnloadSpeed.Length){
			return upgradesUnloadSpeed[currentUnloadSpeed+1];
		}else{
			return 0;
		}
	}

	public int GetTransportationSpeedUpgradeCost(){
		if((currentTransportspeed+1) < upgradesTransportspeed.Length){
			return upgradesTransportspeedCost[currentTransportspeed];
		}else{
			return 0;
		}
	}

	public int GetUnloadSpeedUpgradeCost(){
		if((currentUnloadSpeed+1) < upgradesUnloadSpeed.Length){
			return upgradesUnloadSpeedCost[currentUnloadSpeed];
		}else{
			return 0;
		}
	}

	public void UpgradeTransportSpeed(){
		if((currentTransportspeed+2) <= upgradesTransportspeed.Length){
			UpdateTransportBeltSpeed();
			currentTransportspeed++;
		}
	}

	public void UpgradeUnloadSpeed(){
		if((currentUnloadSpeed+2) <= upgradesUnloadSpeed.Length){
			currentUnloadSpeed++;
		}
	}

	IEnumerator UnloadTruckRepeter(Truck_Loaded truck, int type){
		bool validLoadedRes = false;

		while (validLoadedRes == false){
			int res = Random.Range(0, 3);

			if(truck.LoadedResources[res] > 0){
				truck.UnloadResource(res);
				SpawnResource(res, type);
				validLoadedRes = true;
			}
		}

		if((truck.LoadedResources[0] + truck.LoadedResources[1] + truck.LoadedResources[2]) > 0 && levelMan.isDay == true){
			yield return new WaitForSeconds(upgradesUnloadSpeed[currentUnloadSpeed]);

		StartCoroutine(UnloadTruckRepeter(truck, type));
		} else{
			truck.EndOfDayAnimation();
		}
	}
}
