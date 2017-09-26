using UnityEngine;
using System.Collections;

public class Cash_PrefabScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(DestroyThis());
	}

	IEnumerator DestroyThis(){
		yield return new WaitForSeconds(2f);
		Destroy(gameObject);
	}
}
