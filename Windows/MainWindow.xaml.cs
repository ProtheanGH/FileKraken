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
      Windows.SettingsWindow settingsWindow = new Windows.SettingsWindow("GenericProfile", Settings_Window_Close); // TODO: Use the current active profile
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
        AddComponent(_currentProfile.Components[i]);
      }
    }
    // === End Public Interface
    
    // === Private Interface
    private void AddComponent(Profile.Component component)
    {
      Controls.ComponentDisplay newComponentDisplay = new Controls.ComponentDisplay();

      newComponentDisplay.DisplayText = component._componentName;
      newComponentDisplay.IsChecked = component._active;
      newComponentDisplay.ToolTip = "Source: " + component._sourceLocation + "\nDestination: " + component._destinationLocation;

      Components_ListView.Items.Add(newComponentDisplay);
    }
    // === End Private Interface
  }
}
