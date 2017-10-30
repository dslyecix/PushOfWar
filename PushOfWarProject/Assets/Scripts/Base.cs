using UnityEngine;
using UnityEngine.SceneManagement;

public class Base : MonoBehaviour {

	public bool playerTwo;
	public GameObject unitPrefab;
	public Transform spawnPoint;
	public Transform waitZone;
	public Transform enemyBase;
	public GameObject unitParent;

	public Path path1, path2, path3;
	public Path currentPath;	

	public float hitPoints;
	ScoreManager sm;

	public Path selectedPath;

	void Start () {
		sm = FindObjectOfType<ScoreManager>();

		//Pass the appropriate references on to the path objects
		if (!playerTwo) {
			path1.playerOneBase = this.transform;
			path2.playerOneBase = this.transform;
			path3.playerOneBase = this.transform;
		} else {
			path1.playerTwoBase = this.transform;
			path2.playerTwoBase = this.transform;
			path3.playerTwoBase = this.transform;
		}

		selectedPath = path1;
	}

	void Update () {
		bool a = Input.GetKeyDown(KeyCode.Alpha1);
		bool b = Input.GetKeyDown(KeyCode.Alpha2);
		bool c = Input.GetKeyDown(KeyCode.Alpha3);

		if (a) SpawnUnit();
		if (b) SpawnUnit();
		if (c) SpawnUnit();
	}

	public void TakeDamage (float damage){
		ScoreManager sm = FindObjectOfType<ScoreManager>();
		hitPoints -= damage;
		if (!playerTwo){
			sm.p1health = hitPoints;
		} else { 
			sm.p2health = hitPoints;
		}
		if (hitPoints <= 0) {
			BaseDestroyed();
		}
	
	}

	void BaseDestroyed () {
		Debug.Log("GameOver, " + transform.name + " has been destroyed");
		Destroy(gameObject);
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadSceneAsync(scene.buildIndex);
	}

	public void SpawnUnit () {
		if ((playerTwo?sm.p2money:sm.p1money) >= unitPrefab.GetComponent<Unit>().cost) {
			GameObject go = Instantiate(unitPrefab,spawnPoint.position,Quaternion.identity);
			Unit goUnit = go.GetComponent<Unit>();

			goUnit.playerBase = this.transform;
			goUnit.enemyBase = enemyBase;
			goUnit.playerTwo = playerTwo;
			goUnit.waitZone = waitZone;
			goUnit.targetPath = selectedPath;
			go.transform.parent = unitParent.transform;

			if (!playerTwo){
				sm.p1money -= goUnit.cost;
			} else {
				sm.p2money -= goUnit.cost;
			}
		}
		else {
			Debug.Log("Ya can't afford shit mate");
		}
	}

	public void SetSelectedPath (Path path) {
		selectedPath = path;

	}

}
