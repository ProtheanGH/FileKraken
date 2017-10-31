using System;
using System.Collections.Generic;
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

namespace FileKraken.Controls
{
  public partial class ComponentDisplay : UserControl
  {
    // === Delegates
    public delegate void UpdateComponent(int index, bool isActive);
    
    // === Private Variables
    int _componentIndex;
    UpdateComponent _onCheckedEvent;
    
    // === Constructor
    public ComponentDisplay(int componentIndex, UpdateComponent checkedEvent)
    {
      InitializeComponent();

      _componentIndex = componentIndex;
      _onCheckedEvent = checkedEvent;
    }

    // === UI Events
    private void Component_Chkbox_Checked(object sender, RoutedEventArgs e)
    {
      _onCheckedEvent?.Invoke(_componentIndex, (bool)IsChecked);
    }
    // === End UI Events

    // === Properties
    public string DisplayText
    {
      get { return Component_Lbl.Content.ToString(); }
      set { Component_Lbl.Content = value; }
    }

    public bool? IsChecked
    {
      get { return Component_Chkbox.IsChecked; }
      set { Component_Chkbox.IsChecked = value; }
    }
    // === End Properties
  }
}
