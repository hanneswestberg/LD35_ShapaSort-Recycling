using UnityEngine;
using System.Collections;

public class ResourceScript : MonoBehaviour {

	public Resources_Spawner spawner;
	private bool hasBeenSorted = false;
	private Vector2 movementDirection = Vector2.zero;
	private StatsManager statsMan;

	public int type = 0;

	void Start(){
		statsMan = GameObject.Find("_GameManager").GetComponent<StatsManager>();
	}

	void Update () {
		TransportThis();
	}

	void TransportThis(){
		GetComponent<Rigidbody2D>().velocity = movementDirection * spawner.GetCurrentTransportSpeed();
	}

	public void SetMovementDirection(int dir){
		if(dir == 1){
			movementDirection = Vector2.up;
		}else if(dir == 2){
			movementDirection = Vector2.left;
		}else if(dir == 3){
			movementDirection = Vector2.right;
		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		if(hasBeenSorted == false && coll.tag == "Sorter"){
			this.transform.position = coll.transform.parent.transform.position;
			movementDirection = coll.transform.parent.GetComponent<MachineSorter>().GetDirection(type);

			if(movementDirection == Vector2.zero){
				coll.transform.parent.GetComponent<MachineSorter>().CreateGarbage();
				statsMan.MisstakeRatingHit();
				statsMan.AddMisstakeSort(type);
				Destroy(gameObject);
			}
		} else if(coll.tag == "Storage"){
			coll.transform.parent.GetComponent<MachineStorage>().StoreResource(this.gameObject);
		}
	}
}
