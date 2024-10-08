using System;
using UnityEngine;

public class BlockchainService : MonoBehaviour
{
    public static BlockchainService Instance;

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

    public void CreateTournamentOnChain(string name, decimal entryFee, decimal prizePool, Action<bool, string> callback)
    {
        string tournamentId = Guid.NewGuid().ToString();
        callback?.Invoke(true, tournamentId);
    }

    public void DeductEntryFee(Player player, decimal amount, Action<bool> callback)
    {
        callback?.Invoke(true);
    }

    public void DistributePrizes(List<Player> winners, decimal prizePool, Action<bool> callback)
    {
        decimal prizePerWinner = prizePool / winners.Count;

        foreach (var player in winners)
        {
            // Implement transaction to transfer prize to player's wallet
        }

        callback?.Invoke(true);
    }
}