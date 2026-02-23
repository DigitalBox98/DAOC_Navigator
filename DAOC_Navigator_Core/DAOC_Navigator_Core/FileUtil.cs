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

namespace DAOC_Navigator_Core
{
    /// <summary>
    /// Cross-platform file-system utilities.
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// Resolves the actual on-disk path for a file whose name may differ in case
        /// from what the application expects (needed on Linux / macOS file systems).
        /// </summary>
        /// <param name="pathAndFileName">Candidate path (may have wrong case).</param>
        /// <returns>The correctly-cased path as reported by the file system.</returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when no file matching the name (case-insensitively) can be found.
        /// </exception>
        public static string GetActualCaseForFileName(string pathAndFileName)
        {
            string? directory = Path.GetDirectoryName(pathAndFileName);
            if (directory == null)
                throw new FileNotFoundException("File not found: " + pathAndFileName);

            string[] filesInDirectory = Directory.GetFiles(directory);

            string? match = Array.Find(
                filesInDirectory,
                file => string.Equals(file, pathAndFileName, StringComparison.OrdinalIgnoreCase));

            return match ?? throw new FileNotFoundException("File not found: " + pathAndFileName);
        }

        // FIX #13: GetSystemSeparator() is removed.
        // Use Path.Combine() everywhere instead of manual concatenation with a
        // platform separator. Path.Combine handles cross-platform path building
        // correctly and is already available in all target frameworks.
        //
        // Migration: replace every occurrence of
        //   Path.Combine(dir, file)
        // with
        //   Path.Combine(dir, file)
    }
}
