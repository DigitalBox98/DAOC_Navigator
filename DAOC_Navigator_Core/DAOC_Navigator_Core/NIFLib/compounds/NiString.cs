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
    /// Class NiString.
    /// </summary>
    public class NiString
	{
        /// <summary>
        /// The value
        /// </summary>
        public string Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiString"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiString(NiFile? file, BinaryReader reader)
		{
        	var count = reader.ReadUInt32();
        	if (count > 16384)
        		throw new NotSupportedException("String too long. Not a NIF file or unsupported format?");
        	this.Value = new string(reader.ReadChars((int)count));
		}

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
		{
			return this.Value;
		}
	}
}
