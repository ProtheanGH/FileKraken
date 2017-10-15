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
    List<string> _availabeProfiles;
    Profile _currentProfile;
    int _currentProfileIndex;
    bool _ignoreSelectionChange; // TODO: If another bool gets added, make a flag system

    // === Constructor
    public SettingsWindow(string activeProfile)
    {
      // Initialize the UI and WPF Objects
      InitializeComponent();

      _availabeProfiles = new List<string>();
      _currentProfile = null;
      _currentProfileIndex = 0;
      _ignoreSelectionChange = false;

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
    }

    // === Events
    private void OnRemoveComponent(Controls.ComponentField component)
    {
      Components_ListView.Items.Remove(component);
    }
    // === End Events

    // === UI Events
    private void ComponentAdd_Btn_Click(object sender, RoutedEventArgs e)
    {
      Controls.ComponentField newComponent = new Controls.ComponentField(OnRemoveComponent);

      AddComponent(newComponent);
    }

    private void Okay_Btn_Click(object sender, RoutedEventArgs e)
    {
      SaveCurrentProfile();

      this.Close();
    }

    private void RemoveProfile_Btn_Click(object sender, RoutedEventArgs e)
    {
      _ignoreSelectionChange = true;
      RemoveProfile(_currentProfile.NameOnFile);
      _currentProfile = null;

      if (_availabeProfiles.Count > 0)
      {
        // Switch to a valid Profile
        _currentProfileIndex = (_currentProfileIndex > 0 ? _currentProfileIndex - 1 : 0);
        SwitchProfile(_availabeProfiles[_currentProfileIndex]);
      }
      else
      {
        // Add a Generic Profile
        AddProfile();
      }
    }

    private void ProfileName_Tb_LostFocus(object sender, RoutedEventArgs e)
    {
      // Users is persumably done typing the Profile's name, go ahead and make the change (no need to save the profile here yet though)
      _currentProfile.ProfileName = ProfileName_Tb.Text;
      _availabeProfiles[_currentProfileIndex] = _currentProfile.ProfileName;

      // We don't want to actually switch profiles here, as we are on the same profile, but it just has an unsaved name
      _ignoreSelectionChange = true;
      Profile_Dropdown.Items[_currentProfileIndex] = _currentProfile.ProfileName;
      Profile_Dropdown.SelectedIndex = _currentProfileIndex;
    }

    private void Profile_Dropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (true == _ignoreSelectionChange)
      {
        _ignoreSelectionChange = false;
      }
      else
      {
        string newSelection = (string)Profile_Dropdown.SelectedValue;
        if (_currentProfile.ProfileName != newSelection)
        {
          SwitchProfile(newSelection);
        }
      }
    }
    // === End UI Events

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

      _currentProfile.LoadFromXML(ProfileConstants.GetProfileDirectory() + profileName);

      ProfileName_Tb.Text = _currentProfile.ProfileName;

      ClearComponents();
      // Display the profile's components
      for (int i = 0; i < _currentProfile.Components.Count; ++i)
      {
        Controls.ComponentField compField = new Controls.ComponentField(OnRemoveComponent);

        compField.Source_Tb.Text = _currentProfile.Components[i]._sourceLocation;
        compField.Destination_Tb.Text = _currentProfile.Components[i]._destinationLocation;

        AddComponent(compField);
      }

      // Update the Profile Dropdown
      _currentProfileIndex = _availabeProfiles.IndexOf(_currentProfile.ProfileName);
      Profile_Dropdown.SelectedIndex = _currentProfileIndex;
    }

    private void AddComponent(Controls.ComponentField component)
    {
      int insertAt = Components_ListView.Items.Count - 1;
      Components_ListView.Items.Insert(insertAt, component);
    }

    private void ClearComponents()
    {
      // Only clear the components, leave the Add Button
      while (Components_ListView.Items.Count > 1)
      {
        Components_ListView.Items.RemoveAt(0);
      }
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

    private void RemoveProfile(string profileName)
    {
      File.Delete(ProfileConstants.GetProfileFilePath(profileName));
      _availabeProfiles.Remove(profileName);
      Profile_Dropdown.Items.Remove(profileName);
    }
    // === End Private Interface
  }
}
