using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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

namespace NetSwitcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void SwitchAdapters(string adapteron, string adapteroff)
        {
            ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapter");
            ManagementObjectCollection mgmtObjectColl = managementClass.GetInstances();
            //ManagementObject myObject = null;
            foreach (ManagementObject mgmtObject in mgmtObjectColl)
            {
                if (mgmtObject["NetConnectionID"] != null && mgmtObject["NetConnectionID"].Equals(adapteron))
                {
                    try
                    {
                        mgmtObject.InvokeMethod("Enable", null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка включения адаптера! " + ex.Message);
                    }
                } else if (mgmtObject["NetConnectionID"] != null && mgmtObject["NetConnectionID"].Equals(adapteroff))
                {
                    try
                    {
                        mgmtObject.InvokeMethod("Disable", null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка отключения адаптера! " + ex.Message);
                    }
                }
            }
        }

        public List<string> ListNetworkAdapter()
        {
            List<string> Adapters = new List<string>();
            ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapter");
            ManagementObjectCollection mgmtObjectColl = managementClass.GetInstances();
            foreach (ManagementObject mgmtObject in mgmtObjectColl)
            {
                if (mgmtObject["NetConnectionID"] != null) Adapters.Add(mgmtObject["NetConnectionID"].ToString());
            }
            return Adapters;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var list = ListNetworkAdapter();
            if (String.IsNullOrEmpty(Properties.Settings.Default.netadapter1) || String.IsNullOrEmpty(Properties.Settings.Default.netadapter2) || !list.Contains(Properties.Settings.Default.netadapter1) || !list.Contains(Properties.Settings.Default.netadapter2))
            {
                CFGClear();
                StartCfg(list);
            }
        }

        private void CFGClear()
        {
            Properties.Settings.Default.netadapter1 = string.Empty;
            Properties.Settings.Default.netadapter1name = string.Empty;
            Properties.Settings.Default.netadapter2 = string.Empty;
            Properties.Settings.Default.netadapter2name = string.Empty;
            Properties.Settings.Default.Save();
        }

        private void StartCfg(List<string> list)
        {            
            Adapter1Box.ItemsSource = list;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.netadapter1) && list.Contains(Properties.Settings.Default.netadapter1)) Adapter1Box.SelectedItem = Properties.Settings.Default.netadapter1;
            Adapter2Box.ItemsSource = list;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.netadapter2) && list.Contains(Properties.Settings.Default.netadapter2)) Adapter2Box.SelectedItem = Properties.Settings.Default.netadapter2;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.netadapter1name)) Adapter1Name.Text = Properties.Settings.Default.netadapter1name;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.netadapter2name)) Adapter2Name.Text = Properties.Settings.Default.netadapter2name;
            CfgPanel.Visibility = Visibility.Visible;
        }

        private void Cancle_Click(object sender, RoutedEventArgs e) => CfgPanel.Visibility = Visibility.Hidden;

        private void SaveCfg_Click(object sender, RoutedEventArgs e)
        {
            if (Adapter1Box.SelectedItem.ToString() == Adapter2Box.SelectedItem.ToString())
            {
                MessageBox.Show("Нужно выбрать 2 разных сетевых адаптера");
                return;
            }
            Properties.Settings.Default.netadapter1 = Adapter1Box.SelectedItem.ToString();
            Properties.Settings.Default.netadapter1name = Adapter1Name.Text;
            Properties.Settings.Default.netadapter2 = Adapter2Box.SelectedItem.ToString();
            Properties.Settings.Default.netadapter2name = Adapter2Name.Text;
            Properties.Settings.Default.Save();
            CfgPanel.Visibility = Visibility.Hidden;
        }

        private void Adapter1Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Adapter1Box.SelectedItem.ToString()) && (String.IsNullOrEmpty(Adapter1Name.Text)) || Adapter1Box.Items.Contains(Adapter1Name.Text))
            {
                Adapter1Name.Text = Adapter1Box.SelectedItem.ToString();
            }
        }

        private void Adapter2Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Adapter2Box.SelectedItem.ToString()) && (String.IsNullOrEmpty(Adapter2Name.Text)) || Adapter2Box.Items.Contains(Adapter2Name.Text))
            {
                Adapter2Name.Text = Adapter2Box.SelectedItem.ToString();
            }
        }

        private void OpenCFG_Click(object sender, RoutedEventArgs e)
        {
            var list = ListNetworkAdapter();
            StartCfg(list);
        }

        private void adapter1checker_Checked(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.netadapter1) && !String.IsNullOrEmpty(Properties.Settings.Default.netadapter2))
            {
                SwitchAdapters(Properties.Settings.Default.netadapter1, Properties.Settings.Default.netadapter2);
            }
        }

        private void adapter2checker_Checked(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.netadapter1) && !String.IsNullOrEmpty(Properties.Settings.Default.netadapter2))
            {
                SwitchAdapters(Properties.Settings.Default.netadapter2, Properties.Settings.Default.netadapter1);
            }
        }
    }
}
