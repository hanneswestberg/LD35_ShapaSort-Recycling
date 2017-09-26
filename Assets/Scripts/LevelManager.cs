using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	[SerializeField] private GameObject[] truckPrefabs; 
	[SerializeField] private Transform[] trucksSpawnPos;
	[SerializeField] private GameObject nightImage;
	[SerializeField] private GameObject[] storageTrucks;
	[SerializeField] private MachineStorage[] storageMachines;
	[SerializeField] private GameObject machineSorter;
	[SerializeField] private GameObject cashPrefab;
	private StatsManager statsMan;
	private GUIManager guiMan;
	private SoundManager soundMan;


	[SerializeField] private float dayTime = 15f;
	[SerializeField] private float nightTime = 5f;

	public float DayTime {
		get {
			return dayTime;
		}
	}

	public float NightTime {
		get {
			return nightTime;
		}
	}

	private int baseResources = 10;
	private float dificultyFactor = 0.4f;

	public float DificultyFactor {
		get {
			return dificultyFactor;
		}
	}

	private int lastDaysResources;
	private int currentDayCount = 0;

	public int CurrentDayCount {
		get {
			return currentDayCount;
		}
	}

	private float countDownDayEnd = 0;
	private float countDownNightEnd = 0;

	public float CountDownNightEnd {
		get {
			return countDownNightEnd;
		}
	}

	public float CountDownDayEnd {
		get {
			return countDownDayEnd;
		}
	}

	[HideInInspector] public bool isDay = false;
	[HideInInspector] public bool gameIsOngoing = false;
	private bool dayIsStarting = false;
	private int totalDayResources;
	private int totalNextDayResources;

	public int TotalDayResources {
		get {
			return totalDayResources;
		}
	}

	public int TotalNextDayResources {
		get {
			return totalNextDayResources;
		}
	}

	void Start(){
		statsMan = GetComponent<StatsManager>();
		guiMan = GetComponent<GUIManager>();
		soundMan = GameObject.Find("_SoundManager").GetComponent<SoundManager>();
	}

	void Update(){
		if(isDay == true && gameIsOngoing){
			countDownDayEnd -= Time.deltaTime;

			nightImage.GetComponent<SpriteRenderer>().color = new Color(Mathf.Clamp((dayTime/2 +(countDownDayEnd * 2 * (255f/dayTime)))/255f, 0f, 1f), 
				Mathf.Clamp((dayTime/2 +(countDownDayEnd * 2 * (255f/dayTime)))/255f,0f, 1f), 1f, 0.2f);

			if(countDownDayEnd < 0 && gameIsOngoing){
				isDay = false;
				EndDay();
			}
		} else if(dayIsStarting == true && gameIsOngoing){
			nightImage.GetComponent<SpriteRenderer>().color = Color.Lerp(nightImage.GetComponent<SpriteRenderer>().color, new Color(1f, 1f, 1f, 0.2f), 0.03f);
			countDownNightEnd -= Time.deltaTime;
		}else if (gameIsOngoing){
			countDownNightEnd -= Time.deltaTime;
		}
	}

	public void SetDifficulty(int dif){
		if(dif == 1){
			dificultyFactor = 0.3f;
		}else if(dif == 2){
			dificultyFactor = 0.4f;
		}
	}


	public void StartNewDay(int dayCount){
		if(gameIsOngoing){
			currentDayCount = dayCount;
			soundMan.PlayEffect(7);
			guiMan.ShowDayText(dayCount+1);
			guiMan.UpdateStatsWindow();

			if(dayCount == 0){lastDaysResources = baseResources;}
			countDownDayEnd = dayTime;
			isDay = true;

			if(machineSorter.GetComponent<MachineSorter>().CanHandleTwo == false){
				Spawn1Truck(dayCount);
			}else{
				Spawn2Trucks(dayCount);
			}
		}
	}

	public void EndDay(){
		countDownNightEnd = nightTime;
		StartCoroutine(NightWait());
	}

	void CalculateTodaysResources(int day){
		totalDayResources = Mathf.RoundToInt(baseResources + (baseResources * dificultyFactor*day));
		totalNextDayResources = Mathf.RoundToInt(baseResources + (baseResources * dificultyFactor*(day+1)));
	}

	void Spawn1Truck(int day){
		int[] truckResources = new int[3];
		CalculateTodaysResources(day);
		guiMan.UpdateWindowFunds();

		for (int i = 0; i < totalDayResources; i++) {
			int addRes = Random.Range(0, 3);

			truckResources[addRes]++;
		}

		GameObject truck_1_GO = (GameObject)GameObject.Instantiate(truckPrefabs[0], trucksSpawnPos[0].position, Quaternion.Euler(new Vector3(0f, 0f, 90f)));
		truck_1_GO.GetComponent<Truck_Loaded>().SetLoadedResources(truckResources);
		truck_1_GO.name = "LoadedTruck_1_Day_" + day;
		truck_1_GO.transform.parent = this.transform;
		truck_1_GO.GetComponent<Truck_Loaded>().StartOfDayAnimation();
		truck_1_GO.GetComponent<Truck_Loaded>().type = 1;

		lastDaysResources = totalDayResources;
	}

	void Spawn2Trucks(int day){
		int[] truck_1Resources = new int[3];
		int[] truck_2Resources = new int[3];

		CalculateTodaysResources(day);
		guiMan.UpdateWindowFunds();

		for (int i = 0; i < totalDayResources; i++) {
			int addRes = Random.Range(0, 3);
			int randomTruck = Random.Range(0, 2);
			if(randomTruck == 1){
				truck_1Resources[addRes]++;
			}else{
				truck_2Resources[addRes]++;
			}
		}

		GameObject truck_1_GO = (GameObject)GameObject.Instantiate(truckPrefabs[0], trucksSpawnPos[0].position, Quaternion.Euler(new Vector3(0f, 0f, 90f)));
		truck_1_GO.GetComponent<Truck_Loaded>().SetLoadedResources(truck_1Resources);
		truck_1_GO.name = "LoadedTruck_1_Day_" + day;
		truck_1_GO.transform.parent = this.transform;
		truck_1_GO.GetComponent<Truck_Loaded>().StartOfDayAnimation();
		truck_1_GO.GetComponent<Truck_Loaded>().type = 1;

		GameObject truck_2_GO = (GameObject)GameObject.Instantiate(truckPrefabs[1], trucksSpawnPos[1].position, Quaternion.Euler(new Vector3(0f, 0f, 90f)));
		truck_2_GO.GetComponent<Truck_Loaded>().SetLoadedResources(truck_2Resources);
		truck_2_GO.name = "LoadedTruck_2_Day_" + day;
		truck_2_GO.transform.parent = this.transform;
		truck_2_GO.GetComponent<Truck_Loaded>().StartOfDayAnimation();
		truck_2_GO.GetComponent<Truck_Loaded>().type = 2;

		lastDaysResources = totalDayResources;
	}

	void CalculatePayroll(){
		int currentPay = 0;
		int amountSortedItems = 0;
		foreach(MachineStorage mStorage in storageMachines){
			amountSortedItems += mStorage.CurrentStorage;

			GameObject cash_GO = (GameObject)GameObject.Instantiate(cashPrefab, mStorage.transform.position, Quaternion.identity);
			cash_GO.transform.GetChild(0).GetComponent<TextMesh>().text = "<color=green>+" + (mStorage.CurrentStorage*100).ToString() + " cr</color>";
			cash_GO.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 50;
			mStorage.CurrentStorage = 0;
		}

		currentPay = amountSortedItems*100;
		statsMan.EndOfDayPayroll(currentPay);
		guiMan.UpdateStorageFillers();
		soundMan.PlayEffect(8);
	}

	IEnumerator NightWait(){
		foreach(GameObject truck in storageTrucks){
			truck.GetComponent<Animator>().SetTrigger("Spawn");
			soundMan.PlayTruckSound(3);
		}
		yield return new WaitForSeconds(1f);
		soundMan.PlayTruckSound(4);
		yield return new WaitForSeconds((nightTime/2f)-1f);

		foreach(GameObject truck in storageTrucks){
			truck.GetComponent<Animator>().SetTrigger("DeSpawn");
			soundMan.PlayTruckSound(5);
		}
		CalculatePayroll();

		yield return new WaitForSeconds((nightTime/2f)-1f);
		dayIsStarting = true;

		yield return new WaitForSeconds(1f);
		dayIsStarting = false;
		StartNewDay(currentDayCount+1);
	}
}
