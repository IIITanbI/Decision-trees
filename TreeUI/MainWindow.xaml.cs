using C4_5;
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

namespace TreeUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        C4_5Tree curTree = null;

        public MainWindow(C4_5Tree tree)
        {
            InitializeComponent();
            curTree = tree;
            DrawTree(curTree);

        }
        public void DrawTree(C4_5Tree tree)
        {
            TVizualizer.Draw(tree);
        }
       
    }
}
