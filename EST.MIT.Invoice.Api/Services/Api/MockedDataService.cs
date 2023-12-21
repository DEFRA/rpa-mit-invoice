using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using EST.MIT.Invoice.Api.Models;


namespace EST.MIT.Invoice.Api.Services.Api;

public class MockedDataService : IMockedDataService
{
	public LoggedInUser GetLoggedInUser()
    {
	    return new LoggedInUser()
	    {
		    UserId = "user",
		    EmailAddress = "mockeduser@defra.gov.uk",
	    };
    }
}
