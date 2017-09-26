using UnityEngine;
using System.Collections;

public class StatsManager : MonoBehaviour {

	private int currentFounds = 0;
	private int currentRating = 50;
	private int numberOfSucessfulSort = 0;
	private int numberOfMisstakeSort = 0;
	private int[,] sortedItemsData = new int[3,2];

	private GUIManager guiMan;
	private LevelManager levelMan;


	//DATA

	private int[] scoreLevels = new int[]{10, 20, 35, 45, 60, 75, 90, 110, 130, 150, 180, 210, 250};
	private string[] scoreLevel_Ratings_text = new string[]{"F", "E", "E+", "D", "D+", "C", "C+", "B", "B+", "A", "A+", "A++", "A+++"};

	// Use this for initialization
	void Start () {
		guiMan = GetComponent<GUIManager>();
		levelMan = GetComponent<LevelManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int NumberOfSuccessfulSort {
		get {
			return numberOfSucessfulSort;
		}
	}

	public int NumberOfMisstakeSort {
		get {
			return numberOfMisstakeSort;
		}
	}

	public int CurrentFounds {
		get {
			return currentFounds;
		}
	}

	public int CurrentRating {
		get {
			return currentRating;
		}
	}

	public void AddSuccessfulSort(int type){
		numberOfSucessfulSort++;
		sortedItemsData[type-1,0]++;
		guiMan.UpdateStatsWindow();
	}

	public void AddMisstakeSort(int type){
		numberOfMisstakeSort++;
		sortedItemsData[type-1,1]++;
		guiMan.UpdateStatsWindow();
	}

	public int CalculateWorstSortedItem(){
		int worstItem = -1;
		float tempWorstRatio = 0f;

		for (int i = 0; i < 3; i++) {
			float tempCheckRatio = 0f;
			if(sortedItemsData[i,1] > 0){
				tempCheckRatio = (float)sortedItemsData[i,1] / ((float)sortedItemsData[i,0] + (float)sortedItemsData[i,1]);
			}
		
			if(tempCheckRatio > tempWorstRatio){
				tempWorstRatio = tempCheckRatio;
				worstItem = i;
			}
		}
		return worstItem+1;
	}

	public int CalculateBestSortedItem(){
		int bestItem = -1;
		float tempBestRatio = 0f;

		for (int i = 0; i < 3; i++) {
			float tempCheckRatio = 0f;
			if(sortedItemsData[i,0] > 0){
				tempCheckRatio = (float)sortedItemsData[i,0] / ((float)sortedItemsData[i,0] + (float)sortedItemsData[i,1]);

			}

			if(tempCheckRatio > tempBestRatio){
				tempBestRatio = tempCheckRatio;
				bestItem = i;
			}
		}
		return bestItem+1;
	}


	public bool CheckIfValidPurchase(float cost){
		if(cost <= currentFounds){
			return true;
		}else{
			return false;
		}
	}

	public void BuyUpgrade(int cost){
		currentFounds -= cost;
		guiMan.UpdateWindowFunds();
		guiMan.UpdateMachineInfoText();
	}

	public void EndOfDayPayroll(int pay){
		currentFounds += pay;
		guiMan.UpdateWindowFunds();
		guiMan.UpdateMachineInfoText();
	}

	public string CalculatePerformenceRating(){
		for (int i = 0; i < scoreLevel_Ratings_text.Length-1; i++) {
			if(numberOfSucessfulSort < scoreLevels[i]){
				return scoreLevel_Ratings_text[i];
			}
		}

		if(numberOfSucessfulSort > scoreLevels[scoreLevels.Length-1]){
			return scoreLevel_Ratings_text[scoreLevel_Ratings_text.Length-1];
		}
		return "";
	}

	public void MisstakeRatingHit(){
		currentRating = Mathf.Clamp(currentRating - Mathf.FloorToInt(levelMan.DificultyFactor*15), 0, 100);

		if(currentRating == 0){
			guiMan.ShowLoseGameScreen(CalculatePerformenceRating());
		}
		guiMan.UpdateWindowFunds();
	}

	public void SuccesfulRating(){
		currentRating = Mathf.Clamp(currentRating + 1, 0, 100);
		guiMan.UpdateWindowFunds();
	}
}
