using System.Collections.Generic;

public enum TournamentType
{
    Knockout,
    League,
    Leaderboard
}

public class Tournament
{
    public string TournamentId { get; set; }
    public string Name { get; set; }
    public TournamentType Type { get; set; }
    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }
    public List<Player> Participants { get; set; }
}
