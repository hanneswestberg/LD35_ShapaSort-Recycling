using UnityEngine;
using System.Collections;

public class MachineStorage : MonoBehaviour {

	//Upgradable variables
	private int[] upgradableStorageCapacity = new int[]{8, 14, 22, 32, 46};
	private int[] upgradableStorageCapacityCost = new int[]{300, 600, 1000, 1500};
	private int currentStorageCapacity = 0;
	private StatsManager statsMan;
	private GUIManager guiMan;
	private SoundManager soundMan;

	//Spawner
	private float spawnPosX;
	private float spawnPosY;
	[SerializeField] private GameObject garbage;
	[SerializeField] private BoxCollider2D spawn_Area;

	public int typeOfStorage = 0;

	void Start(){
		statsMan = GameObject.Find("_GameManager").GetComponent<StatsManager>();
		guiMan = GameObject.Find("_GameManager").GetComponent<GUIManager>();
		soundMan = GameObject.Find("_SoundManager").GetComponent<SoundManager>();
	}

	private int currentStorage = 0;

	public int CurrentStorage {
		get {
			return currentStorage;
		}
		set {
			guiMan.UpdateStorageFillers();
			currentStorage = value;
		}
	}

	public float GetCurrentStorageCapacity(){
		return upgradableStorageCapacity[currentStorageCapacity];
	}

	public float GetNextStorageCapacity(){
		if((currentStorageCapacity+2) <= upgradableStorageCapacity.Length){
			return upgradableStorageCapacity[currentStorageCapacity+1];
		}else{
			return 0;
		}
	}

	public int GetNextStorageCapacityCost(){
		if((currentStorageCapacity+1) < upgradableStorageCapacity.Length){
			return upgradableStorageCapacityCost[currentStorageCapacity];
		}else{
			return 0;
		}
	}

	public void UpgradeStorageCapacity(){
		if((currentStorageCapacity+2) <= upgradableStorageCapacity.Length){
			currentStorageCapacity++;
			guiMan.UpdateStorageFillers();
		}
	}

	public void CreateGarbage(){
		spawnPosX = spawn_Area.transform.position.x + Random.Range(-spawn_Area.size.x/2, spawn_Area.size.x/2);
		spawnPosY = spawn_Area.transform.position.y + Random.Range(-spawn_Area.size.y/2, spawn_Area.size.y/2);
		GameObject garbage_GO = (GameObject)GameObject.Instantiate(garbage, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.Euler(new Vector3(0f,0f, Random.Range(0, 255f))));
		garbage_GO.name = "Garbage";
	}




	public void StoreResource(GameObject res_GO){
		if(res_GO.GetComponent<ResourceScript>().type == typeOfStorage && ((currentStorage+1) <= upgradableStorageCapacity[currentStorageCapacity])){
			currentStorage++;
			statsMan.SuccesfulRating();
			statsMan.AddSuccessfulSort(res_GO.GetComponent<ResourceScript>().type);

			if(typeOfStorage == 1){
				soundMan.PlayEffect(0);
			}else if(typeOfStorage == 2){
				soundMan.PlayEffect(1);
			}else if(typeOfStorage == 3){
				soundMan.PlayEffect(2);
			}

			guiMan.UpdateStorageFillers();
			Destroy(res_GO);
		} else{
			// FAIL!!
			CreateGarbage();
			Destroy(res_GO);
			statsMan.MisstakeRatingHit();
			statsMan.AddMisstakeSort(res_GO.GetComponent<ResourceScript>().type);
			soundMan.PlayGarbageEffect();
		}
	}
}
