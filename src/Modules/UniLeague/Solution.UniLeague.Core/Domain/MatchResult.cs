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


    private MatchResult() { } // EF 

    private MatchResult(int homeScore, int awayScore, IEnumerable<QuarterScore>? quarters, IEnumerable<SetScore>? sets)
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
    }

    //Koristiti za sportove bez četvrtina i setova (fudbal)
    public static MatchResult Create(int homeScore, int awayScore)
        => new(homeScore, awayScore, null, null);

    //Koristiti za sportove sa četvrtinama (košarka, američki fudbal).</summary>
    public static MatchResult CreateWithQuarters(int homeScore, int awayScore, IEnumerable<QuarterScore> quarters)
    {
        ArgumentNullException.ThrowIfNull(quarters);
        return new(homeScore, awayScore, quarters, null);
    }

    //Koristiti za sportove sa setovima (odbojka)
    public static MatchResult CreateWithSets(int homeScore, int awayScore, IEnumerable<SetScore> sets)
    {
        ArgumentNullException.ThrowIfNull(sets);
        return new(homeScore, awayScore, null, sets);
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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HomeScore;
        yield return AwayScore;
        foreach (var q in _quarters)
            yield return q;
    }

    public override string ToString() => $"{HomeScore}:{AwayScore}";
}