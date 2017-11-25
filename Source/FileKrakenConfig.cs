using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using FileKraken.Source;

namespace FileKraken.Source
{
  static class FileKrakenConfig
  {
    // === Private Enums
    private enum ConfigValue
    {
      CFG_INVALID_VALUE = -1,
      CFG_LAST_PROFILE,
      CFG_WINDOW_X_POS,
      CFG_WINDOW_Y_POS,
      CFG_WINDOW_WIDTH,
      CFG_WINDOW_HEIGHT,
      CFG_SETTINGS_WINDOW_POS,
      CFG_SETTINGS_WINDOW_WIDTH,
      CFG_SETTINGS_WINDOW_HEIGHT
    };

    // === Private Constants
    private const string kConfigFileName = "FileKraken.xml";
    private const string kXMLElement_ConfigHeader = "FileKraken_Config";
    
    // === Private Variables
    private static bool _initialized;
    // Profile
    private static string _lastProfile = "GenericProfile";
    // Main Window
    private static int _windowXPosition = 0;
    private static int _windowYPosition = 0;
    private static int _windowWidth;
    private static int _windowHeight;
    // Settings Window
    private static int _settingsWindowPosition; // TODO: ID corresponding to some pre-defined position behaviors
    private static int _settingsWidth;
    private static int _settingsHeight;

    // === Public Interface
    public static void LoadConfigFile()
    {
      XmlReader configReader = null;
      try
      {
        configReader = XmlReader.Create(FileKrakenConstants.GetFileKrakenDocumentsDirectory() + "\\" + kConfigFileName);

        while (configReader.Read())
        {
          if (XmlNodeType.Element == configReader.NodeType && kXMLElement_ConfigHeader == configReader.Name)
          {
            XMLHelper_ReadConfigValue(ref configReader);

            // There should not be multiple configs in one xml file, so incase there is, stop reading after the first one
            break;
          }
        }

        configReader.Close();
      }
      catch (System.IO.FileNotFoundException ex)
      {
        configReader?.Close();

        Console.WriteLine("No config found, using defaults.");

        SaveConfigFile();
      }
      catch (Exception ex)
      {
        configReader?.Close();

        Console.WriteLine("Loading Config from XML failed!\n\tError: " + ex.ToString());

        return;
      }

      _initialized = true;
    }

    public static void SaveConfigFile()
    {
      XmlWriter configWriter = XmlWriter.Create(FileKrakenConstants.GetFileKrakenDocumentsDirectory() + "\\" + kConfigFileName);

      configWriter.WriteStartDocument();
      configWriter.WriteStartElement(kXMLElement_ConfigHeader);
      {
        configWriter.WriteElementString(ConfigValue.CFG_LAST_PROFILE.ToString(), _lastProfile);
      
        configWriter.WriteElementString(ConfigValue.CFG_WINDOW_X_POS.ToString(), _windowXPosition.ToString());
        configWriter.WriteElementString(ConfigValue.CFG_WINDOW_Y_POS.ToString(), _windowYPosition.ToString());
        configWriter.WriteElementString(ConfigValue.CFG_WINDOW_HEIGHT.ToString(), _windowHeight.ToString());
        configWriter.WriteElementString(ConfigValue.CFG_WINDOW_WIDTH.ToString(), _windowWidth.ToString());

        configWriter.WriteElementString(ConfigValue.CFG_SETTINGS_WINDOW_POS.ToString(), _settingsWindowPosition.ToString());
        configWriter.WriteElementString(ConfigValue.CFG_SETTINGS_WINDOW_HEIGHT.ToString(), _settingsHeight.ToString());
        configWriter.WriteElementString(ConfigValue.CFG_SETTINGS_WINDOW_WIDTH.ToString(), _settingsWidth.ToString());
      }
      configWriter.WriteEndElement();

      configWriter.Flush();
      configWriter.Close();
    }
    // === End Public Interface

    // === Private Interface
    private static void XMLHelper_ReadConfigValue(ref XmlReader configReader)
    {
      ConfigValue configEnumValue;

      while (configReader.Read() && XmlNodeType.EndElement != configReader.NodeType)
      {
        configEnumValue = (Enum.TryParse<ConfigValue>(configReader.Name, out configEnumValue) ? configEnumValue : ConfigValue.CFG_INVALID_VALUE);
        switch (configEnumValue)
        {
          case ConfigValue.CFG_LAST_PROFILE:
          XMLHelper_GetStringFromConfig(ref configReader, out _lastProfile);
          break;
          case ConfigValue.CFG_WINDOW_X_POS:
          XMLHelper_GetIntFromConfig(ref configReader, out _windowXPosition);
          break;
          case ConfigValue.CFG_WINDOW_Y_POS:
          XMLHelper_GetIntFromConfig(ref configReader, out _windowYPosition);
          break;
          case ConfigValue.CFG_WINDOW_WIDTH:
          XMLHelper_GetIntFromConfig(ref configReader, out _windowWidth);
          break;
          case ConfigValue.CFG_WINDOW_HEIGHT:
          XMLHelper_GetIntFromConfig(ref configReader, out _windowHeight);
          break;
          case ConfigValue.CFG_SETTINGS_WINDOW_POS:
          XMLHelper_GetIntFromConfig(ref configReader, out _settingsWindowPosition);
          break;
          case ConfigValue.CFG_SETTINGS_WINDOW_WIDTH:
          XMLHelper_GetIntFromConfig(ref configReader, out _settingsWidth);
          break;
          case ConfigValue.CFG_SETTINGS_WINDOW_HEIGHT:
          XMLHelper_GetIntFromConfig(ref configReader, out _settingsHeight);
          break;
          default:
            // TODO: Add logging for unsupported config value
          break;
        }
      }
    }

    private static void XMLHelper_GetStringFromConfig(ref XmlReader configReader, out string _value)
    {
      _value = "";
      while (configReader.Read() && XmlNodeType.EndElement != configReader.NodeType)
      {
        if (XmlNodeType.Text == configReader.NodeType)
        {
          _value = configReader.Value;
        }
      }
    }

    private static void XMLHelper_GetIntFromConfig(ref XmlReader configReader, out int _value)
    {
      _value = -1;
      while (configReader.Read() && XmlNodeType.EndElement != configReader.NodeType)
      {
        if (XmlNodeType.Text == configReader.NodeType)
        {
          int.TryParse(configReader.Value, out _value);
        }
      }
    }
    // === End Private Interface

    // === Properties
    public static bool IsInitialized
    {
      get { return _initialized; }
    }

    public static string LastProfile
    {
      set { _lastProfile = value; }
      get { return _lastProfile; }
    }

    // TODO: Add the rest of the properties as they are implemented

    // === End Properties
  }
}
