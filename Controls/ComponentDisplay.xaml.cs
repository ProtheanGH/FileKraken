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

namespace FileKraken.Controls
{
  /// <summary>
  /// Interaction logic for ComponentDisplay.xaml
  /// </summary>
  public partial class ComponentDisplay : UserControl
  {
    // === Constructor
    public ComponentDisplay()
    {
      InitializeComponent();
    }

    // === Properties
    public string DisplayText
    {
      get { return Component_Lbl.Content.ToString(); }
      set { Component_Chkbox.Content = value; }
    }

    public bool? IsChecked
    {
      get { return Component_Chkbox.IsChecked; }
      set { Component_Chkbox.IsChecked = value; }
    }
    // === End Properties
  }
}
