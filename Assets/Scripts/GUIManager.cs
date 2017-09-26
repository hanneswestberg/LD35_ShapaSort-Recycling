using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour {

	[SerializeField] private GameObject[] windowMachines;
	[SerializeField] private GameObject[] interMachines;
	[SerializeField] private Text[] windowFunds;
	[SerializeField] private Text[] windowStats;
	[SerializeField] private Text[] machineTexts;
	[SerializeField] private Image[] storageFillers;
	[SerializeField] private Text[] dayTexts;
	[SerializeField] private Text[] timeTexts;
	[SerializeField] private GameObject windowMain;
	[SerializeField] private Text[] windowLoose;
	[SerializeField] private GameObject[] tutorialWindows;
	[SerializeField] private GameObject[] audSrc;
	private SoundManager soundMan;
	private StatsManager statsMan;
	private LevelManager levelMan;
	private bool machineInfoIsShowing = false;
	private bool soundIsOn = true;

	// DATA
	private string[] looseText = new string[]{"\"I'm not gonna lie. That was some GOOD sorting\" \n - Dr. Mike Garbage", };



	void Start(){
		statsMan = gameObject.GetComponent<StatsManager>();
		levelMan = gameObject.GetComponent<LevelManager>();
		soundMan = GameObject.Find("_SoundManager").GetComponent<SoundManager>();
		windowMain.SetActive(true);
	}
		
	public void Update(){
		if(levelMan.isDay == true){
			timeTexts[0].text = "Time until Night:";
			timeTexts[1].text = levelMan.CountDownDayEnd.ToString("F1");
		}else if(levelMan.isDay == false){
			timeTexts[0].text = "Time until Day:";
			timeTexts[1].text = levelMan.CountDownNightEnd.ToString("F1");
		}

	}

	public void ShowTutorialWindow(int number){
		soundMan.PlayUIClick();
		tutorialWindows[number].SetActive(true);
		windowMain.SetActive(false);

		for (int i = 0; i < tutorialWindows.Length; i++) {
			if(i != number){
				tutorialWindows[i].SetActive(false);
			}
		}
	}

	public void CloseTutorialWindow(){
		for (int i = 0; i < tutorialWindows.Length; i++) {
			tutorialWindows[i].SetActive(false);
		}
		soundMan.PlayUIClick();
		windowMain.SetActive(true);

	}

	public void QuitApplication(){
		soundMan.PlayUIClick();
		Application.Quit();
	}

	public void RestartLevel(){
		soundMan.PlayUIClick();
		SceneManager.LoadScene("game");
	}

	public void ToggleAudio(){
		if(soundIsOn == true){
			foreach(GameObject aud in audSrc){aud.SetActive(false); soundIsOn = false;}
		}else{
			foreach(GameObject aud in audSrc){aud.SetActive(true); soundIsOn = true;}
		}
		soundMan.PlayUIClick();
	}

	public void StartGameOnDifficulty(int dif){
		levelMan.gameIsOngoing = true;
		windowMain.SetActive(false);
		levelMan.SetDifficulty(dif);
		levelMan.StartNewDay(0);
		soundMan.PlayUIClick();
	}
		

	public void UpdateStorageFillers(){
		for (int i = 2; i < 5; i++) {
			storageFillers[i-2].fillAmount = Mathf.Clamp(interMachines[i].GetComponent<MachineStorage>().CurrentStorage/interMachines[i].GetComponent<MachineStorage>().GetCurrentStorageCapacity(), 
				0.05f, 0.95f);
		}
	}

	public void ShowDayText(int day){
		dayTexts[0].text = "Day " + day.ToString();
		dayTexts[1].text = "Day " + day.ToString();
		dayTexts[0].transform.parent.GetComponent<Animator>().SetTrigger("PopIn");
	}

	public void ShowLoseGameScreen(string score){
		windowLoose[0].text = looseText[0];
		windowLoose[1].text = score;
		windowLoose[0].transform.parent.gameObject.SetActive(true);
		levelMan.gameIsOngoing = false;
	}


	public void ShowMachineInfo(int machine){
		if(machineInfoIsShowing == false){
			windowMachines[machine].SetActive(true);
			UpdateMachineInfoText();
			machineInfoIsShowing = true;
			soundMan.PlayUIClick();
		}
	}

	public void QuitMachineInfo(){
		for (int i = 0; i < windowMachines.Length; i++) {
			windowMachines[i].SetActive(false);
			machineInfoIsShowing = false;
		}
		soundMan.PlayUIClick();
	}

	public void UpdateStatsWindow(){
		windowStats[0].text =  "Day " + (levelMan.CurrentDayCount+1).ToString();
		windowStats[1].text =  statsMan.NumberOfSuccessfulSort.ToString();
		windowStats[2].text =  statsMan.NumberOfMisstakeSort.ToString();
		if(statsMan.CalculateBestSortedItem() == 1){
			windowStats[3].text = "Paper";
		}else if(statsMan.CalculateBestSortedItem() == 2){
			windowStats[3].text = "Glass";
		}else if(statsMan.CalculateBestSortedItem() == 3){
			windowStats[3].text = "Metal";
		}else{
			windowStats[3].text = "None";
		}

		if(statsMan.CalculateWorstSortedItem() == 1){
			windowStats[4].text = "Paper";
		}else if(statsMan.CalculateWorstSortedItem() == 2){
			windowStats[4].text = "Glass";
		}else if(statsMan.CalculateWorstSortedItem() == 3){
			windowStats[4].text = "Metal";
		}else{
			windowStats[4].text = "None";
		}
	}

	public void UpdateWindowFunds(){
		//Calls when something has updated
		windowFunds[0].text =  "<color=green>" + statsMan.CurrentFounds.ToString() + " cr" + "</color>";

		if(statsMan.CurrentRating <= 25){
			windowFunds[1].text = "<color=red>" + statsMan.CurrentRating.ToString() + " %" + "</color>";
		} else if(statsMan.CurrentRating <= 50){
			windowFunds[1].text = "<color=yellow>" + statsMan.CurrentRating.ToString() + " %" + "</color>";
		} else if(statsMan.CurrentRating <= 75){
			windowFunds[1].text = statsMan.CurrentRating.ToString() + " %";
		} else if(statsMan.CurrentRating > 75){
			windowFunds[1].text = "<color=green>" + statsMan.CurrentRating.ToString() + " %" + "</color>";
		}

		windowFunds[2].text = levelMan.TotalDayResources.ToString() + " items";
		windowFunds[3].text = levelMan.TotalNextDayResources.ToString() + " items";
	}

	public void UpgradeMachineTransport(int upgradeNr){
		if(upgradeNr == 1 && statsMan.CheckIfValidPurchase(interMachines[0].GetComponent<Resources_Spawner>().GetTransportationSpeedUpgradeCost())){
			statsMan.BuyUpgrade(interMachines[0].GetComponent<Resources_Spawner>().GetTransportationSpeedUpgradeCost());
			interMachines[0].GetComponent<Resources_Spawner>().UpgradeTransportSpeed();
		}else if(upgradeNr == 2 && statsMan.CheckIfValidPurchase(interMachines[0].GetComponent<Resources_Spawner>().GetUnloadSpeedUpgradeCost())){
			statsMan.BuyUpgrade(interMachines[0].GetComponent<Resources_Spawner>().GetUnloadSpeedUpgradeCost());
			interMachines[0].GetComponent<Resources_Spawner>().UpgradeUnloadSpeed();
		}
		soundMan.PlayUIClick();
		UpdateMachineInfoText();
	}

	public void UpgradeMachineSorter(int upgradeNr){
		if(upgradeNr == 1 && statsMan.CheckIfValidPurchase(interMachines[1].GetComponent<MachineSorter>().GetNextSortingSpeedCost())){
			statsMan.BuyUpgrade(interMachines[1].GetComponent<MachineSorter>().GetNextSortingSpeedCost());
			interMachines[1].GetComponent<MachineSorter>().UpgradeSorterSpeed();
		}else if(upgradeNr == 2 && statsMan.CheckIfValidPurchase(interMachines[1].GetComponent<MachineSorter>().GetHandleTwoCost())){
			statsMan.BuyUpgrade(interMachines[1].GetComponent<MachineSorter>().GetHandleTwoCost());
			interMachines[1].GetComponent<MachineSorter>().UpgradeHandleTwo();
		}
		soundMan.PlayUIClick();
		UpdateMachineInfoText();
	}

	public void UpgradeMachineStorage(int storageNr){
		if(storageNr == 1 && statsMan.CheckIfValidPurchase(interMachines[2].GetComponent<MachineStorage>().GetNextStorageCapacityCost())){
			statsMan.BuyUpgrade(interMachines[2].GetComponent<MachineStorage>().GetNextStorageCapacityCost());
			interMachines[2].GetComponent<MachineStorage>().UpgradeStorageCapacity();

		}else if(storageNr == 2 && statsMan.CheckIfValidPurchase(interMachines[3].GetComponent<MachineStorage>().GetNextStorageCapacityCost())){
			statsMan.BuyUpgrade(interMachines[3].GetComponent<MachineStorage>().GetNextStorageCapacityCost());
			interMachines[3].GetComponent<MachineStorage>().UpgradeStorageCapacity();

		} else if(storageNr == 3 && statsMan.CheckIfValidPurchase(interMachines[4].GetComponent<MachineStorage>().GetNextStorageCapacityCost())){
			statsMan.BuyUpgrade(interMachines[4].GetComponent<MachineStorage>().GetNextStorageCapacityCost());
			interMachines[4].GetComponent<MachineStorage>().UpgradeStorageCapacity();
		}
		soundMan.PlayUIClick();
		UpdateMachineInfoText();
	}

	public void UpdateMachineInfoText(){
		// MACHINE TransportSpeed
		machineTexts[0].text = interMachines[0].GetComponent<Resources_Spawner>().GetCurrentTransportSpeed().ToString() + " units/sec";

		if(interMachines[0].GetComponent<Resources_Spawner>().GetNextTransportSpeed() == 0){
			machineTexts[1].text = "<color=green>" + "MAX" + "</color>";
		}else{
			machineTexts[1].text = "<color=green>" + interMachines[0].GetComponent<Resources_Spawner>().GetNextTransportSpeed().ToString() + " units/sec" + "</color>";
		}

		// Unloading Speed
		machineTexts[2].text = interMachines[0].GetComponent<Resources_Spawner>().GetCurrentUnloadSpeed().ToString() + " sec/item";

		if(interMachines[0].GetComponent<Resources_Spawner>().GetNextUnloadSpeed() == 0){
			machineTexts[3].text = "<color=green>" + "MAX" + "</color>";
		}else{
			machineTexts[3].text = "<color=green>" + interMachines[0].GetComponent<Resources_Spawner>().GetNextUnloadSpeed().ToString() + " sec/item" + "</color>";
		}

		//Upgrade Cost

		if(interMachines[0].GetComponent<Resources_Spawner>().GetTransportationSpeedUpgradeCost() == 0){
			machineTexts[4].text = "<color=green>" + "MAX" + "</color>";
		}else{
			if(statsMan.CheckIfValidPurchase(interMachines[0].GetComponent<Resources_Spawner>().GetUnloadSpeedUpgradeCost())){
				machineTexts[4].text = "<color=green>" + interMachines[0].GetComponent<Resources_Spawner>().GetTransportationSpeedUpgradeCost().ToString()  + " cr" + "</color>";
			}else{
				machineTexts[4].text = "<color=red>" + interMachines[0].GetComponent<Resources_Spawner>().GetTransportationSpeedUpgradeCost().ToString() + " cr" + "</color>";
			}
		}

		if(interMachines[0].GetComponent<Resources_Spawner>().GetUnloadSpeedUpgradeCost() == 0){
			machineTexts[5].text = "<color=green>" + "MAX" + "</color>";
		}else{
			if(statsMan.CheckIfValidPurchase(interMachines[0].GetComponent<Resources_Spawner>().GetUnloadSpeedUpgradeCost())){
				machineTexts[5].text = "<color=green>" + interMachines[0].GetComponent<Resources_Spawner>().GetUnloadSpeedUpgradeCost().ToString()  + " cr" + "</color>";
			}else{
				machineTexts[5].text = "<color=red>" + interMachines[0].GetComponent<Resources_Spawner>().GetUnloadSpeedUpgradeCost().ToString()  + " cr" + "</color>";
			}
		}




		// MACHINE SortingSpeed
		machineTexts[6].text = interMachines[1].GetComponent<MachineSorter>().GetCurrentSortingSpeed().ToString() + " sec to open door";

		if(interMachines[1].GetComponent<MachineSorter>().GetNextSortingSpeed() == -1){
			machineTexts[7].text = "<color=green>" + "MAX" + "</color>";
		}else{
			machineTexts[7].text = "<color=green>" + interMachines[1].GetComponent<MachineSorter>().GetNextSortingSpeed().ToString() + " sec to open door" + "</color>";
		}

		if(interMachines[1].GetComponent<MachineSorter>().GetNextSortingSpeedCost() == 0){
			machineTexts[8].text = "<color=green>" + "MAX" + "</color>";
		}else{
			if(statsMan.CheckIfValidPurchase(interMachines[1].GetComponent<MachineSorter>().GetNextSortingSpeedCost())){
				machineTexts[8].text = "<color=green>" + interMachines[1].GetComponent<MachineSorter>().GetNextSortingSpeedCost().ToString()  + " cr" + "</color>";
			}else{
				machineTexts[8].text = "<color=red>" + interMachines[1].GetComponent<MachineSorter>().GetNextSortingSpeedCost().ToString()  + " cr" + "</color>";
			}
		}

		if(interMachines[1].GetComponent<MachineSorter>().GetHandleTwoCost() == 0){
			machineTexts[9].text = "<color=green>" + "MAX" + "</color>";
		}else{
			if(statsMan.CheckIfValidPurchase(interMachines[1].GetComponent<MachineSorter>().GetHandleTwoCost())){
				machineTexts[9].text = "<color=green>" + interMachines[1].GetComponent<MachineSorter>().GetHandleTwoCost().ToString()  + " cr" + "</color>";
			}else{
				machineTexts[9].text = "<color=red>" + interMachines[1].GetComponent<MachineSorter>().GetHandleTwoCost().ToString()  + " cr" + "</color>";
			}
		}



		// MACHINE StoragePaper
		machineTexts[10].text = interMachines[2].GetComponent<MachineStorage>().GetCurrentStorageCapacity().ToString() + " items";

		if(interMachines[2].GetComponent<MachineStorage>().GetNextStorageCapacity() == 0){
			machineTexts[11].text = "<color=green>" + "MAX" + "</color>";
		}else{
			machineTexts[11].text = "<color=green>" + interMachines[2].GetComponent<MachineStorage>().GetNextStorageCapacity().ToString() + " items" + "</color>";
		}

		if(interMachines[2].GetComponent<MachineStorage>().CurrentStorage < interMachines[2].GetComponent<MachineStorage>().GetCurrentStorageCapacity()){
			machineTexts[12].text = "<color=green>" + interMachines[2].GetComponent<MachineStorage>().CurrentStorage + "</color>";
		}else{
			machineTexts[12].text = "<color=red>" + interMachines[2].GetComponent<MachineStorage>().CurrentStorage + "</color>";
		}

		if(interMachines[2].GetComponent<MachineStorage>().GetNextStorageCapacityCost() == 0){
			machineTexts[13].text = "<color=green>" + "MAX" + "</color>";
		}else{
			if(statsMan.CheckIfValidPurchase(interMachines[2].GetComponent<MachineStorage>().GetNextStorageCapacityCost())){
				machineTexts[13].text = "<color=green>" + interMachines[2].GetComponent<MachineStorage>().GetNextStorageCapacityCost().ToString()  + " cr" + "</color>";
			}else{
				machineTexts[13].text = "<color=red>" + interMachines[2].GetComponent<MachineStorage>().GetNextStorageCapacityCost().ToString()  + " cr" + "</color>";
			}
		}



		// MACHINE StorageGlas
		machineTexts[14].text = interMachines[3].GetComponent<MachineStorage>().GetCurrentStorageCapacity().ToString() + " items";

		if(interMachines[3].GetComponent<MachineStorage>().GetNextStorageCapacity() == 0){
			machineTexts[15].text = "<color=green>" + "MAX" + "</color>";
		}else{
			machineTexts[15].text = "<color=green>" + interMachines[3].GetComponent<MachineStorage>().GetNextStorageCapacity().ToString() + " items" + "</color>";
		}

		if(interMachines[3].GetComponent<MachineStorage>().CurrentStorage < interMachines[3].GetComponent<MachineStorage>().GetCurrentStorageCapacity()){
			machineTexts[16].text = "<color=green>" + interMachines[3].GetComponent<MachineStorage>().CurrentStorage + "</color>";
		}else{
			machineTexts[16].text = "<color=red>" + interMachines[3].GetComponent<MachineStorage>().CurrentStorage + "</color>";
		}

		if(interMachines[3].GetComponent<MachineStorage>().GetNextStorageCapacityCost() == 0){
			machineTexts[17].text = "<color=green>" + "MAX" + "</color>";
		}else{
			if(statsMan.CheckIfValidPurchase(interMachines[3].GetComponent<MachineStorage>().GetNextStorageCapacityCost())){
				machineTexts[17].text = "<color=green>" + interMachines[3].GetComponent<MachineStorage>().GetNextStorageCapacityCost().ToString()  + " cr" + "</color>";
			}else{
				machineTexts[17].text = "<color=red>" + interMachines[3].GetComponent<MachineStorage>().GetNextStorageCapacityCost().ToString()  + " cr" + "</color>";
			}
		}

		// MACHINE StorageMetal
		machineTexts[18].text = interMachines[4].GetComponent<MachineStorage>().GetCurrentStorageCapacity().ToString() + " items";

		if(interMachines[4].GetComponent<MachineStorage>().GetNextStorageCapacity() == 0){
			machineTexts[19].text = "<color=green>" + "MAX" + "</color>";
		}else{
			machineTexts[19].text = "<color=green>" + interMachines[4].GetComponent<MachineStorage>().GetNextStorageCapacity().ToString() + " items" + "</color>";
		}

		if(interMachines[4].GetComponent<MachineStorage>().CurrentStorage < interMachines[4].GetComponent<MachineStorage>().GetCurrentStorageCapacity()){
			machineTexts[20].text = "<color=green>" + interMachines[4].GetComponent<MachineStorage>().CurrentStorage + "</color>";
		}else{
			machineTexts[20].text = "<color=red>" + interMachines[4].GetComponent<MachineStorage>().CurrentStorage + "</color>";
		}

		if(interMachines[4].GetComponent<MachineStorage>().GetNextStorageCapacityCost() == 0){
			machineTexts[21].text = "<color=green>" + "MAX" + "</color>";
		}else{
			if(statsMan.CheckIfValidPurchase(interMachines[4].GetComponent<MachineStorage>().GetNextStorageCapacityCost())){
				machineTexts[21].text = "<color=green>" + interMachines[4].GetComponent<MachineStorage>().GetNextStorageCapacityCost().ToString()  + " cr" + "</color>";
			}else{
				machineTexts[21].text = "<color=red>" + interMachines[4].GetComponent<MachineStorage>().GetNextStorageCapacityCost().ToString()  + " cr" + "</color>";
			}
		}
	}
}
