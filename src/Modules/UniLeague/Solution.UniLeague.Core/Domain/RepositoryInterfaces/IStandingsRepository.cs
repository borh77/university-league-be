using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Core.Domain.RepositoryInterfaces;

public interface IStandingsRepository
{
    // Briše sve redove tabele za ligu i upisuje nove, u jednoj transakciji
    void ReplaceForLeague(long leagueId, IReadOnlyCollection<StandingEntry> entries);

    // Dodaje jedan red - koristi se kad admin rucno ubaci tim u ligu
    void Add(StandingEntry entry);
}
