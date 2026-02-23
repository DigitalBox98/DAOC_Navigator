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

using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using ImageMagick;
using System.Diagnostics;
using DAOC_Navigator_Core.Game;

namespace DAOC_Navigator_Core.GL_Rendering
{
    /// <summary>
    /// Wraps an OpenGL 2-D texture loaded from a file or an MPAK skin archive.
    /// </summary>
    public class GL_Texture
    {
        public int    ID;
        public string type;
        public string path;

        public GL_Texture(int id, string path, string type)
        {
            this.ID   = id;
            this.path = path;
            this.type = type;
        }

        // -------------------------------------------------------------------------
        // Factory methods
        // -------------------------------------------------------------------------

        /// <summary>Loads a texture directly from a full file path.</summary>
        public static int LoadTexture(string path)
        {
            int id = GL_Init_And_Bind_Texture();
            using var image = new MagickImage(path);
            GL_Generate_Texture(image);
            return id;
        }

        /// <summary>
        /// Loads a texture from <paramref name="file"/> inside <paramref name="directory"/>,
        /// falling back to alternative extensions and directories when the exact file is missing.
        /// FIX #13: uses Path.Combine instead of manual separator concatenation.
        /// </summary>
        public static GL_Texture LoadFromFile(string file, string directory, string type, bool flip)
        {
            int id = GL_Init_And_Bind_Texture();

            // Resolve the actual texture file, trying common DAoC fallback paths.
            string filename = Path.Combine(directory, file);

            if (!File.Exists(filename))
            {
                // 1. Try .dds instead of .tga
                filename = filename.Replace(".tga", ".dds");

                if (!File.Exists(filename))
                {
                    // 2. Try "Nifs" directory instead of "Dnifs"
                    filename = filename.Replace("Dnifs", "Nifs");

                    if (!File.Exists(filename))
                    {
                        // 3. Fall back to the bundled default texture
                        filename = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "Resources",
                            "default.dds");
                    }
                }
            }

            using var image = new MagickImage(filename);
            GL_Generate_Texture(image, flip);

            return new GL_Texture(id, file, type);
        }

        /// <summary>
        /// Loads a texture from a named entry inside an MPAK skin archive.
        /// FIX #13: uses Path.Combine for archive path construction.
        /// </summary>
        public static GL_Texture LoadFromArchive(Skin skin, string directory, string type, bool flip)
        {
            int id = GL_Init_And_Bind_Texture();

            string texFilename  = skin.Filename.Replace(".tga", ".dds");
            string archiveFile  = Path.Combine(directory, "skins", $"skin{skin.ArchiveNum}.mpk");

            MemoryStream? fstream = null;
            if (File.Exists(archiveFile))
            {
                using var mpk = new PAKFile(archiveFile);
                fstream = new MemoryStream();
                mpk.ExtractFile(texFilename, fstream);
            }

            if (fstream != null)
            {
                using var image = new MagickImage(fstream);
                GL_Generate_Texture(image, flip);
            }

            return new GL_Texture(id, skin.Filename, type);
        }

        // -------------------------------------------------------------------------
        // OpenGL helpers
        // -------------------------------------------------------------------------

        /// <summary>Allocates and binds a new OpenGL texture handle.</summary>
        public static int GL_Init_And_Bind_Texture()
        {
            int id = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, id);
            return id;
        }

        /// <summary>Uploads <paramref name="image"/> data to the currently bound texture.</summary>
        public static void GL_Generate_Texture(MagickImage image, bool flip = false)
        {
            if (flip)
                Debug.WriteLine("[GL_Texture] Vertical flip requested but not yet implemented.");

            byte[]? imageData = image.GetPixels().ToByteArray(PixelMapping.RGBA);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                image.Width, image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, imageData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,     (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,     (int)TextureWrapMode.Repeat);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        /// <summary>Binds this texture to the specified texture unit.</summary>
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }
    }
}
