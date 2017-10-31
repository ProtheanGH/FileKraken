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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileKraken.Source;

namespace FileKraken
{
  static class FileKrakenConstants
  {
    public static string GetFileKrakenDocumentsDirectory()
    {
      return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\FileKraken";
    }
  }
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    // === Delegates
    public delegate void OnSettingsWindowClose(Profile activeProfile);

    // === Private Variables
    Profile _currentProfile;

    // === Constructor
    public MainWindow()
    {
      InitializeComponent();

      // Initialize
      ProfileConstants.Initialize();

      // NOTE: May remove, left over from trying to copy over the default at the start of runtime
      Components_ListView.Items.RemoveAt(0);

      // Make sure the needed Directories / Files exist
      if (false == Directory.Exists(FileKrakenConstants.GetFileKrakenDocumentsDirectory()))
      {
        Directory.CreateDirectory(ProfileConstants.GetProfileDirectory());
      }

      // TODO: Load the active profile from some last session file
      // === TESTING ONLY === //
      Profile test = new Profile();
      test.LoadFromXML(ProfileConstants.GetProfileDirectory() + "GenericProfile");

      SetProfile(test);
      // ==================== //
    }

    // === UI Events
    private void Settings_Btn_Click(object sender, RoutedEventArgs e)
    {
      SaveActiveProfile();
      Windows.SettingsWindow settingsWindow = new Windows.SettingsWindow(_currentProfile.ProfileName, Settings_Window_Close);
      settingsWindow.Show();
    }

    private void Sync_Btn_Click(object sender, RoutedEventArgs e)
    {

    }

    private void SyncTo_Btn_Click(object sender, RoutedEventArgs e)
    {

    }

    private void Settings_Window_Close(Profile activeProfile)
    {
      SetProfile(activeProfile);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      SaveActiveProfile();
    }
    // === End UI Events

    // === Public Interface
    public void SetProfile(string profileName)
    {
      Profile newProfile = new Profile();
      newProfile.LoadFromXML(ProfileConstants.GetProfileFilePath(profileName));

      SetProfile(newProfile);
    }

    public void SetProfile(Profile profile)
    {
      _currentProfile = profile;

      Components_ListView.Items.Clear();

      for (int i = 0; i < _currentProfile.Components.Count; ++i)
      {
        AddComponent(i);
      }
    }
    // === End Public Interface

    // === Private Interface
    private void AddComponent(int componentIndex)
    {
      Profile.Component component = _currentProfile.Components[componentIndex];

      Controls.ComponentDisplay newComponentDisplay = new Controls.ComponentDisplay(componentIndex, UpdateComponent);

      newComponentDisplay.DisplayText = component.Name;
      newComponentDisplay.IsChecked = component.Active;
      newComponentDisplay.ToolTip = "Source: " + component.SourceLocation + "\nDestination: " + component.Destination;

      Components_ListView.Items.Add(newComponentDisplay);
    }

    private void SaveActiveProfile()
    {
      _currentProfile.SaveToXML();
    }

    private void UpdateComponent(int componentIndex, bool isActive)
    {
      // TODO: There's gotta be a better way to do this
      Profile.Component component = _currentProfile.Components[componentIndex];
      component.Active = isActive;
      _currentProfile.Components[componentIndex] = component;
    }
    // === End Private Interface
  }
}
