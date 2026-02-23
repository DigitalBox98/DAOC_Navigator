/*
 * DAOC Navigator - The free open DAOC game navigator
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
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace DAOC_Navigator_ImGUI
{
    public static class Program
    {

        public static bool NEW_RENDERING = true;

        private static void Main()
        {

            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(1024, 768),
                Title = "DAOC Navigator",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
                APIVersion = new Version(3, 3),
            };

            using (var window = new MainWindow(nativeWindowSettings))
            {
                window.Run();
            }
        }
    }
}
