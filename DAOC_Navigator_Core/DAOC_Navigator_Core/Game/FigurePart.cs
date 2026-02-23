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

namespace DAOC_Navigator_Core.Game
{
    public class FigurePart
    {
        private string niffile    = string.Empty;
        private string archivenum = string.Empty;

        public string NIFFile    { get => niffile;    set => niffile    = value; }
        public string ArchiveNum { get => archivenum; set => archivenum = value; }

        public FigurePart(string nifFile, string archiveNum)
        {
            niffile    = nifFile;
            archivenum = archiveNum;
        }

        public override string ToString()
            => $" nifFile = {niffile}, archiveNum = {archivenum}";
    }
}
