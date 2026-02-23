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
    /// Class NiSkinInstance.
    /// </summary>
    public class NiSkinInstance : NiObject
	{
        /// <summary>
        /// The data
        /// </summary>
        public NiRef<NiSkinData> Data;

        /// <summary>
        /// The partition
        /// </summary>
        public NiRef<NiSkinPartition>? Partition;

        /// <summary>
        /// The skeleton root
        /// </summary>
        public NiRef<NiNode> SkeletonRoot;

        /// <summary>
        /// The bones
        /// </summary>
        public NiRef<NiNode>[] Bones;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiSkinInstance"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiSkinInstance(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.Data = new NiRef<NiSkinData>(reader);
			if (base.Version >= eNifVersion.VER_10_2_0_0)
			{
				this.Partition = new NiRef<NiSkinPartition>(reader);
			}
			this.SkeletonRoot = new NiRef<NiNode>(reader);
			uint num = reader.ReadUInt32();
			this.Bones = new NiRef<NiNode>[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				this.Bones[num2] = new NiRef<NiNode>(reader);
				num2++;
			}
		}
	}
}
