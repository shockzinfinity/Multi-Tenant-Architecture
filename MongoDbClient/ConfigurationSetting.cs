namespace MongoDbClient
{
  public class ConfigurationSetting
  {
    public DatabaseSettings DatabaseSettings { get; set; }
  }

  public class DatabaseSettings
  {
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
  }
}