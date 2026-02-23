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
    /// <summary>
    /// Class NiExtraData.
    /// </summary>
    public class NiExtraData : NiObject
	{
        /// <summary>
        /// The name
        /// </summary>
        public NiString? Name;

        /// <summary>
        /// The next extra data
        /// </summary>
        public NiRef<NiExtraData>? NextExtraData;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiExtraData" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiExtraData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (this.File.Header.Version >= eNifVersion.VER_10_0_1_0)
			{
				this.Name = new NiString(file, reader);
			}
			if (this.File.Header.Version <= eNifVersion.VER_4_2_2_0)
			{
				this.NextExtraData = new NiRef<NiExtraData>(reader.ReadUInt32());
			}
		}
	}
}
