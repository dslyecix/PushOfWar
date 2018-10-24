using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

	public enum UnitState {onPath, attacking}

	[SerializeField] UnitState state;

	public LayerMask enemyLayer;

	Renderer renderer;
	Animator animator;
	NavMeshAgent navMeshAgent;
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
		navMeshAgent.SetDestination(waitZone.position);

		readyToStartPath = false; //Will wait at the first start zone reached
		attackTimer = 0; //Make it so when we first attack we don't have to wait for CD
		enemySearchTimer = 0;

		currentPathTarget = waitZone;

		state = UnitState.onPath;
	}
	
	void Update ()
    {

        switch (state)
        {
            case UnitState.onPath:

                if (GetDistanceTo(currentPathTarget) <= aggroRange)
                {
					Debug.Log("Close to destination");
					if (readyToStartPath)  {
						Debug.Log("Getting new path target");
                    	currentPathTarget = targetPath.ReturnNextNodePosition(currentPathTarget, this);
					} else {
						Debug.Log("Waiting to be released");
						if (Input.GetKeyDown(KeyCode.Space)) {
							Debug.Log("Released!");
							readyToStartPath = true;
						}
					}
                }
                else
                {                 
					Debug.Log("Moving towards destination");
                    navMeshAgent.SetDestination(currentPathTarget.position);
                }

				SearchForEnemies();

                break;

            case UnitState.attacking:

				SearchForEnemies();

                if (targetEnemy == null)
                {
                    Debug.Log("No more enemies found, reverting to path");
                    navMeshAgent.SetDestination(currentPathTarget.position);
                    state = UnitState.onPath;
                }
                else if (GetDistanceTo(targetEnemy) <= attackRange)
                {
                    if (attackTimer > 0f)
                    {
                        attackTimer -= Time.deltaTime;
                    }
                    else
                    {
                        Debug.Log("Attacking " + targetEnemy.name);
                        Attack(targetEnemy);
                        attackTimer = attackTimeCooldown;
                        //isAttacking = false;
					}
                }
                else {
					Debug.Log("Stuck here");
                    navMeshAgent.SetDestination(targetEnemy.position); 
                }
                break;
        }
    }

    private float GetDistanceTo(Transform targetTransform)
    {
        return (transform.position - targetTransform.position).magnitude;
    }

    public void SearchForEnemies() {
		if (enemySearchTimer > 0) {	
			enemySearchTimer -= Time.deltaTime;
		} else {
			enemySearchTimer = enemySearchTime;
			enemies = null;
			enemies = Physics.OverlapSphere(transform.position,aggroRange,enemyLayer);
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
			} else {
				targetEnemy = null;
			}

			if (targetEnemy == null) {	
				state = UnitState.onPath;
			} else {	
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
