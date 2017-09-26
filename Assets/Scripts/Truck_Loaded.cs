using UnityEngine;
using System.Collections;

public class Truck_Loaded : MonoBehaviour {

	private int[] loadedResources = new int[3];
	public Resources_Spawner resSpawner;
	public bool canUnload = false;
	public TextMesh[] resourcesText;
	[SerializeField] private BoxCollider2D spawn_Area;
	[SerializeField] private GameObject garbage;
	private float spawnPosX;
	private float spawnPosY;
	private StatsManager statsMan;
	private SoundManager soundMan;


	public int type = 0;

	void Start(){
		resSpawner = GameObject.Find("MachineTransportBelt").GetComponent<Resources_Spawner>();
		statsMan = GameObject.Find("_GameManager").GetComponent<StatsManager>();
		soundMan = GameObject.Find("_SoundManager").GetComponent<SoundManager>();

		if(type == 1){
			spawn_Area = GameObject.Find("GarbageAreaLeft").GetComponent<BoxCollider2D>();
		}else if(type == 2){
			spawn_Area = GameObject.Find("GarbageAreaRight").GetComponent<BoxCollider2D>();
		}
	}

	public int[] LoadedResources {
		get {
			return loadedResources;
		}
	}

	public void CreateGarbage(){
		spawnPosX = spawn_Area.transform.position.x + Random.Range(-spawn_Area.size.x/2, spawn_Area.size.x/2);
		spawnPosY = spawn_Area.transform.position.y + Random.Range(-spawn_Area.size.y/2, spawn_Area.size.y/2);
		GameObject garbage_GO = (GameObject)GameObject.Instantiate(garbage, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.Euler(new Vector3(0f,0f, Random.Range(0, 255f))));
		garbage_GO.name = "Garbage";
	}

	public void SetLoadedResources(int[] res){
		loadedResources[0] = res[0];
		loadedResources[1] = res[1];
		loadedResources[2] = res[2];

		resourcesText[0].text = res[0].ToString();
		resourcesText[1].text = res[1].ToString();
		resourcesText[2].text = res[2].ToString();
	}

	public void UnloadResource(int res){
		loadedResources[res]--;
		resourcesText[res].text = loadedResources[res].ToString();
	}

	public void StartOfDayAnimation(){
		soundMan = GameObject.Find("_SoundManager").GetComponent<SoundManager>();
		StartCoroutine(WaitForSpawnAnim());
		GetComponent<Animator>().SetTrigger("Spawn");
		soundMan.PlayTruckSound(3);
	}

	public void EndOfDayAnimation(){
		if(LoadedResources[0] + LoadedResources[1] + LoadedResources[2] > 0){
			int resCount = LoadedResources[0] + LoadedResources[1] + LoadedResources[2];
			for (int i = 0; i < resCount; i++) {
				CreateGarbage();
				statsMan.MisstakeRatingHit();
			}
			soundMan.PlayGarbageEffect();
		}
		GetComponent<Animator>().SetTrigger("DeSpawn");
		soundMan.PlayTruckSound(5);
		StartCoroutine(WaitForDeSpawnAnim());
	}

	IEnumerator WaitForSpawnAnim(){
		yield return new WaitForSeconds(1f);
		soundMan.PlayTruckSound(4);
		resSpawner.UnloadTruck(this.GetComponent<Truck_Loaded>(), type);
		canUnload = true;
	}

	IEnumerator WaitForDeSpawnAnim(){
		canUnload = false;
		yield return new WaitForSeconds(3f);
		Destroy(this.gameObject);
	}
}
