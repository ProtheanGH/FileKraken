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
  // === Delegates
  public delegate void RemoveComponentCallback(ComponentField toRemove);

  /// <summary>
  /// Interaction logic for ComponentField.xaml
  /// </summary>
  public partial class ComponentField : UserControl
  {
    // === Public Variables
    public RemoveComponentCallback _Event_OnRemove;

    // === Private Variables
    private bool _active;

    // === Constructor
    public ComponentField(RemoveComponentCallback onRemove)
    {
      _Event_OnRemove = onRemove;

      InitializeComponent();
    }

    // === UI Events
    private void Remove_Btn_Click(object sender, RoutedEventArgs e)
    {
      _Event_OnRemove?.Invoke(this);
    }
    // === End UI Events

    // === Public Interface
    public Profile.Component GetComponentData()
    {
      return new Profile.Component
      {
        Active = _active,
        SourceLocation = Source_Tb.Text,
        Destination = Destination_Tb.Text
      };
    }
    // === End Public Interface

    // === Properties
    public bool Active
    {
      get { return _active; }
      set { _active = value; }
    }
    // === End Properties
  }
}
