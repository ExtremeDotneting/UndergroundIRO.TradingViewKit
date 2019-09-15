using System.Threading.Tasks;
using System.Windows;

namespace UndergroundIRO.Tests.TradingViewKitWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(async () =>
            {
                await TerminalControl.Initialize();
                await TerminalControl.RunAsync((cmd) => new TestsCmdLine(cmd));
            });
        }
    }
}
