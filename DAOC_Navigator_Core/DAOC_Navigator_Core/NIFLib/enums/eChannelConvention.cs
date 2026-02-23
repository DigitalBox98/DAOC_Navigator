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
    /// Enum eChannelConvention
    /// </summary>
    public enum eChannelConvention : uint
	{
        /// <summary>
        /// The c c_ fixed
        /// </summary>
        CC_FIXED,
        /// <summary>
        /// The c c_ index
        /// </summary>
        CC_INDEX = 3u,
        /// <summary>
        /// The c c_ compressed
        /// </summary>
        CC_COMPRESSED,
        /// <summary>
        /// The c c_ empty
        /// </summary>
        CC_EMPTY
    }
}
