using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace GhostOfTConfigurator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string settingsLoc =
        "Software\\Sucker Punch Productions\\Ghost of Tsushima DIRECTOR'S CUT\\Graphics";

    public MainWindow()
    {
        InitializeComponent();

        var key = Registry.CurrentUser.OpenSubKey(settingsLoc);
        if (key != null)
        {
            var values = key.GetValueNames().ToList();
            values.Sort();
            foreach (var val in values)
            {
                this.Settings_Panel.Children.Add(new TextBox()
                {
                    Text = $"{val} = {key.GetValue(val)}",
                });
            }

            return;
        }

        RegReadErr();
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var key = Registry.CurrentUser.OpenSubKey(settingsLoc, true);
        if (key != null)
        {
            foreach (TextBox tb in this.Settings_Panel.Children)
            {
                var tbTextSplit = tb.Text.Split('=');
                string value = tbTextSplit.Last().Trim();
                string name = tbTextSplit.First().Trim();


                if (key != null)
                {
                    Console.WriteLine($"Setting Name: {name}, Value: {value}");

                    try
                    {
                        key.SetValue(name, value, RegistryValueKind.DWord);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                }
            }

            AllGoodMessage();
            return;
        }

        RegReadErr();
    }

    void RegReadErr()
    {
        MessageBox.Show(
            "Cannot read registry, perhaps admin privileges are required!\nThe App will close after clicking OK!",
            "Error", MessageBoxButton.OK,
            MessageBoxImage.Error);
        Environment.Exit(-1);
    }

    void AllGoodMessage()
    {
        MessageBox.Show("Saved settings!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void SaveConfigButton_OnClick(object sender, RoutedEventArgs e)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Title = "Save current registry snapshot as file...";
        saveFileDialog.FileName = "GoTS-Settings.txt";

        if (saveFileDialog.ShowDialog() == true)
        {
            List<string> content = new List<string>();
            foreach (TextBox tb in this.Settings_Panel.Children)
            {
                content.Add(tb.Text);
            }

            File.WriteAllLines(saveFileDialog.FileName, content);
            MessageBox.Show("Saved settings to file!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void LoadButton_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Open settings file...";

        if (openFileDialog.ShowDialog() == true)
        {
            this.Settings_Panel.Children.Clear();
            foreach (var str in File.ReadLines(openFileDialog.FileName))
            {
                this.Settings_Panel.Children.Add(new TextBox()
                {
                    Text = str
                });
            }

            MessageBox.Show($"Loaded settings from file: {openFileDialog.FileName}!", "Info", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}