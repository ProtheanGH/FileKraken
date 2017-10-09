using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FileKraken.Source;

namespace FileKraken.Windows
{
  /// <summary>
  /// Interaction logic for SettingsWindow.xaml
  /// </summary>
  public partial class SettingsWindow : Window
  {
    // === Private Variables
    Profile _currentProfile;
    List<string> _availabeProfiles;

    // === Constructor
    public SettingsWindow(string activeProfile)
    {
      // Initialize the UI and WPF Objects
      InitializeComponent();

      _currentProfile = null;
      _availabeProfiles = new List<string>();

      // Get the available profiles
      string[] profiles = Directory.GetFiles(ProfileConstants.GetProfileDirectory());
      for (int i = 0; i < profiles.Length; ++i)
      {
        string[] splitString = profiles[i].Split('\\');
        splitString = splitString[splitString.Length - 1].Split('.');
        _availabeProfiles.Add(splitString[0]);
        Profile_Dropdown.Items.Add(_availabeProfiles[i]);
      }

      // No profiles? Add Default
      if (_availabeProfiles.Count == 0)
      {
        AddProfile();
      }
      else
      {
        if (false == _availabeProfiles.Contains(activeProfile))
        {
          Console.WriteLine("Could not find profile: " + activeProfile);
          activeProfile = _availabeProfiles[0];
        }

        SwitchProfile(activeProfile);
      }

      Profile_Dropdown.SelectedIndex = _availabeProfiles.IndexOf(_currentProfile.ProfileName);
    }

    // === Events
    private void ComponentAdd_Btn_Click(object sender, RoutedEventArgs e)
    {
      Controls.ComponentField newComponent = new Controls.ComponentField(OnRemoveComponent);

      AddComponent(newComponent);
    }

    private void OnRemoveComponent(Controls.ComponentField component)
    {
      Components_ListView.Items.Remove(component);
    }

    private void Okay_Btn_Click(object sender, RoutedEventArgs e)
    {
      SaveCurrentProfile();
    }

    // === Private Interface
    private void SwitchProfile(string profileName)
    {
      // Handle the current profile
      if (_currentProfile != null)
      {
        SaveCurrentProfile();
      }
      else
      {
        // At this point we can now create a profile if we don't already have one
        _currentProfile = new Profile();
      }

      _currentProfile.LoadFromXML(ProfileConstants.GetProfileDirectory() + profileName + ".xml");

      ProfileName_Tb.Text = _currentProfile.ProfileName;
      for (int i = 0; i < _currentProfile.Components.Count; ++i)
      {
        Controls.ComponentField compField = new Controls.ComponentField(OnRemoveComponent);

        compField.Source_Tb.Text = _currentProfile.Components[i]._sourceLocation;
        compField.Destination_Tb.Text = _currentProfile.Components[i]._destinationLocation;

        AddComponent(compField);
      }
    }

    private void AddComponent(Controls.ComponentField component)
    {
      int insertAt = Components_ListView.Items.Count - 1;
      Components_ListView.Items.Insert(insertAt, component);
    }

    private void SaveCurrentProfile()
    {
      _currentProfile.ProfileName = ProfileName_Tb.Text;
      _currentProfile.Components.Clear();
      for (int i = 0; i < Components_ListView.Items.Count - 1; ++i)
      {
        _currentProfile.Components.Add(((Controls.ComponentField)Components_ListView.Items[i]).GetComponentData());
      }

      _currentProfile.SaveToXML(ProfileConstants.GetProfileDirectory());
    }

    private void AddProfile()
    {
      string profileName = "GenericProfile";
      int profileNumber = 0;

      // Determine if we already have a profile with this name
      while (_availabeProfiles.Contains(profileName + (profileNumber > 0 ? profileNumber.ToString() : "")))
      {
        ++profileNumber;
      }

      if (profileNumber > 0)
      {
        profileName += profileNumber.ToString();
      }
      _availabeProfiles.Add(profileName);
      Profile_Dropdown.Items.Add(profileName);

      Profile newProfile = new Profile
      {
        ProfileName = profileName
      };

      newProfile.SaveToXML(ProfileConstants.GetProfileDirectory());
      
      // Switch to this new Profile
      SwitchProfile(profileName);
    }
  }
}
