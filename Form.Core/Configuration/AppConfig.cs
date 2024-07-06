using Microsoft.Extensions.Configuration;

namespace FormCore.Configuration;

public class AppConfig
{
	private readonly IConfiguration _config;
	public AppConfig(IConfiguration config)
	{
		_config = config;
	}

	public string APIKey()
	{
		return _config["Key:Form.API"].ToString();
	}
}
