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
    /// Class NiStringExtraData.
    /// </summary>
    public class NiStringExtraData : NiExtraData
	{
        /// <summary>
        /// The bytes remaining
        /// </summary>
        public uint BytesRemaining;

        /// <summary>
        /// The string data
        /// </summary>
        public NiString StringData;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiStringExtraData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiStringExtraData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (base.Version <= eNifVersion.VER_4_2_2_0)
			{
				this.BytesRemaining = reader.ReadUInt32();
			}
			this.StringData = new NiString(file, reader);
		}
	}
}
