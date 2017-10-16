using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

	public enum UnitState {onPath, attacking}

	[SerializeField]
	UnitState state;

	public LayerMask enemyLayer;

	Renderer renderer;
	Animator animator;
	public NavMeshAgent navMeshAgent;
	Transform currentPathTarget;
	public Transform waitZone;
	public Transform playerBase;
	public Transform enemyBase;

	public bool playerTwo;
	public bool readyToStartPath;

	public float moveSpeed = 3f;
	public float attackRange = 1f;
	public float aggroRange = 4f;

	public int cost;

	public Path targetPath;

	public Transform targetEnemy;

	public float damageAmount;
	public float attackTimeCooldown;
	float attackTimer;
	//bool isAttacking; //bool for animation trigger

	public float hitPoints;

	public float enemySearchTime;
	float enemySearchTimer;
	Collider[] enemies;

	void Start () {
		animator = GetComponent<Animator>();
		renderer = GetComponent<Renderer>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		navMeshAgent.speed = moveSpeed;
		navMeshAgent.acceleration = 300f;
		navMeshAgent.stoppingDistance = attackRange;

		readyToStartPath = false; //Will wait at the first start zone reached
		attackTimer = 0; //Make it so when we first attack we don't have to wait for CD
		enemySearchTimer = 0;

		currentPathTarget = waitZone;

		state = UnitState.onPath;
	}
	
	void Update () {

		switch (state)
		{
			case UnitState.onPath: 
				if (!readyToStartPath) { 																				//Unit needs to reach the waitzone before it can embark
					navMeshAgent.SetDestination(waitZone.position);

					SearchForEnemies();

					if ((transform.position - waitZone.position).sqrMagnitude < (aggroRange * aggroRange) && Input.GetKeyDown(KeyCode.Space)){//We get within 5 units and initiate embark
						readyToStartPath = true;
						if (currentPathTarget == enemyBase) {
							Debug.Log("End of the line");
						} else {
							currentPathTarget = targetPath.ReturnNextNodePosition(null,playerTwo);
						}
					}
				} else if ((transform.position - currentPathTarget.position).sqrMagnitude < aggroRange * aggroRange) {						//If less than 5 units away, get new path target
					currentPathTarget = targetPath.ReturnNextNodePosition(currentPathTarget,playerTwo); 
					Debug.Log("Setting path to " + currentPathTarget);
				} else {																								//Otherwise, get new path target
					navMeshAgent.SetDestination(currentPathTarget.position);
				}

				SearchForEnemies();
				
				break;
			

			case UnitState.attacking:

				SearchForEnemies();

				if (targetEnemy == null) {
					Debug.Log("No more enemies found, reverting to path");
					navMeshAgent.SetDestination(currentPathTarget.position);
					state = UnitState.onPath;
					break;
				} else if ((transform.position - targetEnemy.position).sqrMagnitude > (attackRange * attackRange)) {
					navMeshAgent.SetDestination(targetEnemy.position);													//TODO: add ReturnOpenAttackSpot() to figure out nice sorting
				} else {
					if (attackTimer > 0f) {
						attackTimer -= Time.deltaTime;
					} else {
						Debug.Log ("Attacking " + targetEnemy.name);
						Attack(targetEnemy);
						attackTimer = attackTimeCooldown;
						//isAttacking = false;
					}
				}
				break;
		}
	}
			

	public void SearchForEnemies() {
		if (enemySearchTimer > 0) {																			
			enemySearchTimer -= Time.deltaTime;
		} else {
			enemySearchTimer = enemySearchTime;
			enemies = null;
			enemies = Physics.OverlapSphere(transform.position,aggroRange,enemyLayer); //Try to find nearby enemies
			float dist = float.MaxValue;
			if (enemies != null) {
				foreach (Collider e in enemies)	{
					float newDist = (e.transform.position - transform.position).magnitude;
					if (newDist < dist) {
						dist = newDist;
						targetEnemy = e.transform;
						Debug.Log(transform.name + " targetting " + e.name);
					}
				}
			}

			if (targetEnemy == null) {																	//If none are found, return to path
				Debug.Log("Changing state: following path");
				navMeshAgent.SetDestination(currentPathTarget.position);
				state = UnitState.onPath;
			} else {	
				navMeshAgent.SetDestination(targetEnemy.position);
				state = UnitState.attacking;
			}			
		}
		
	}

	// public IEnumerator ChangeColour (Color colour) {
	// 	if (isAttacking = true) {
	// 		Color originalColour = renderer.material.color;
	// 		renderer.material.color = Color.green;
	// 		yield return new WaitForSeconds(0.2f);
	// 		isAttacking = false;
	// 		renderer.material.color = originalColour;
	// 	}		
	// }


	public void Attack (Transform target) {
		//isAttacking = true;
		// StartCoroutine(ChangeColour(Color.green));
		if (targetEnemy == enemyBase) target.GetComponent<Base>().TakeDamage(damageAmount);		
		else target.GetComponent<Unit>().TakeDamage(damageAmount);		
	}
	

	public void AttackBase (Transform target) {
		
		if (attackTimer > 0f) {
			attackTimer -= Time.deltaTime;
		} else {
			Debug.Log(transform.name + " attacking " + currentPathTarget.name);
			target.GetComponent<Base>().TakeDamage(damageAmount);
			attackTimer = attackTimeCooldown;
		}
	}
	
	void TakeDamage (float damage){
		//StartCoroutine(ChangeColour(Color.red));

		hitPoints -= damage;
		if (hitPoints <= 0) {
			Die();
		}
	}

	void Die () {
		Debug.Log(transform.name + " died");
		ScoreManager sm = FindObjectOfType<ScoreManager>();
			if (!playerTwo){
				sm.p2money += 6;
			} else {
				sm.p1money += 6;
			}
		Destroy(gameObject);
	}
}
