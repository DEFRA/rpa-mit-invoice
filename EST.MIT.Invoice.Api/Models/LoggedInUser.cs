using Newtonsoft.Json;

namespace EST.MIT.Invoice.Api.Models;

public class LoggedInUser
{
	public string UserId { get; set; }
	public string EmailAddress { get; set; }
}