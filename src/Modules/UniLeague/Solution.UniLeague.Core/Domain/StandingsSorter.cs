using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Core.Domain;

public class StandingsSorter
{
    public IReadOnlyList<StandingEntry> Sort(
        Sport sport,
        IReadOnlyCollection<StandingEntry> standings,
        IReadOnlyList<Match> matches)
    {
        var byPoints = standings
            .OrderByDescending(s => s.Points)
            .ToList();

        var result = new List<StandingEntry>();
        var i = 0;

        while (i < byPoints.Count)
        {
            var currentPoints = byPoints[i].Points;
            var group = byPoints
                .Skip(i)
                .TakeWhile(s => s.Points == currentPoints)
                .ToList();

            result.AddRange(group.Count == 1
                ? group
                : SortGroupByH2H(sport, group, matches));

            i += group.Count;
        }

        return result.AsReadOnly();
    }

    private List<StandingEntry> SortGroupByH2H(
        Sport sport,
        List<StandingEntry> group,
        IReadOnlyList<Match> allMatches)
    {
        var teamIds = group.Select(s => (long)s.TeamId).ToHashSet();

        var h2hMatches = allMatches
            .Where(m => teamIds.Contains(m.HomeTeamId) && teamIds.Contains(m.AwayTeamId))
            .ToList();

        return sport switch
        {
            Sport.Football => SortFootball(group, h2hMatches),
            Sport.Basketball => SortBasketball(group, h2hMatches),
            Sport.Volleyball => SortVolleyball(group, h2hMatches),
            _ => throw new NotSupportedException($"Unsupported sport: {sport}")
        };
    }

    private static List<StandingEntry> SortFootball(
        List<StandingEntry> group, List<Match> h2hMatches)
    {
        var h2h = group.ToDictionary(s => s.TeamId, _ => new BasicH2H());

        foreach (var m in h2hMatches)
        {
            var homeId = (int)m.HomeTeamId;
            var awayId = (int)m.AwayTeamId;
            var homeGoals = m.Result!.HomeScore;
            var awayGoals = m.Result!.AwayScore;

            h2h[homeId].Scored += homeGoals;
            h2h[homeId].Conceded += awayGoals;
            h2h[awayId].Scored += awayGoals;
            h2h[awayId].Conceded += homeGoals;

            if (homeGoals > awayGoals) h2h[homeId].Points += 3;
            else if (homeGoals == awayGoals)
            {
                h2h[homeId].Points += 1;
                h2h[awayId].Points += 1;
            }
            else
            {
                h2h[awayId].Points += 3;
            }
        }

        return group
            .OrderByDescending(s => h2h[s.TeamId].Points)
            .ThenByDescending(s => h2h[s.TeamId].Difference)
            .ThenByDescending(s => h2h[s.TeamId].Scored)
            .ThenByDescending(s => s.Difference)
            .ThenByDescending(s => s.Scored)
            .ToList();
    }

    private static List<StandingEntry> SortBasketball(
        List<StandingEntry> group, List<Match> h2hMatches)
    {
        var h2h = group.ToDictionary(s => s.TeamId, _ => new BasicH2H());

        foreach (var m in h2hMatches)
        {
            var homeId = (int)m.HomeTeamId;
            var awayId = (int)m.AwayTeamId;
            var homePoints = m.Result!.HomeScore;
            var awayPoints = m.Result!.AwayScore;

            h2h[homeId].Scored += homePoints;
            h2h[homeId].Conceded += awayPoints;
            h2h[awayId].Scored += awayPoints;
            h2h[awayId].Conceded += homePoints;

            if (homePoints > awayPoints)
            {
                h2h[homeId].Points += 2;
                h2h[awayId].Points += 1;
            }
            else
            {
                h2h[awayId].Points += 2;
                h2h[homeId].Points += 1;
            }
        }

        return group
            .OrderByDescending(s => h2h[s.TeamId].Points)
            .ThenByDescending(s => h2h[s.TeamId].Difference)
            .ThenByDescending(s => s.Difference)
            .ThenByDescending(s => s.Scored)
            .ToList();
    }

    private static List<StandingEntry> SortVolleyball(
        List<StandingEntry> group, List<Match> h2hMatches)
    {
        var h2h = group.ToDictionary(s => s.TeamId, _ => new VolleyballH2H());

        foreach (var m in h2hMatches)
        {
            var homeId = (int)m.HomeTeamId;
            var awayId = (int)m.AwayTeamId;
            var homeSets = m.Result!.HomeScore;
            var awaySets = m.Result!.AwayScore;

            h2h[homeId].SetsWon += homeSets;
            h2h[homeId].SetsLost += awaySets;
            h2h[awayId].SetsWon += awaySets;
            h2h[awayId].SetsLost += homeSets;

            foreach (var set in m.Result.Sets)
            {
                h2h[homeId].PointsScored += set.HomeScore;
                h2h[homeId].PointsConceded += set.AwayScore;
                h2h[awayId].PointsScored += set.AwayScore;
                h2h[awayId].PointsConceded += set.HomeScore;
            }

            if (homeSets > awaySets) h2h[homeId].Points += 3;
            else h2h[awayId].Points += 3;
        }

        return group
            .OrderByDescending(s => h2h[s.TeamId].Points)
            .ThenByDescending(s => h2h[s.TeamId].SetDifference)
            .ThenByDescending(s => h2h[s.TeamId].PointsDifference)
            .ThenByDescending(s => s.SetDifference)
            .ThenByDescending(s => s.Scored)
            .ToList();
    }

    private class BasicH2H
    {
        public int Points { get; set; }
        public int Scored { get; set; }
        public int Conceded { get; set; }
        public int Difference => Scored - Conceded;
    }

    private class VolleyballH2H
    {
        public int Points { get; set; }
        public int SetsWon { get; set; }
        public int SetsLost { get; set; }
        public int SetDifference => SetsWon - SetsLost;
        public int PointsScored { get; set; }
        public int PointsConceded { get; set; }
        public int PointsDifference => PointsScored - PointsConceded;
    }
}