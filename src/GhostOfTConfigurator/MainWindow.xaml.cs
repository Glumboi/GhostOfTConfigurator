using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using TextBox = System.Windows.Controls.TextBox;

namespace GhostOfTConfigurator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
    private const string settingsLoc =
        "Software\\Sucker Punch Productions\\Ghost of Tsushima DIRECTOR'S CUT\\Graphics";

    private bool mouseDown;
    private Point lastLocation;

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
                var tb = new Wpf.Ui.Controls.TextBox()
                {
                    Text = $"{val} = {key.GetValue(val)}",
                    Margin = new Thickness(0, 2, 0, 2),
                    PlaceholderText = val
                };

                this.Settings_Panel.Children.Add(tb);
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

    async void RegReadErr()
    {
        Wpf.Ui.Controls.MessageBox msgBox = new Wpf.Ui.Controls.MessageBox();
        msgBox.Title = "Error";
        msgBox.Content =
            "Cannot read registry, perhaps admin privileges are required!\nThe App will close after clicking OK!";
        msgBox.CloseButtonText = "OK";
        msgBox.CloseButtonAppearance = ControlAppearance.Primary;
        await msgBox.ShowDialogAsync();
        Environment.Exit(-1);
    }

    async void AllGoodMessage()
    {
        Wpf.Ui.Controls.MessageBox msgBox = new Wpf.Ui.Controls.MessageBox();
        msgBox.Title = "Info";
        msgBox.Content =
            "Saved settings!";
        msgBox.CloseButtonText = "OK";
        msgBox.CloseButtonAppearance = ControlAppearance.Primary;
        await msgBox.ShowDialogAsync();
    }

    private async void SaveConfigButton_OnClick(object sender, RoutedEventArgs e)
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

            Wpf.Ui.Controls.MessageBox msgBox = new Wpf.Ui.Controls.MessageBox();
            msgBox.Title = "Info";
            msgBox.Content =
                $"Saved settings to file: {saveFileDialog.FileName}!";
            msgBox.CloseButtonText = "OK";
            msgBox.CloseButtonAppearance = ControlAppearance.Primary;
            await msgBox.ShowDialogAsync();
        }
    }

    private async void LoadButton_OnClick(object sender, RoutedEventArgs e)
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

            Wpf.Ui.Controls.MessageBox msgBox = new Wpf.Ui.Controls.MessageBox();
            msgBox.Title = "Info";
            msgBox.Content =
                $"Loaded settings from file: {openFileDialog.FileName}!";
            msgBox.CloseButtonText = "OK";
            msgBox.CloseButtonAppearance = ControlAppearance.Primary;
            await msgBox.ShowDialogAsync();
        }
    }


    private void Banner_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}