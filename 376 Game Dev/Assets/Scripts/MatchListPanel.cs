using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;

public class MatchListPanel : MonoBehaviour
{
	[SerializeField] private JoinButton joinButtonPrefab;

    public void updateList(List<MatchInfoSnapshot> matches)
    {
		ClearExistingButtons();
		CreateNewJoinGameButtons(matches);
	}

	private void ClearExistingButtons()
	{
		var buttons = GetComponentsInChildren<JoinButton>();
		foreach (var button in buttons)
		{
			Destroy(button.gameObject);
		}
	}

	private void CreateNewJoinGameButtons(List<MatchInfoSnapshot> matches)
	{
		foreach (var match in matches)
		{
			var button = Instantiate(joinButtonPrefab, transform);
			button.Initialize(match);
		}
	}
}