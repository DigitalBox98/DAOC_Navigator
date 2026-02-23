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

using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace DAOC_Navigator_Avalonia;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();

        // WGL rendering is Windows-only; on Linux/macOS Avalonia uses GLX/Metal automatically.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            builder = builder.With(new Win32PlatformOptions
            {
                RenderingMode = new Collection<Win32RenderingMode> { Win32RenderingMode.Wgl }
            });
        }

        return builder;
    }
} 
