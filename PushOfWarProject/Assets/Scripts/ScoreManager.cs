using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	public float p1health = 100f;
	public float p2health = 100f;
	public Text p1healthText;
	public Text p2healthText;
	public int p1money = 100;
	public int p2money = 100;
	public Text p1moneyText;
	public Text p2moneyText;

	void UpdateUI () {
		p1healthText.text = "P1 HP: " + p1health;
		p2healthText.text = "P2 HP: " + p2health;
		p1moneyText.text = "P1 Money: " + p1money;
		p2moneyText.text = "P2 Money: " + p2money;
	}
	
	void Update () {
		UpdateUI();
	}
}
