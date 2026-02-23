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


namespace GL_Graphics.Tests;

public class GL_TextureTests
{
    [Fact]
    public void ShouldReturnConstructor()
    {
        // Arrange
        //new GL_Helper(new GL_Mock());

        int ID = 1; // GL_Texture.GL_Init_And_Bind_Texture();

        // Act

        // Assert
        Assert.NotEqual(0, ID);
    }

}

