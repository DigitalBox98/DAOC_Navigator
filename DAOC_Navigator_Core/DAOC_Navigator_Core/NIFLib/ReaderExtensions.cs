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
	using OpenTK.Mathematics;
	using Matrix = OpenTK.Mathematics.Matrix4;
	using Color3 = OpenTK.Mathematics.Color4;
	using System.IO;
	
    /// <summary>
    /// Class ReaderExtensions.
    /// </summary>
    public static class ReaderExtensions
	{
    	/// <summary>
    	/// Read a Boolean from the Stream Depending on Nif Version
    	/// </summary>
    	/// <param name="reader">Reader</param>
    	/// <param name="version">Nif Object Version</param>
    	/// <returns>bool from Int or Byte</returns>
    	public static bool ReadBoolean(this BinaryReader reader, eNifVersion version)
    	{
    		if (version < eNifVersion.VER_4_1_0_1)
    			return reader.ReadUInt32() != 0;
    		
    		return reader.ReadBoolean();
    	}
    	
        /// <summary>
        /// Reads the float array.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.Single[].</returns>
        public static float[] ReadFloatArray(this BinaryReader reader, int length)
		{
			float[] array = new float[length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = reader.ReadSingle();
			}
			return array;
		}

        /// <summary>
        /// Reads the u int32 array.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.UInt32[].</returns>
        public static uint[] ReadUInt32Array(this BinaryReader reader, int length)
		{
			uint[] array = new uint[length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = reader.ReadUInt32();
			}
			return array;
		}

        /// <summary>
        /// Reads the u int16 array.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.UInt16[].</returns>
        public static ushort[] ReadUInt16Array(this BinaryReader reader, int length)
		{
			ushort[] array = new ushort[length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = reader.ReadUInt16();
			}
			return array;
		}

        /// <summary>
        /// Reads the vector2.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 ReadVector2(this BinaryReader reader)
		{
			return new Vector2(reader.ReadSingle(), reader.ReadSingle());
		}

        /// <summary>
        /// Reads the vector3.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 ReadVector3(this BinaryReader reader)
		{
			return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

        /// <summary>
        /// Reads the vector4.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Vector4.</returns>
        public static Vector4 ReadVector4(this BinaryReader reader)
		{
			return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

        /// <summary>
        /// Reads the color3.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Color3.</returns>
        public static Color3 ReadColor3(this BinaryReader reader)
		{
			return new Color3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0);
		}

        /// <summary>
        /// Reads the color4.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Color4.</returns>
        public static Color4 ReadColor4(this BinaryReader reader)
		{
			return new Color4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

        /// <summary>
        /// Reads the color4 byte.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Color4.</returns>
        public static Color4 ReadColor4Byte(this BinaryReader reader)
		{
			return new Color4((float)reader.ReadByte() / 255f, (float)reader.ReadByte() / 255f, (float)reader.ReadByte() / 255f, (float)reader.ReadByte() / 255f);
		}

        /// <summary>
        /// Reads the matrix33.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Matrix.</returns>
        public static Matrix ReadMatrix33(this BinaryReader reader)
		{
			Matrix identity = Matrix.Identity;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					identity[j, i] = reader.ReadSingle();
				}
			}
			return identity;
		}
	}
}
