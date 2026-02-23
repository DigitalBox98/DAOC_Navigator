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
    /// Enum eTargetColor
    /// </summary>
    public enum eTargetColor : ushort
	{
        /// <summary>
        /// The t c_ ambient
        /// </summary>
        TC_AMBIENT,
        /// <summary>
        /// The t c_ diffuse
        /// </summary>
        TC_DIFFUSE,
        /// <summary>
        /// The t c_ specular
        /// </summary>
        TC_SPECULAR,
        /// <summary>
        /// The t c_ sel f_ illum
        /// </summary>
        TC_SELF_ILLUM
    }
}
