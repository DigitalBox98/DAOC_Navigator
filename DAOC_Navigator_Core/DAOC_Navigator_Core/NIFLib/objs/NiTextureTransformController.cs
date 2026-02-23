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
    /// Class NiTextureTransformController.
    /// </summary>
    public class NiTextureTransformController : NiFloatInterpController
	{
        /// <summary>
        /// The unkown2
        /// </summary>
        public byte Unkown2;

        /// <summary>
        /// The texture slot
        /// </summary>
        public eTexType TextureSlot;

        /// <summary>
        /// The operation
        /// </summary>
        public eTexTransform Operation;

        /// <summary>
        /// The data
        /// </summary>
        public NiRef<NiFloatData>? Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiTextureTransformController"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiTextureTransformController(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.Unkown2 = reader.ReadByte();
			this.TextureSlot = (eTexType)reader.ReadUInt32();
			this.Operation = (eTexTransform)reader.ReadUInt32();
			if (base.Version <= eNifVersion.VER_10_1_0_0)
			{
				this.Data = new NiRef<NiFloatData>(reader);
			}
		}
	}
}
