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

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DAOC_Navigator_Avalonia.Views;

public partial class AboutWindow : Window // check why partial
{
    public AboutWindow()
    {
        InitializeComponent();
        this.Opacity = 0.2;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void CloseButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(); // Ferme la fenêtre "À propos"
    }

}