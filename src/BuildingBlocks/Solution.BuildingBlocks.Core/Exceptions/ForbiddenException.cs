namespace Solution.BuildingBlocks.Core.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) {}
}