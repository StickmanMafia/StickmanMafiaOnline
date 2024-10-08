using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TournamentUIManager : MonoBehaviour
{
    public GameObject TournamentListPanel;
    public GameObject TournamentItemPrefab;

    private void Start()
    {
        PopulateTournamentList();
    }

    public void PopulateTournamentList()
    {
        foreach (Transform child in TournamentListPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var tournament in TournamentManager.Instance.ActiveTournaments)
        {
            GameObject item = Instantiate(TournamentItemPrefab, TournamentListPanel.transform);
            item.transform.Find("TournamentName").GetComponent<Text>().text = tournament.Name;
            item.transform.Find("EntryFee").GetComponent<Text>().text = $"Entry Fee: {tournament.EntryFee} SOL";
            item.transform.Find("PrizePool").GetComponent<Text>().text = $"Prize Pool: {tournament.PrizePool} SOL";

            Button joinButton = item.transform.Find("JoinButton").GetComponent<Button>();
            joinButton.onClick.AddListener(() => OnJoinTournament(tournament.TournamentId));
        }
    }

    public void OnJoinTournament(string tournamentId)
    {
        Player currentPlayer = new Player
        {
            PlayerId = "player123",
            Username = "PlayerOne",
            WalletAddress = "YourSolanaWalletAddress"
        };

        TournamentManager.Instance.RegisterPlayer(tournamentId, currentPlayer, (success) =>
        {
            if (success)
            {
                Debug.Log("Successfully registered for the tournament.");
            }
            else
            {
                Debug.LogError("Failed to register for the tournament.");
            }
        });
    }
}