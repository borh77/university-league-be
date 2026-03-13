using Solution.BuildingBlocks.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class MatchResult : ValueObject
{
    public int HomeScore { get; }
    public int AwayScore { get; }

    private readonly List<QuarterScore> _quarters = new();
    public IReadOnlyList<QuarterScore> Quarters => _quarters.AsReadOnly();
    public bool HasQuarters => _quarters.Count > 0;

    private MatchResult() { } // EF 

    private MatchResult(int homeScore, int awayScore, IEnumerable<QuarterScore>? quarters)
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
    }

    //Koristiti za sportove bez četvrtina (fudbal, odbojka)
    public static MatchResult Create(int homeScore, int awayScore)
        => new(homeScore, awayScore, null);

    //Koristiti za sportove sa četvrtinama (košarka, američki fudbal).</summary>
    public static MatchResult CreateWithQuarters(int homeScore, int awayScore, IEnumerable<QuarterScore> quarters)
    {
        ArgumentNullException.ThrowIfNull(quarters);
        return new(homeScore, awayScore, quarters);
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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HomeScore;
        yield return AwayScore;
        foreach (var q in _quarters)
            yield return q;
    }

    public override string ToString() => $"{HomeScore}:{AwayScore}";
}