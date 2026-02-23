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

namespace Niflib
{
	using System.IO;

    /// <summary>
    /// Class NiFooter.
    /// </summary>
    public class NiFooter
	{
        /// <summary>
        /// The root nodes
        /// </summary>
        public NiRef<NiObject>[]? RootNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiFooter"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiFooter(NiFile file, BinaryReader reader)
		{
			if (file.Header.Version >= eNifVersion.VER_3_3_0_13)
			{
				uint num = reader.ReadUInt32();
				this.RootNodes = new NiRef<NiObject>[num];
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					this.RootNodes[num2] = new NiRef<NiObject>(reader.ReadUInt32());
					num2++;
				}
			}
		}
	}
}
