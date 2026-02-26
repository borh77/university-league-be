using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Security.Claims;
using Solution.API;
using Xunit;

namespace Solution.BuildingBlocks.Tests;

public class BaseWebIntegrationTest<TTestFactory> : IClassFixture<TTestFactory> where TTestFactory : WebApplicationFactory<Program>
{
    protected TTestFactory Factory { get; }

    public BaseWebIntegrationTest(TTestFactory factory)
    {
        Factory = factory;
    }

    protected static ControllerContext BuildContext(string id)
    {
        return new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("personId", id)
                }))
            }
        };
    }
}