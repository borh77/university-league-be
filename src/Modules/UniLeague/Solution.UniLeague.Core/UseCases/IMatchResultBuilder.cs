using Solution.UniLeague.API.Dtos;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Core.UseCases;

// Deljena logika izgradnje MatchResult-a iz DTO-a - koristi je i delegat i admin
public interface IMatchResultBuilder
{
    MatchResult Build(Sport sport, Match match, SubmitMatchResultDto request);
}
