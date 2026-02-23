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

using System.Text;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace DAOC_Navigator_Core
{
    /// <summary>
    /// Provides read access to files packed inside MPAK archives
    /// (files with the <c>.mpk</c> and <c>.npk</c> extensions).
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IDisposable"/> – always dispose (or use a <c>using</c> block)
    /// when you are finished so the underlying stream is released promptly.
    /// </remarks>
    public class PAKFile : IDisposable
    {
        private Stream?       archiveStream;
        private BinaryReader? reader;
        private bool          disposed;

        private string    internalArchiveName = string.Empty;
        private Hashtable entriesTable        = new Hashtable();  // lowercase filename → Entry

        /// <summary>The internal archive name stored inside the MPAK header.</summary>
        public string ArchiveName => internalArchiveName;

        // -------------------------------------------------------------------------
        // Constructors
        // -------------------------------------------------------------------------

        /// <summary>
        /// Opens and parses the MPAK archive at the given file path.
        /// </summary>
        /// <param name="filePath">Full path to the <c>.mpk</c> or <c>.npk</c> file.</param>
        public PAKFile(string filePath)
        {
            archiveStream = File.OpenRead(filePath);
            ProcessPAKFile(filePath);
        }

        /// <summary>
        /// Opens and parses an MPAK archive from an existing stream.
        /// </summary>
        /// <param name="fileStream">Stream positioned at the beginning of the archive.</param>
        public PAKFile(Stream fileStream)
        {
            archiveStream = fileStream;
            ProcessPAKFile(null);
        }

        // -------------------------------------------------------------------------
        // Parsing
        // -------------------------------------------------------------------------

        private void ProcessPAKFile(string? archivePath)
        {
            if (archiveStream == null) return;

            reader = new BinaryReader(archiveStream);

            string head = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (head != "MPAK")
                throw new FileLoadException("This file is not an MPAK file.", archivePath);

            // Skip 17 unknown header bytes (bytes 4-20); position the stream at byte 21.
            reader.BaseStream.Seek(21, SeekOrigin.Begin);

            // Decompress the archive name and directory streams.
            internalArchiveName = Encoding.ASCII.GetString(ReadCompressedStream());
            byte[] directory    = ReadCompressedStream();

            long dataRegionStart = reader.BaseStream.Position;

            // Each directory entry is exactly 0x11C (284) bytes long.
            const int EntrySize   = 0x11C;
            const int EntryStride = EntrySize;

            for (int offset = 0; offset <= directory.Length - EntrySize; offset += EntryStride)
            {
                Entry entry = new Entry();

                // Null-terminated filename occupies the first 0x100 (256) bytes.
                var name = new StringBuilder();
                for (int i = 0; directory[offset + i] != 0; i++)
                    name.Append((char)directory[offset + i]);
                entry.FileName = name.ToString();

                // Read the multi-byte integer fields stored in little-endian order.
                entry.unknown3 = ReadUInt64LE(directory, offset + 0x100);

                entry.unknown1        = ReadInt32LE(directory, offset + 0x108);
                entry.UncompressedSize = ReadInt32LE(directory, offset + 0x10C);
                entry.FileOffset       = dataRegionStart + ReadInt32LE(directory, offset + 0x110);
                entry.CompressedSize   = ReadInt32LE(directory, offset + 0x114);
                entry.unknown2        = ReadInt32LE(directory, offset + 0x118);

                entry.dateCreation = new DateTime((long)(entry.unknown3 * 1_000_000));

                string key = entry.FileName.ToLower();
                if (!entriesTable.Contains(key))
                    entriesTable.Add(key, entry);
            }
        }

        // -------------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------------

        /// <summary>
        /// Extracts the named file from the archive into <paramref name="destinationStream"/>.
        /// The stream is rewound to position 0 after writing.
        /// </summary>
        /// <returns><c>true</c> on success, <c>false</c> if the entry was not found.</returns>
        public bool ExtractFile(string fileToExtract, MemoryStream destinationStream)
        {
            byte[]? data = null;
            if (!TryExtractBytes(fileToExtract, out data))
                return false;

            destinationStream.Write(data!, 0, data!.Length);
            destinationStream.Position = 0;
            return true;
        }

        /// <summary>All entries contained in the archive.</summary>
        public Entry[] Files
        {
            get
            {
                var t = new Entry[entriesTable.Values.Count];
                entriesTable.Values.CopyTo(t, 0);
                return t;
            }
        }

        /// <summary>
        /// Closes the underlying stream. Equivalent to <see cref="Dispose"/>.
        /// </summary>
        public void Close() => Dispose();

        // -------------------------------------------------------------------------
        // IDisposable
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!disposed)
            {
                reader?.Close();
                reader = null;
                archiveStream?.Dispose();
                archiveStream = null;
                disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------------
        // Internal helpers
        // -------------------------------------------------------------------------

        private bool TryExtractBytes(string name, out byte[]? result)
        {
            result = null;
            if (reader == null)
                throw new ObjectDisposedException(nameof(PAKFile), "Archive is closed.");

            string lowerName = name.ToLower();
            if (entriesTable[lowerName] is not Entry entry || entry.FileOffset == 0)
                return false;

            reader.BaseStream.Seek(entry.FileOffset, SeekOrigin.Begin);
            result = ReadCompressedStream();
            return true;
        }

        /// <summary>
        /// Decompresses one zlib-deflate stream from the current reader position.
        /// The reader is left positioned immediately after the compressed data.
        /// </summary>
        private byte[] ReadCompressedStream()
        {
            if (reader == null)
                throw new ObjectDisposedException(nameof(PAKFile));

            var inflater   = new Inflater();
            var inputBuf   = new byte[1024];
            var outputBuf  = new byte[1024];
            int outputUsed = 0;

            while (!inflater.IsFinished)
            {
                while (inflater.IsNeedingInput)
                {
                    int count = reader.Read(inputBuf, 0, inputBuf.Length);
                    if (count <= 0)
                        throw new EndOfStreamException("Unexpected end of file inside compressed stream.");
                    inflater.SetInput(inputBuf, 0, count);
                }

                if (outputUsed == outputBuf.Length)
                {
                    var larger = new byte[outputBuf.Length * 2];
                    Array.Copy(outputBuf, larger, outputBuf.Length);
                    outputBuf = larger;
                }

                try
                {
                    outputUsed += inflater.Inflate(outputBuf, outputUsed, outputBuf.Length - outputUsed);
                }
                catch (FormatException ex)
                {
                    throw new IOException("Decompression error: " + ex.Message, ex);
                }
            }

            // Rewind the reader past only the bytes that were consumed by the inflater.
            reader.BaseStream.Seek(-inflater.RemainingInput, SeekOrigin.Current);
            inflater.Reset();

            var output = new byte[outputUsed];
            Array.Copy(outputBuf, output, outputUsed);
            return output;
        }

        // ---- Little-endian binary helpers ----

        private static long ReadInt32LE(byte[] buf, int offset)
            => (buf[offset + 3] << 24) | (buf[offset + 2] << 16) | (buf[offset + 1] << 8) | buf[offset];

        private static ulong ReadUInt64LE(byte[] buf, int offset)
            => ((ulong)buf[offset + 7] << 56) | ((ulong)buf[offset + 6] << 48)
             | ((ulong)buf[offset + 5] << 40) | ((ulong)buf[offset + 4] << 32)
             | ((ulong)buf[offset + 3] << 24) | ((ulong)buf[offset + 2] << 16)
             | ((ulong)buf[offset + 1] <<  8) |  (ulong)buf[offset];
    }

    /// <summary>Represents a single file entry inside an MPAK archive.</summary>
    public struct Entry
    {
        public long     unknown1;
        public long     unknown2;
        public long     CompressedSize;
        public ulong    unknown3;
        public long     UncompressedSize;
        internal long   FileOffset;
        public string   FileName;
        public DateTime dateCreation;
    }
}
