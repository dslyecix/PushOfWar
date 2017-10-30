using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {

	[SerializeField]
	List<Transform> pathNodes;

	public Transform playerOneBase, playerTwoBase;

	void Start () {
		//Load up reference to your nodes
		for (int i = 0; i < transform.childCount; i++)
		{
			pathNodes.Add(transform.GetChild(i));
		}
	}

	public Transform ReturnNextNodePosition (Transform currentNode, Unit unit) {
		int currentNodeIndex = GetIndexOfCurrentNode(currentNode, unit); //-1 if not found, -2 if targetting enemy base 
		int nextNodeIndex = GetIndexOfNextNode(currentNodeIndex, unit);
		if (nextNodeIndex == -2) {
			if (!unit.playerTwo){
				return playerTwoBase;
			} else {
				return playerOneBase;
			}
		} else {
			return pathNodes[nextNodeIndex];
		}
	}


	int GetIndexOfCurrentNode (Transform currentNode, Unit unit) {
		for (int i = 0; i < pathNodes.Count; i++) {
			if (currentNode == pathNodes[i]) {
				return i;
			}
		}

		if (currentNode == unit.enemyBase) {
			return -2;
		}

		return -1;
	}

	int GetIndexOfNextNode (int currentNodeIndex, Unit unit) {
		if (currentNodeIndex == -1) {
			if (!unit.playerTwo) {
				return 0;
			} else {
				return pathNodes.Count - 1;
			}
		}
		if (!unit.playerTwo) {
			if (currentNodeIndex < pathNodes.Count - 1) { 
				return currentNodeIndex + 1;
			} else {
				return -2;
			}				
		} else {
			if (currentNodeIndex > 0) { 
				return currentNodeIndex - 1;
			} else {
				return -2;
			}	
		}
	}

}
