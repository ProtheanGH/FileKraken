﻿using System;
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
    public MainWindow()
    {
      InitializeComponent();

      // Initialize
      ProfileConstants.Initialize();

      // Make sure the needed Directories / Files exist
      if (false == Directory.Exists(FileKrakenConstants.GetFileKrakenDocumentsDirectory()))
      {
        Directory.CreateDirectory(ProfileConstants.GetProfileDirectory());
      }

      // TODO: Load the active profile from some last session file
    }

    // === UI Events
    private void Settings_Btn_Click(object sender, RoutedEventArgs e)
    {
      Windows.SettingsWindow settingsWindow = new Windows.SettingsWindow("GenericProfile"); // TODO: Use the current active profile
      settingsWindow.Show();
    }

    private void Sync_Btn_Click(object sender, RoutedEventArgs e)
    {

    }

    private void SyncTo_Btn_Click(object sender, RoutedEventArgs e)
    {

    }
    // === End UI Events
  }
}
