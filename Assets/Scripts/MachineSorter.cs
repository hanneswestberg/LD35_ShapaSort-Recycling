using UnityEngine;
using System.Collections;

public class MachineSorter : MonoBehaviour {

	[SerializeField] private GameObject[] doors;
	[SerializeField] private GameObject transportBelt_2;


	// Upgradable variables
	bool canHandleTwo = false;
	private float[] upgradableSortingSpeed = new float[]{0.4f, 0.3f, 0.2f, 0.1f, 0.0f};
	private int[] upgradableSortingSpeedCost = new int[]{500, 800, 1200, 1500};
	private int handleTwoCost = 2000;
	private int currentSortingSpeed = 0;

	// States
	private int currentDirection_1 = 0;
	private int currentDirection_2 = 0;
	private int tempCurrentDirr_1 = 0;
	private int tempCurrentDirr_2 = 0;
	private Vector2[] resourceDirections = new Vector2[]{Vector2.left, Vector2.up, Vector2.right};
	private float currentSortingDelay_1 = 0;
	private float currentSortingDelay_2 = 0;

	//Spawner
	private float spawnPosX;
	private float spawnPosY;
	[SerializeField] private GameObject garbage;
	[SerializeField] private BoxCollider2D spawn_Area;
	private SoundManager soundMan;

	void Start(){
		soundMan = GameObject.Find("_SoundManager").GetComponent<SoundManager>();
	}

	// Update is called once per frame
	void Update () {
		tempCurrentDirr_1 = currentDirection_1;
		tempCurrentDirr_2 = currentDirection_2;
		currentDirection_1 = 0;
		currentDirection_2 = 0;


		if((Input.GetButton("Left") && currentDirection_1 == 0) || (Input.GetButton("Left") && (canHandleTwo == true && currentDirection_2 == 0))){
			if(canHandleTwo == true && currentDirection_1 != 0){
				currentDirection_2 = 1;
			}else{
				currentDirection_1 = 1;
			}
			doors[0].GetComponent<Animator>().SetBool("DoorOpen", true);
		} 

		if((Input.GetButton("Up") && currentDirection_1 == 0) || (Input.GetButton("Up") && (canHandleTwo == true && currentDirection_2 == 0))){
			if(canHandleTwo == true && currentDirection_1 != 0){
				currentDirection_2 = 2;
			}else{
				currentDirection_1 = 2;
			}
			doors[1].GetComponent<Animator>().SetBool("DoorOpen", true);
		}

		if((Input.GetButton("Right") && currentDirection_1 == 0) || (Input.GetButton("Right") && (canHandleTwo == true && currentDirection_2 == 0))){
			if(canHandleTwo == true && currentDirection_1 != 0){
				currentDirection_2 = 3;
			}else{
				currentDirection_1 = 3;
			}

			doors[2].GetComponent<Animator>().SetBool("DoorOpen", true);
		}
		// The player is still pressing the key
		if(currentDirection_1 == tempCurrentDirr_1){
			currentSortingDelay_1 -= Time.deltaTime;
		}else{
			currentSortingDelay_1 = upgradableSortingSpeed[currentSortingSpeed];
		}

		if(currentDirection_2 == tempCurrentDirr_2){
			currentSortingDelay_2 -= Time.deltaTime;
		}else{
			currentSortingDelay_2 = upgradableSortingSpeed[currentSortingSpeed];
		}

		if(currentDirection_1 != tempCurrentDirr_1){
			soundMan.PlayDoorEffect();
		}

		if(currentDirection_2 != tempCurrentDirr_2){
			soundMan.PlayDoorEffect();
		}


		if(currentDirection_1 != tempCurrentDirr_1 && tempCurrentDirr_1 != 0){
			doors[tempCurrentDirr_1-1].GetComponent<Animator>().SetBool("DoorOpen", false);
		}

		if(currentDirection_2 != tempCurrentDirr_2 && tempCurrentDirr_2 != 0){
			doors[tempCurrentDirr_2-1].GetComponent<Animator>().SetBool("DoorOpen", false);
		}
	}

	public bool CanHandleTwo {
		get {
			return canHandleTwo;
		}
	}

	public float GetCurrentSortingSpeed(){
		return upgradableSortingSpeed[currentSortingSpeed];
	}

	public float GetNextSortingSpeed(){
		if((currentSortingSpeed+2) <= upgradableSortingSpeed.Length){
			return upgradableSortingSpeed[currentSortingSpeed+1];
		}else{
			return -1;
		}
	}

	public int GetNextSortingSpeedCost(){
		if((currentSortingSpeed+1) < upgradableSortingSpeed.Length){
			return upgradableSortingSpeedCost[currentSortingSpeed];
		}else{
			return 0;
		}
	}

	public int GetHandleTwoCost(){
		if(canHandleTwo == false){
			return handleTwoCost;
		} else{
			return 0;
		}
	}

	public void UpgradeSorterSpeed(){
		if((currentSortingSpeed+2) <= upgradableSortingSpeed.Length){
			foreach(GameObject door in doors){
				door.GetComponent<Animator>().speed = 1f/upgradableSortingSpeed[currentSortingSpeed];
			}
			currentSortingSpeed++;
		}
	}

	public void UpgradeHandleTwo(){
		transportBelt_2.SetActive(true);
		canHandleTwo = true;
	}

	public void CreateGarbage(){
		spawnPosX = spawn_Area.transform.position.x + Random.Range(-spawn_Area.size.x/2, spawn_Area.size.x/2);
		spawnPosY = spawn_Area.transform.position.y + Random.Range(-spawn_Area.size.y/2, spawn_Area.size.y/2);
		GameObject garbage_GO = (GameObject)GameObject.Instantiate(garbage, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.Euler(new Vector3(0f,0f, Random.Range(0, 255f))));
		garbage_GO.name = "Garbage";
		soundMan.PlayGarbageEffect();
	}


	public Vector2 GetDirection(int type){
		soundMan.PlayEffect(9);
		// If we can only handle one resource, it follows the given direction
		if(currentDirection_1 != 0 && currentDirection_2 == 0 && currentSortingDelay_1 < 0){
			if(currentDirection_1 == 1){
				return resourceDirections[0];
			}else if(currentDirection_1 == 2){
				return resourceDirections[1];
			}else if(currentDirection_1 == 3){
				return resourceDirections[2];
			}
			// If we can handle two resources, it follows it's "designated" direction
		} else if(currentDirection_1 != 0 && currentDirection_2 != 0){
			if(type == currentDirection_1 && currentSortingDelay_1 < 0){
				return resourceDirections[currentDirection_1-1];
			}else if(type == currentDirection_2 && currentSortingDelay_2 < 0){
				return resourceDirections[currentDirection_2-1];
			}else if(currentSortingDelay_1 < 0 && currentSortingDelay_2 < 0){
				int random = Random.Range(0, 2);
				if(random == 1){return resourceDirections[currentDirection_1-1];}
				else{return resourceDirections[currentDirection_2-1];}
			} else if(currentSortingDelay_1 < 0){
				return resourceDirections[currentDirection_1-1];
			}else if(currentSortingDelay_2 < 0){
				return resourceDirections[currentDirection_2-1];
			}
		}
			
		return Vector2.zero;
	}
}
