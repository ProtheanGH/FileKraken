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
      _profileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\FileKraken\\Profiles\\";
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
      public string _componentName;
      public string _sourceLocation;
      public string _destinationLocation;
      public bool _active;
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

      XmlReader profileReader = null;
      try
      {
        profileReader = XmlReader.Create(filepath + ProfileConstants.kXMLFileExtension);

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

    public void SaveToXML(string directory)
    {
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
      string componentName = "";
      for (int i = 0; i < _components.Count; ++i)
      {
        // Get the name of the componet
        string[] splitComponent = _components[i]._sourceLocation.Split('\\');
        componentName = splitComponent[splitComponent.Length - 1];

        profileWriter.WriteStartElement(ProfileConstants.kXMLElement_Component);
        profileWriter.WriteAttributeString(ProfileConstants.kXMLAttribrute_Name, componentName);
        profileWriter.WriteElementString(ProfileConstants.kXMLElement_Source, _components[i]._sourceLocation);
        profileWriter.WriteElementString(ProfileConstants.kXMLElement_Destination, _components[i]._destinationLocation);
        profileWriter.WriteElementString(ProfileConstants.kXMLElement_Active, _components[i]._active ? "true" : "false");
        profileWriter.WriteEndElement();
      }

      // Close the Profile element
      profileWriter.WriteEndElement();

      profileWriter.Flush();
      profileWriter.Close();
    }


    // === Private Interface
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
      component._componentName = profileReader.GetAttribute(ProfileConstants.kXMLAttribrute_Name);

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
                component._sourceLocation = profileReader.Value;
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
                component._destinationLocation = profileReader.Value;
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
                component._active = (profileReader.Value.ToLower() == "true" ? true : false);
              }
            }
          }
          break;
        }
      }
    }

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
  }
}
