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
#if DEBUG
		return _config["Key:Form.API"].ToString();
#else
		return AccessSecretVersion.Get(_config["ProjectID"], "FormAPIKey");
#endif
	}
}
