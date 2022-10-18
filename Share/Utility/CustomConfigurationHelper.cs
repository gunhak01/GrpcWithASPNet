using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Diagnostics;

namespace Share.Utility;
public class CustomConfigurationHelper
{
  private static IConfigurationBuilder _configBuilder;
  private static readonly object _lock = new object();
  public static IConfigurationRoot CreateConfigurationBuilder(string customJsonFile, string[] args = null)
  {

    var result = CreateConfigurationRoot(customJsonFile, args);

    result = result?.Providers.Count() == 0 ? CreateConfigurationRoot(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, customJsonFile), args) : result;

    return result;
  }


  private static IConfigurationRoot CreateConfigurationRoot(string customJsonFile, string[] args)
  {
    lock (_lock)
    {
      try
      {

        if (_configBuilder == null)
        {
          _configBuilder = new ConfigurationBuilder().AddJsonFile(customJsonFile, true, true);
        }
        else
        {
          var jsonConfigList = _configBuilder.Sources.Where(source => source is JsonConfigurationSource).OfType<JsonConfigurationSource>().ToList();
          if (jsonConfigList.Exists(jsonSource => jsonSource.Path.Equals(customJsonFile, StringComparison.OrdinalIgnoreCase)))
          {
            return _configBuilder.Build();
          }

          _configBuilder = _configBuilder.AddJsonFile(customJsonFile, true, true);
        }

        if (args != null)
        {
          _configBuilder = _configBuilder.AddCommandLine(args);
        }

        return _configBuilder.Build();
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
        return _configBuilder.Build();
      }
    }
  }
}