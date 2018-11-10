using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class JoinButton : MonoBehaviour
{
	private Text buttonText;
	private MatchInfoSnapshot match;

	private void Awake()
	{
		buttonText = GetComponentInChildren<Text>();
		GetComponent<Button>().onClick.AddListener(JoinMatch);
	}

	public void Initialize(MatchInfoSnapshot match)
	{
		this.match = match;
		buttonText.text = "Join: " + match.name;
	}

	private void JoinMatch()
	{
		FindObjectOfType<NetworkManager_Custom>().JoinMatch(match);
	}
}