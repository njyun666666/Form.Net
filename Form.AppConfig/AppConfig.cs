using Microsoft.Extensions.Configuration;

namespace FormAppConfig.Configuration;

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
