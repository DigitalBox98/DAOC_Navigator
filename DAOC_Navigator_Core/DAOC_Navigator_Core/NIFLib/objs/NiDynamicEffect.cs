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
    /// Class NiDynamicEffect.
    /// </summary>
    public class NiDynamicEffect : NiAVObject
	{
        /// <summary>
        /// The switch state
        /// </summary>
        public bool SwitchState;

        /// <summary>
        /// The affected nodes
        /// </summary>
        public NiRef<NiAVObject>[]? AffectedNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiDynamicEffect"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiDynamicEffect(NiFile file, BinaryReader reader) : base(file, reader)
		{
			this.SwitchState = true;
			if (base.Version >= eNifVersion.VER_10_1_0_106)
			{
				this.SwitchState = reader.ReadBoolean(Version);
			}
			if (base.Version <= eNifVersion.VER_4_0_0_2 || base.Version >= eNifVersion.VER_10_0_1_0)
			{
				this.AffectedNodes = new NiRef<NiAVObject>[reader.ReadUInt32()];
				for (int i = 0; i < this.AffectedNodes.Length; i++)
				{
					this.AffectedNodes[i] = new NiRef<NiAVObject>(reader);
				}
			}
		}
	}
}
