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
    /// Class NiGeometry.
    /// </summary>
    public class NiGeometry : NiAVObject
	{
        /// <summary>
        /// The data
        /// </summary>
        public NiRef<NiGeometryData> Data;

        /// <summary>
        /// The skin instance
        /// </summary>
        public NiRef<NiSkinInstance>? SkinInstance;

        /// <summary>
        /// The material names
        /// </summary>
        public NiString[]? MaterialNames;

        /// <summary>
        /// The material extra data
        /// </summary>
        public int[]? MaterialExtraData;

        /// <summary>
        /// The active material
        /// </summary>
        public int ActiveMaterial;

        /// <summary>
        /// The has shader
        /// </summary>
        public bool HasShader;

        /// <summary>
        /// The shader name
        /// </summary>
        public string? ShaderName;

        /// <summary>
        /// The unkown integer
        /// </summary>
        public uint UnkownInteger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiGeometry"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        /// <exception cref="Exception">unsupported data</exception>
        public NiGeometry(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.Data = new NiRef<NiGeometryData>(reader);
			if (base.Version >= eNifVersion.VER_3_3_0_13)
			{
				this.SkinInstance = new NiRef<NiSkinInstance>(reader);
			}
			if (base.Version >= eNifVersion.VER_20_2_0_7)
			{
				this.MaterialNames = new NiString[reader.ReadUInt32()];
				for (int i = 0; i < this.MaterialNames.Length; i++)
				{
					this.MaterialNames[i] = new NiString(file, reader);
				}
				this.MaterialExtraData = new int[this.MaterialNames.Length];
				for (int j = 0; j < this.MaterialNames.Length; j++)
				{
					this.MaterialExtraData[j] = reader.ReadInt32();
				}
				this.ActiveMaterial = reader.ReadInt32();
			}
			if (base.Version >= eNifVersion.VER_10_0_1_0 && base.Version <= eNifVersion.VER_20_1_0_3)
			{
				this.HasShader = reader.ReadBoolean(Version);
				if (this.HasShader)
				{
					this.ShaderName = new NiString(file, reader).Value;
					this.UnkownInteger = reader.ReadUInt32();
				}
			}
			if (base.Version == eNifVersion.VER_10_4_0_1)
			{
				reader.ReadUInt32();
			}
			if (base.Version >= eNifVersion.VER_20_2_0_7)
			{
				throw new Exception("unsupported data");
			}
		}
	}
}
