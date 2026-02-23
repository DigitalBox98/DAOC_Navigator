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
    /// Class NiTextKeyExtraData.
    /// </summary>
    public class NiTextKeyExtraData : NiExtraData
	{
        /// <summary>
        /// The number text keys
        /// </summary>
        public uint NumTextKeys;

        /// <summary>
        /// The unkown int1
        /// </summary>
        public uint UnkownInt1;

        /// <summary>
        /// The text keys
        /// </summary>
        public StringKey[] TextKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiTextKeyExtraData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiTextKeyExtraData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (base.Version <= eNifVersion.VER_4_2_2_0)
			{
				this.UnkownInt1 = reader.ReadUInt32();
			}
			this.NumTextKeys = reader.ReadUInt32();
			this.TextKeys = new StringKey[this.NumTextKeys];
			int num = 0;
			while ((long)num < (long)((ulong)this.NumTextKeys))
			{
				this.TextKeys[num] = new StringKey(reader, eKeyType.LINEAR_KEY);
				num++;
			}
		}
	}
}
