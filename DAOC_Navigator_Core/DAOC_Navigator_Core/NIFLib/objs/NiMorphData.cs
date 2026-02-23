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
    /// Class NiMorphData.
    /// </summary>
    public class NiMorphData : NiObject
	{
        /// <summary>
        /// The number morphs
        /// </summary>
        public uint NumMorphs;

        /// <summary>
        /// The number vertices
        /// </summary>
        public uint NumVertices;

        /// <summary>
        /// The relative targets
        /// </summary>
        public byte RelativeTargets;

        /// <summary>
        /// The morphs
        /// </summary>
        public Morph[] Morphs;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiMorphData" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiMorphData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.NumMorphs = reader.ReadUInt32();
			this.NumVertices = reader.ReadUInt32();
			this.RelativeTargets = reader.ReadByte();
			this.Morphs = new Morph[this.NumMorphs];
			int num = 0;
			while ((long)num < (long)((ulong)this.NumMorphs))
			{
				this.Morphs[num] = new Morph(file, reader, this.NumVertices);
				num++;
			}
		}
	}
}
