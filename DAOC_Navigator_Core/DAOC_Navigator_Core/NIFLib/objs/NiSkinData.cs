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
    /// Class NiSkinData.
    /// </summary>
    public class NiSkinData : NiObject
	{
        /// <summary>
        /// The transform
        /// </summary>
        public SkinTransform Transform;

        /// <summary>
        /// The partition
        /// </summary>
        public NiRef<NiSkinPartition>? Partition;

        /// <summary>
        /// The has vertex weights
        /// </summary>
        public bool HasVertexWeights;

        /// <summary>
        /// The bone list
        /// </summary>
        public SkinData[]? BoneList;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiSkinData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiSkinData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.HasVertexWeights = true;
			this.Transform = new SkinTransform(file, reader);
			uint num = reader.ReadUInt32();
			if (base.Version >= eNifVersion.VER_4_0_0_2 && base.Version <= eNifVersion.VER_10_1_0_0)
			{
				this.Partition = new NiRef<NiSkinPartition>(reader);
			}
			if (base.Version >= eNifVersion.VER_4_2_1_0)
			{
				this.HasVertexWeights = reader.ReadBoolean(Version);
			}
			if (this.HasVertexWeights)
			{
				this.BoneList = new SkinData[num];
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					this.BoneList[num2] = new SkinData(file, reader, this.HasVertexWeights);
					num2++;
				}
			}
		}
	}
}
