using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class TournamentController : MonoBehaviour
{
    public GameObject TournamentListPanel;
    public GameObject TournamentItemPrefab;
    public InputField TournamentNameInput;
    public InputField EntryFeeInput;
    public InputField PrizePoolInput;
    public Button CreateTournamentButton;
    public Button JoinTournamentButton;
    public Button LeaveTournamentButton;
    public Button WinTournamentButton;
    public Text StatusText;

    private void Start()
    {
        CreateTournamentButton.onClick.AddListener(OnCreateTournament);
        JoinTournamentButton.onClick.AddListener(OnJoinTournament);
        LeaveTournamentButton.onClick.AddListener(OnLeaveTournament);
        WinTournamentButton.onClick.AddListener(OnWinTournament);

        PopulateTournamentList();
    }

    private void PopulateTournamentList()
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

    private void OnCreateTournament()
    {
        string name = TournamentNameInput.text;
        decimal entryFee = decimal.Parse(EntryFeeInput.text);
        decimal prizePool = decimal.Parse(PrizePoolInput.text);

        TournamentManager.Instance.CreateTournament(name, TournamentType.Knockout, entryFee, prizePool, (success) =>
        {
            if (success)
            {
                StatusText.text = "Tournament created successfully.";
                PopulateTournamentList();
            }
            else
            {
                StatusText.text = "Failed to create tournament.";
            }
        });
    }

    private void OnJoinTournament(string tournamentId)
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
                StatusText.text = "Successfully registered for the tournament.";
            }
            else
            {
                StatusText.text = "Failed to register for the tournament.";
            }
        });
    }

    private void OnLeaveTournament()
    {
        string tournamentId = "tournamentId123"; // Replace with actual tournament ID
        Player currentPlayer = new Player
        {
            PlayerId = "player123",
            Username = "PlayerOne",
            WalletAddress = "YourSolanaWalletAddress"
        };

        TournamentManager.Instance.LeaveTournament(tournamentId, currentPlayer, (success) =>
        {
            if (success)
            {
                StatusText.text = "Successfully left the tournament.";
            }
            else
            {
                StatusText.text = "Failed to leave the tournament.";
            }
        });
    }

    private void OnWinTournament()
    {
        string tournamentId = "tournamentId123"; // Replace with actual tournament ID
        Player currentPlayer = new Player
        {
            PlayerId = "player123",
            Username = "PlayerOne",
            WalletAddress = "YourSolanaWalletAddress"
        };
        int moneyGet = 100; // Replace with actual amount

        TournamentManager.Instance.WinTournament(tournamentId, currentPlayer, moneyGet, (success) =>
        {
            if (success)
            {
                StatusText.text = "Successfully won the tournament.";
            }
            else
            {
                StatusText.text = "Failed to win the tournament.";
            }
        });
    }
}
