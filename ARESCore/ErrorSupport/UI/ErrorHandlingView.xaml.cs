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
using Ninject;

namespace ARESCore.ErrorSupport.UI
{
  /// <summary>
  /// Interaction logic for ErrorHandlingView.xaml
  /// </summary>
  public partial class ErrorHandlingView : UserControl
  {
    public ErrorHandlingView()
    {
      InitializeComponent();
      DataContext = AresKernel._kernel.Get<ErrorHandlingViewModel>();
    }
  }
}
