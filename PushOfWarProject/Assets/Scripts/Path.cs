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

	public Transform ReturnNextNodePosition (Transform currentNode, bool playerTwo) {
		if (currentNode == null) { 						// Unit passes in 'null', meaning they are just starting their journey
			if (!playerTwo) {
				return pathNodes[0];
			} else {
				return pathNodes[pathNodes.Count - 1];
			}
		} else {										// Unit passes in an existing node, figure out the next one in the path
			for (int i = 0; i < pathNodes.Count; i++)
			{
				if (currentNode == pathNodes[i]) {
					if (!playerTwo){
						if (i < pathNodes.Count - 1) { 
							return pathNodes[i + 1];
						} else {						//a PlayerOne unit has reached the end of its path, return enemy base
							return playerTwoBase;
						}
					} else if (i > 0) {  
						return pathNodes[i - 1];
					} else {							//a PlayerOne unit has reached the end of its path, return enemy base
						return playerOneBase;
					}
				}
			}
		}
		//There is no such node on the path!
		return null;
	}
}
