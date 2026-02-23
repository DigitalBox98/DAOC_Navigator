/*
 * DAOC Navigator - The free open source DAOC game navigator
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, see <https://www.gnu.org/licenses/>
 *
 */

using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DAOC_Navigator_Avalonia.ViewModels;

namespace DAOC_Navigator_Avalonia.Views;

public partial class MainWindow : Window
{
    Expander? myCamera;
    Expander? myOptions;
    Expander? myMonster;
    MainControlWindowGLRendering? myControl;
    TextBox? monsterTextBox;

    public MainWindowViewModel viewModelContext { get; } = new MainWindowViewModel();

    public MainWindow()
    {
        InitializeComponent();

        myCamera = this.FindControl<Expander>("MyCamera");
        if (myCamera != null)
        {
            myCamera.IsExpanded = true;
        }
        myOptions = this.FindControl<Expander>("MyOptions");
        myMonster = this.FindControl<Expander>("MyMonster");
        if (myMonster != null)
        {
            myMonster.IsExpanded = true;
        }
        
        monsterTextBox = this.Find<TextBox>("Monster");
        if (monsterTextBox != null)
        {
            monsterTextBox.KeyUp += MonsterTextBox_KeyUp;
        }

        DataContext = viewModelContext;

    }

    private void MonsterTextBox_KeyUp(object? sender, KeyEventArgs e)
    {

        if (e.Key == Key.Enter)
        {
            if (myControl != null && monsterTextBox != null && monsterTextBox.Text != null)
            {
                myControl.SetMonster(monsterTextBox.Text);
            }
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Method called when the view is opened
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // Get the control MyControl and push it to the ViewModel
        myControl = this.FindControl<MainControlWindowGLRendering>("MyControl");
        if (myControl != null && DataContext is MainWindowViewModel viewModel)
        {
            myControl.Focus();
            viewModel.SetMyControl(myControl);
        }


    }

    private void OnQuit(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnCamera(object sender, RoutedEventArgs e)
    {
        if (myCamera != null)
        {
            if (myCamera.IsVisible == true)
                myCamera.IsVisible = false;
            else
                myCamera.IsVisible = true;
        }

    }

    private void OnOptions(object sender, RoutedEventArgs e)
    {
        if (myOptions != null)
        {
            if (myOptions.IsVisible == true)
                myOptions.IsVisible = false;
            else
                myOptions.IsVisible = true;
        }

    }

    private void OnMonster(object sender, RoutedEventArgs e)
    {
        if (myMonster != null)
        {
            if (myMonster.IsVisible == true)
                myMonster.IsVisible = false;
            else
                myMonster.IsVisible = true;
        }
    }

    private async void ShowAboutDialog(object sender, RoutedEventArgs e)
    {
        AboutWindow aboutWindow = new AboutWindow();
        await aboutWindow.ShowDialog(this); 
    }


}