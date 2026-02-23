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
    /// Enum eChannelType
    /// </summary>
    public enum eChannelType : uint
	{
        /// <summary>
        /// The CHN l_ red
        /// </summary>
        CHNL_RED,
        /// <summary>
        /// The CHN l_ green
        /// </summary>
        CHNL_GREEN,
        /// <summary>
        /// The CHN l_ blue
        /// </summary>
        CHNL_BLUE,
        /// <summary>
        /// The CHN l_ alpha
        /// </summary>
        CHNL_ALPHA,
        /// <summary>
        /// The CHN l_ compressed
        /// </summary>
        CHNL_COMPRESSED,
        /// <summary>
        /// The CHN l_ index
        /// </summary>
        CHNL_INDEX = 16u,
        /// <summary>
        /// The CHN l_ empty
        /// </summary>
        CHNL_EMPTY = 19u
	}
}
