using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FileKraken.Source
{
  static class ProfileConstants
  {
    // === Constant Variables
    public const string kXMLElement_Profile       = "Profile";
    public const string kXMLElement_Component     = "Component";
    public const string kXMLElement_Source        = "Source";
    public const string kXMLElement_Destination   = "Destination"; 
    public const string kXMLElement_Active        = "Active";
    public const string kXMLAttribrute_Name       = "name";
    public const string kXMLFileExtension         = ".xml";

    // === Private Variables
    private static string _profileDirectory = null;

    // === Public Interface
    public static void Initialize()
    {
      _profileDirectory = FileKrakenConstants.GetFileKrakenDocumentsDirectory() + "\\Profiles\\";
    }

    public static string GetProfileDirectory()
    {
      return _profileDirectory;
    }

    public static string GetProfileFilePath(string profile)
    {
      return _profileDirectory + profile + kXMLFileExtension;
    }
  }

  public class Profile
  {
    // === Structures
    public struct Component
    {
      // === Private Variables
      private string _componentName;
      private string _sourceLocation;
      private string _destinationLocation;
      private bool _active;

      // === Properties
      public string Name
      {
        get { return _componentName; }
      }

      public string SourceLocation
      {
        get { return _sourceLocation; }
        set
        {
          _sourceLocation = value;
          _sourceLocation.Replace('/', '\\');
          string[] split = _sourceLocation.Split('\\');
          _componentName = split[split.Length - 1];
        }
      }

      public string Destination
      {
        get { return _destinationLocation; }
        set { _destinationLocation = value; }
      }

      public bool Active
      {
        get { return _active; }
        set { _active = value; }
      }
      // === End Properties
    }

    // === Private Variables
    private string _originalName;
    private string _profileName;
    private List<Component> _components;

    // === Constructor
    public Profile(string profileName = "EmptyProfile", List<Component> components = null)
    {
      _originalName = "";
      _profileName = profileName;
      _components = (components == null) ? new List<Component>() : components;
    }

    // === Public Interface
    public bool LoadFromXML(string filepath)
    {
      _components.Clear();

      if (false == HasProfileExtension(filepath))
      {
        filepath += ProfileConstants.kXMLFileExtension;
      }

      XmlReader profileReader = null;
      try
      {
        profileReader = XmlReader.Create(filepath);

        while (profileReader.Read())
        {
          if (XmlNodeType.Element == profileReader.NodeType && ProfileConstants.kXMLElement_Profile == profileReader.Name)
          {
            XMLHelper_ReadProfile(ref profileReader);

            // There should not be multiple profiles in one xml file, so incase there is, stop reading after the first one
            break;
          }
        }

        profileReader.Close();
      }
      catch (Exception ex)
      {
        profileReader?.Close();

        Console.WriteLine("Loading Profile from XML failed!\n\tError: " + ex.ToString());

        return false;
      }

      _originalName = _profileName;
      return true;
    }

    public void SaveToXML(string directory = "")
    {
      if ("" == directory)
      {
        directory = ProfileConstants.GetProfileDirectory();
      }

      // === If we had a valid original name that was different from our current name, then we need to delete that old profile file
      if ("" != _originalName && _originalName != _profileName)
      {
        File.Delete(ProfileConstants.GetProfileDirectory() + _originalName + ProfileConstants.kXMLFileExtension);
      }

      XmlWriterSettings xmlSettings = new XmlWriterSettings();
      xmlSettings.Indent = true;

      XmlWriter profileWriter = XmlWriter.Create(directory + "\\" + _profileName + ProfileConstants.kXMLFileExtension, xmlSettings);

      profileWriter.WriteStartDocument();
      profileWriter.WriteStartElement(ProfileConstants.kXMLElement_Profile);
      profileWriter.WriteAttributeString(ProfileConstants.kXMLAttribrute_Name, _profileName);

      // Write all the components
      for (int i = 0; i < _components.Count; ++i)
      {
        profileWriter.WriteStartElement(ProfileConstants.kXMLElement_Component);
        profileWriter.WriteAttributeString(ProfileConstants.kXMLAttribrute_Name, _components[i].Name);
        profileWriter.WriteElementString(ProfileConstants.kXMLElement_Source, _components[i].SourceLocation);
        profileWriter.WriteElementString(ProfileConstants.kXMLElement_Destination, _components[i].Destination);
        profileWriter.WriteElementString(ProfileConstants.kXMLElement_Active, _components[i].Active ? "true" : "false");
        profileWriter.WriteEndElement();
      }

      // Close the Profile element
      profileWriter.WriteEndElement();

      profileWriter.Flush();
      profileWriter.Close();
    }
    // === End Public Interface

    // === Private Interface
    private bool HasProfileExtension(string filePath)
    {
      string[] split = filePath.Split('.');

      if (split.Length >= 2)
      {
        int lastIndex = split.Length - 1;
        split[lastIndex] = "." + split[lastIndex];

        if (split[lastIndex] == ProfileConstants.kXMLFileExtension)
        {
          return true;
        }
      }

      return false;
    }

    private void XMLHelper_ReadProfile(ref XmlReader profileReader)
    {
      _profileName = profileReader.GetAttribute(ProfileConstants.kXMLAttribrute_Name);

      while (profileReader.Read())
      {
        if (XmlNodeType.Element == profileReader.NodeType && ProfileConstants.kXMLElement_Component == profileReader.Name)
        {
          Component newComponent = new Component();
          XMLHelper_ReadComponent(ref profileReader, ref newComponent);
          _components.Add(newComponent);
        }
      }
    }

    private void XMLHelper_ReadComponent(ref XmlReader profileReader, ref Component component)
    {
      while (profileReader.Read() && XmlNodeType.EndElement != profileReader.NodeType)
      {
        switch (profileReader.Name)
        {
          case ProfileConstants.kXMLElement_Source:
          {
            while (profileReader.Read() && XmlNodeType.EndElement != profileReader.NodeType)
            {
              if (XmlNodeType.Text == profileReader.NodeType)
              {
                component.SourceLocation = profileReader.Value;
              }
            }
          }
          break;
          case ProfileConstants.kXMLElement_Destination:
          {
            while (profileReader.Read() && XmlNodeType.EndElement != profileReader.NodeType)
            {
              if (XmlNodeType.Text == profileReader.NodeType)
              {
                component.Destination = profileReader.Value;
              }
            }
          }
          break;
          case ProfileConstants.kXMLElement_Active:
          {
            while (profileReader.Read() && XmlNodeType.EndElement != profileReader.NodeType)
            {
              if (XmlNodeType.Text == profileReader.NodeType)
              {
                component.Active = (profileReader.Value.ToLower() == "true" ? true : false);
              }
            }
          }
          break;
        }
      }
    }
    // === End Private Interface

    // === Properties

    // @property: The profile's name
    // @note: This name could be different from the name of corresponding file if the profile is 
    public string ProfileName
    {
      get { return _profileName; }
      set { _profileName = value; }
    }

    // @property: The profile's name that corresponds to the existing file 
    public string NameOnFile
    {
      get { return _originalName; }
    }

    // @property: The profile's components
    public ref List<Component> Components
    {
      get { return ref _components; }
    }
    // === End Properties
  }
}
