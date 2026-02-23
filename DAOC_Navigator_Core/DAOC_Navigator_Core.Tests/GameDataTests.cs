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


using DAOC_Navigator_Core;
using DAOC_Navigator_Core.Game;

namespace Tests;

public class GameDataTests
{
    [Fact]
    public void ShouldReturnConstructor()
    {
        // Arrange
        var gamedata = new GameData("");

        // Act

        // Assert
        Assert.NotNull(gamedata);
    }

    [Fact]
    public void ShouldLoadGameData()
    {
        // Arrange
        var gamedata = new GameData("Resources/Data");

        // Act

        // Assert
        Assert.True(gamedata.IsDefined);
    }

}
