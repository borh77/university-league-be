namespace Solution.UniLeague.Core.Domain;

public record MatchTeamSnapshot(
    long TeamId,
    string TeamName,
    string TeamLogoUrl,
    int? Seed);
