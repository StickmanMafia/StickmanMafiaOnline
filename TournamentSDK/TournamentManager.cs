using UnityEngine;
using System;
using System.Collections.Generic;

public class TournamentManager : MonoBehaviour
{
    public static TournamentManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public List<Tournament> ActiveTournaments = new List<Tournament>();

    public void CreateTournament(string name, TournamentType type, decimal entryFee, decimal prizePool, Action<bool> callback)
    {
        BlockchainService.Instance.CreateTournamentOnChain(name, entryFee, prizePool, (success, tournamentId) =>
        {
            if (success)
            {
                Tournament newTournament = new Tournament
                {
                    TournamentId = tournamentId,
                    Name = name,
                    Type = type,
                    EntryFee = entryFee,
                    PrizePool = prizePool,
                    Participants = new List<Player>()
                };

                ActiveTournaments.Add(newTournament);
                callback?.Invoke(true);
            }
            else
            {
                callback?.Invoke(false);
            }
        });
    }

    public void RegisterPlayer(string tournamentId, Player player, Action<bool> callback)
    {
        Tournament tournament = ActiveTournaments.Find(t => t.TournamentId == tournamentId);

        if (tournament != null)
        {
            BlockchainService.Instance.DeductEntryFee(player, tournament.EntryFee, (success) =>
            {
                if (success)
                {
                    tournament.Participants.Add(player);
                    callback?.Invoke(true);
                }
                else
                {
                    callback?.Invoke(false);
                }
            });
        }
        else
        {
            callback?.Invoke(false);
        }
    }

    public void DistributePrizes(string tournamentId, List<Player> winners, Action<bool> callback)
    {
        Tournament tournament = ActiveTournaments.Find(t => t.TournamentId == tournamentId);

        if (tournament != null)
        {
            BlockchainService.Instance.DistributePrizes(winners, tournament.PrizePool, (success) =>
            {
                callback?.Invoke(success);
            });
        }
        else
        {
            callback?.Invoke(false);
        }
    }
}
