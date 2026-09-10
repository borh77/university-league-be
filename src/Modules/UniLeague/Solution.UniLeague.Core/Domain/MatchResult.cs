using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class MatchResult : ValueObject
{
    public int HomeScore { get; }
    public int AwayScore { get; }

    private readonly List<QuarterScore> _quarters = new();
    public IReadOnlyList<QuarterScore> Quarters => _quarters.AsReadOnly();
    public bool HasQuarters => _quarters.Count > 0;

    private readonly List<SetScore> _sets = new();
    public IReadOnlyList<SetScore> Sets => _sets.AsReadOnly();
    public bool HasSets => _sets.Count > 0;

    private readonly List<GoalEvent> _goals = new();
    public IReadOnlyList<GoalEvent> Goals => _goals.AsReadOnly();
    public bool HasGoals => _goals.Count > 0;

    private readonly List<PlayerStatLine> _playerStats = new();
    public IReadOnlyList<PlayerStatLine> PlayerStats => _playerStats.AsReadOnly();
    public bool HasPlayerStats => _playerStats.Count > 0;

    private MatchResult() { } // EF

    private MatchResult(
        int homeScore, int awayScore,
        IEnumerable<QuarterScore>? quarters,
        IEnumerable<SetScore>? sets,
        IEnumerable<GoalEvent>? goals,
        IEnumerable<PlayerStatLine>? playerStats)
    {
        ValidateNonNegative(homeScore, nameof(homeScore));
        ValidateNonNegative(awayScore, nameof(awayScore));

        HomeScore = homeScore;
        AwayScore = awayScore;

        if (quarters is not null)
        {
            var list = quarters.OrderBy(q => q.QuarterNumber).ToList();
            ValidateQuarters(list, homeScore, awayScore);
            _quarters = list;
        }

        if (sets is not null)
        {
            var list = sets.OrderBy(s => s.SetNumber).ToList();
            ValidateSets(list, homeScore, awayScore);
            _sets = list;
        }

        if (goals is not null)
        {
            var list = goals.OrderBy(g => g.Minute).ToList();
            ValidateGoals(list, homeScore, awayScore);
            _goals = list;
        }

        if (playerStats is not null)
        {
            var list = playerStats.OrderBy(p => p.IsHomeTeam ? 0 : 1).ThenBy(p => p.JerseyNumber).ToList();
            ValidatePlayerStats(list, homeScore, awayScore, _sets);
            _playerStats = list;
        }
    }

    //Koristiti za sportove bez četvrtina i setova (fudbal)
    public static MatchResult Create(int homeScore, int awayScore)
        => new(homeScore, awayScore, null, null, null, null);

    //Koristiti za sportove sa četvrtinama (košarka, američki fudbal)
    public static MatchResult CreateWithQuarters(int homeScore, int awayScore, IEnumerable<QuarterScore> quarters)
    {
        ArgumentNullException.ThrowIfNull(quarters);
        return new(homeScore, awayScore, quarters, null, null, null);
    }

    //Koristiti za sportove sa setovima (odbojka)
    public static MatchResult CreateWithSets(int homeScore, int awayScore, IEnumerable<SetScore> sets)
    {
        ArgumentNullException.ThrowIfNull(sets);
        return new(homeScore, awayScore, null, sets, null, null);
    }

    //Koristiti za fudbalske utakmice sa listom golova i strelaca
    public static MatchResult CreateWithGoals(int homeScore, int awayScore, IEnumerable<GoalEvent> goals)
    {
        ArgumentNullException.ThrowIfNull(goals);
        return new(homeScore, awayScore, null, null, goals, null);
    }

    //Koristiti za sportove sa poenima po igraču (košarka + četvrtine, odbojka + setovi)
    public static MatchResult CreateWithPlayerStats(
        int homeScore, int awayScore,
        IEnumerable<PlayerStatLine> playerStats,
        IEnumerable<QuarterScore>? quarters = null,
        IEnumerable<SetScore>? sets = null)
    {
        ArgumentNullException.ThrowIfNull(playerStats);
        return new(homeScore, awayScore, quarters, sets, null, playerStats);
    }

    public bool IsDraw() => HomeScore == AwayScore;
    public bool HomeWon() => HomeScore > AwayScore;
    public bool AwayWon() => AwayScore > HomeScore;

    private static void ValidateNonNegative(int value, string name)
    {
        if (value < 0)
            throw new ArgumentException($"{name} cannot be negative.", name);
    }

    private static void ValidateQuarters(List<QuarterScore> quarters, int totalHome, int totalAway)
    {
        if (quarters.Count == 0)
            throw new ArgumentException("Quarters list cannot be empty when provided.");

        for (int i = 0; i < quarters.Count; i++)
        {
            if (quarters[i].QuarterNumber != i + 1)
                throw new ArgumentException(
                    $"Quarter numbers must be sequential starting from 1. " +
                    $"Expected {i + 1}, got {quarters[i].QuarterNumber}.");
        }

        var sumHome = quarters.Sum(q => q.HomeScore);
        var sumAway = quarters.Sum(q => q.AwayScore);

        if (sumHome != totalHome)
            throw new ArgumentException(
                $"Sum of home quarter scores ({sumHome}) does not match total home score ({totalHome}).");
        if (sumAway != totalAway)
            throw new ArgumentException(
                $"Sum of away quarter scores ({sumAway}) does not match total away score ({totalAway}).");
    }

    private static void ValidateSets(List<SetScore> sets, int totalHomeSets, int totalAwaySets)
    {
        if (sets.Count == 0)
            throw new ArgumentException("Sets list cannot be empty when provided.");

        for (int i = 0; i < sets.Count; i++)
        {
            if (sets[i].SetNumber != i + 1)
                throw new ArgumentException(
                    $"Set numbers must be sequential starting from 1. " +
                    $"Expected {i + 1}, got {sets[i].SetNumber}.");
        }

        var setsWonHome = sets.Count(s => s.HomeWonSet());
        var setsWonAway = sets.Count(s => s.AwayWonSet());

        if (setsWonHome != totalHomeSets)
            throw new ArgumentException(
                $"Number of sets won by home team ({setsWonHome}) does not match home score ({totalHomeSets}).");
        if (setsWonAway != totalAwaySets)
            throw new ArgumentException(
                $"Number of sets won by away team ({setsWonAway}) does not match away score ({totalAwaySets}).");
    }

    private static void ValidateGoals(List<GoalEvent> goals, int totalHome, int totalAway)
    {
        if (goals.Count == 0)
            throw new ArgumentException("Goals list cannot be empty when provided.");

        var homeGoals = goals.Count(g => g.IsHomeTeamGoal);
        var awayGoals = goals.Count(g => !g.IsHomeTeamGoal);

        if (homeGoals != totalHome)
            throw new ArgumentException(
                $"Number of home goals ({homeGoals}) does not match home score ({totalHome}).");
        if (awayGoals != totalAway)
            throw new ArgumentException(
                $"Number of away goals ({awayGoals}) does not match away score ({totalAway}).");
    }

    private static void ValidatePlayerStats(
        List<PlayerStatLine> stats, int totalHome, int totalAway, List<SetScore> sets)
    {
        if (stats.Count == 0)
            throw new ArgumentException("Player stats list cannot be empty when provided.");

        // Odbojka: timski skor za poene je zbir poena po setovima, a ne broj osvojenih setova
        var expectedHome = sets.Count > 0 ? sets.Sum(s => s.HomeScore) : totalHome;
        var expectedAway = sets.Count > 0 ? sets.Sum(s => s.AwayScore) : totalAway;

        var sumHome = stats.Where(p => p.IsHomeTeam).Sum(p => p.Points);
        var sumAway = stats.Where(p => !p.IsHomeTeam).Sum(p => p.Points);

        if (sumHome != expectedHome)
            throw new ArgumentException(
                $"Sum of home player points ({sumHome}) does not match home team score ({expectedHome}).");
        if (sumAway != expectedAway)
            throw new ArgumentException(
                $"Sum of away player points ({sumAway}) does not match away team score ({expectedAway}).");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HomeScore;
        yield return AwayScore;
        foreach (var q in _quarters) yield return q;
        foreach (var s in _sets) yield return s;
        foreach (var g in _goals) yield return g;
        foreach (var p in _playerStats) yield return p;
    }

    public override string ToString() => $"{HomeScore}:{AwayScore}";
}