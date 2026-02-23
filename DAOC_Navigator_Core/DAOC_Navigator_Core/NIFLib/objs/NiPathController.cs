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
    /// Class NiPathController.
    /// </summary>
    public class NiPathController : NiTimeController
	{
        /// <summary>
        /// The unknown1
        /// </summary>
        public ushort Unknown1;

        /// <summary>
        /// The unknown2
        /// </summary>
        public uint Unknown2;

        /// <summary>
        /// The unknown3
        /// </summary>
        public float Unknown3;

        /// <summary>
        /// The unknown4
        /// </summary>
        public float Unknown4;

        /// <summary>
        /// The unknown5
        /// </summary>
        public ushort Unknown5;

        /// <summary>
        /// The position data
        /// </summary>
        public NiRef<NiPosData> PosData;

        /// <summary>
        /// The float data
        /// </summary>
        public NiRef<NiFloatData> FloatData;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPathController" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPathController(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (this.File.Header.Version >= eNifVersion.VER_10_1_0_0)
			{
				this.Unknown1 = reader.ReadUInt16();
			}
			this.Unknown2 = reader.ReadUInt32();
			this.Unknown3 = reader.ReadSingle();
			this.Unknown4 = reader.ReadSingle();
			this.Unknown5 = reader.ReadUInt16();
			this.PosData = new NiRef<NiPosData>(reader);
			this.FloatData = new NiRef<NiFloatData>(reader);
		}
	}
}
