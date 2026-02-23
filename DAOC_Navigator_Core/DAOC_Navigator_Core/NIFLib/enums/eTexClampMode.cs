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
    /// Enum eTexClampMode
    /// </summary>
    public enum eTexClampMode : uint
	{
        /// <summary>
        /// The clam p_ s_ clam p_ t
        /// </summary>
        CLAMP_S_CLAMP_T,
        /// <summary>
        /// The clam p_ s_ wra p_ t
        /// </summary>
        CLAMP_S_WRAP_T,
        /// <summary>
        /// The wra p_ s_ clam p_ t
        /// </summary>
        WRAP_S_CLAMP_T,
        /// <summary>
        /// The wra p_ s_ wra p_ t
        /// </summary>
        WRAP_S_WRAP_T
    }
}
