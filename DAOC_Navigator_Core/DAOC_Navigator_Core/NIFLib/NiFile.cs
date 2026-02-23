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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a parsed NIF file and all its NiObjects, indexed by reference id.
    /// </summary>
    public class NiFile
    {
        /// <summary>
        /// Sentinel value for an invalid / null NIF reference (0xFFFFFFFF).
        /// </summary>
        public const uint INVALID_REF = 0xFFFFFFFFu;

        /// <summary>
        /// Block type string used in pre-3.3.0.13 files to signal the top-level object list.
        /// </summary>
        public const string CMD_TOP_LEVEL_OBJECT = "Top Level Object";

        /// <summary>
        /// Block type string used in pre-3.3.0.13 files to signal the end of the object list.
        /// </summary>
        public const string CMD_END_OF_FILE = "End Of File";

        /// <summary>File header.</summary>
        public NiHeader Header;

        /// <summary>File footer (root node references).</summary>
        public NiFooter Footer;

        /// <summary>All parsed NiObjects keyed by their reference id.</summary>
        public Dictionary<uint, NiObject>? ObjectsByRef;

        /// <summary>NIF format version, delegated from the header.</summary>
        public eNifVersion Version => this.Header.Version;

        /// <summary>
        /// Parses a NIF file from the supplied binary reader.
        /// </summary>
        /// <param name="reader">Binary reader positioned at the start of the file.</param>
        public NiFile(BinaryReader reader)
        {
            this.Header = new NiHeader(this, reader);
            this.ReadNiObjects(reader);
            this.Footer = new NiFooter(this, reader);
            this.FixRefs();
        }

        // -------------------------------------------------------------------------
        // Object reading
        // -------------------------------------------------------------------------

        /// <summary>
        /// Reads all NiObject blocks from the file.
        /// Unknown block types are skipped (logged as warnings) instead of throwing.
        /// </summary>
        private void ReadNiObjects(BinaryReader reader)
        {
            this.ObjectsByRef = new Dictionary<uint, NiObject>();
            int blockIndex = 0;
            string blockTypeName = string.Empty;

            while (true)
            {
                // --- Determine block type name ---
                if (this.Version >= eNifVersion.VER_5_0_0_1)
                {
                    // Versions 5.0.0.1 → 10.1.0.106 write a check uint that must be zero.
                    if (this.Version <= eNifVersion.VER_10_1_0_106)
                    {
                        uint checkValue = reader.ReadUInt32();
                        if (checkValue != 0u)
                            throw new InvalidDataException("Check value is not zero – file may be corrupt.");
                    }

                    if (Header.BlockTypes != null && Header.BlockTypeIndex != null)
                        blockTypeName = this.Header.BlockTypes[(int)this.Header.BlockTypeIndex[blockIndex]].Value;
                }
                else
                {
                    uint typeLen = reader.ReadUInt32();
                    if (typeLen < 6u || typeLen > 30u)
                        throw new InvalidDataException($"Invalid object type string length ({typeLen}) – file may be corrupt.");

                    blockTypeName = new string(reader.ReadChars((int)typeLen));

                    if (this.Header.Version < eNifVersion.VER_3_3_0_13)
                    {
                        if (blockTypeName == CMD_TOP_LEVEL_OBJECT) continue;
                        if (blockTypeName == CMD_END_OF_FILE) return;
                    }
                }

                // --- Determine reference key ---
                uint refKey = (this.Version < eNifVersion.VER_3_3_0_13)
                    ? reader.ReadUInt32()
                    : (uint)blockIndex;

                // --- Instantiate object via reflection ---
                Type? objectType = Type.GetType("Niflib." + blockTypeName);
                if (objectType == null)
                {
                    // Unknown block type: skip it using the declared block size when available.
                    SkipUnknownBlock(reader, blockIndex, blockTypeName);
                }
                else
                {
                    NiObject? instance = (NiObject?)Activator.CreateInstance(objectType, new object[] { this, reader });
                    if (instance != null)
                        this.ObjectsByRef[refKey] = instance;
                }

                // --- Advance block counter (version >= 3.3.0.13) ---
                if (this.Version >= eNifVersion.VER_3_3_0_13)
                {
                    blockIndex++;
                    if ((ulong)blockIndex >= (ulong)this.Header.NumBlocks)
                        return;
                }
            }
        }

        /// <summary>
        /// Skips an unknown block by seeking past its declared size, or stops parsing
        /// if no size information is available (pre-10.0.1.0 files).
        /// </summary>
        private void SkipUnknownBlock(BinaryReader reader, int blockIndex, string blockTypeName)
        {
            Debug.WriteLine($"[NiFile] Warning: unknown block type '{blockTypeName}' at index {blockIndex} – skipping.");

            if (this.Header.BlockSizes != null && blockIndex < this.Header.BlockSizes.Length)
            {
                // BlockSizes available (version >= 20.2.0.7): seek forward by the declared size.
                reader.BaseStream.Seek(this.Header.BlockSizes[blockIndex], SeekOrigin.Current);
            }
            else
            {
                // No size information available – we cannot safely skip; abort parsing.
                throw new NotSupportedException(
                    $"Unknown NIF block type '{blockTypeName}' encountered and no block size information is available to skip it.");
            }
        }

        // -------------------------------------------------------------------------
        // Reference fixup
        // -------------------------------------------------------------------------

        /// <summary>
        /// Resolves all NiRef&lt;T&gt; fields throughout the object graph after loading.
        /// Uses typed reflection instead of dynamic dispatch.
        /// </summary>
        private void FixRefs()
        {
            if (ObjectsByRef != null)
            {
                foreach (NiObject obj in this.ObjectsByRef.Values)
                    FixRefsOnObject(obj);
            }

            if (Footer.RootNodes != null)
            {
                foreach (var niRef in Footer.RootNodes)
                    niRef.SetRef(this);
            }
        }

        /// <summary>
        /// Reflects over every field of <paramref name="obj"/> and resolves any
        /// <c>NiRef&lt;T&gt;</c> or <c>NiRef&lt;T&gt;[]</c> field it finds.
        /// </summary>
        private void FixRefsOnObject(object obj)
        {
            foreach (FieldInfo field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!field.FieldType.Name.Contains("NiRef"))
                    continue;

                if (field.FieldType.IsArray)
                {
                    // Array of NiRef<T>
                    if (field.GetValue(obj) is not System.Collections.IEnumerable enumerable)
                        continue;

                    foreach (object? element in enumerable)
                    {
                        if (element == null) continue;

                        // Call SetRef(NiFile) via the concrete NiRef<T> type.
                        InvokeSetRef(element);

                        // Maintain parent link for child nodes.
                        if (field.Name == "Children")
                            TrySetParent(element, obj);
                    }
                }
                else
                {
                    // Scalar NiRef<T>
                    object? value = field.GetValue(obj);
                    if (value == null) continue;

                    // Skip if RefId is the sentinel INVALID_REF value.
                    if (GetRefId(value) == INVALID_REF) continue;

                    InvokeSetRef(value);
                }
            }
        }

        /// <summary>
        /// Calls <c>SetRef(NiFile)</c> on a NiRef&lt;T&gt; instance through reflection.
        /// </summary>
        private void InvokeSetRef(object niRefInstance)
        {
            niRefInstance.GetType().GetMethod("SetRef")?.Invoke(niRefInstance, new object[] { this });
        }

        /// <summary>
        /// Returns the <c>RefId</c> property value of a NiRef&lt;T&gt; instance.
        /// </summary>
        private static uint GetRefId(object niRefInstance)
        {
            return (uint)(niRefInstance.GetType().GetProperty("RefId")?.GetValue(niRefInstance) ?? INVALID_REF);
        }

        /// <summary>
        /// Assigns the parent NiNode reference on a child NiAVObject when resolving the
        /// Children array of a NiNode.
        /// </summary>
        private static void TrySetParent(object niRefElement, object parentObject)
        {
            bool? isValid = (bool?)niRefElement.GetType().GetMethod("IsValid")?.Invoke(niRefElement, null);
            if (isValid != true) return;

            object? child = niRefElement.GetType().GetProperty("Object")?.GetValue(niRefElement);
            if (child is NiAVObject avObject && parentObject is NiNode parentNode)
                avObject.Parent = parentNode;
        }

        // -------------------------------------------------------------------------
        // Public helpers
        // -------------------------------------------------------------------------

        /// <summary>
        /// Finds the root NiAVObject by walking up the parent chain from the first
        /// object in the dictionary.
        /// </summary>
        public NiAVObject? FindRoot()
        {
            if (ObjectsByRef == null) return null;

            NiAVObject? root = ObjectsByRef.Values.OfType<NiAVObject>().FirstOrDefault();
            if (root == null) return null;

            while (root.Parent != null)
                root = root.Parent;

            return root;
        }

        // -------------------------------------------------------------------------
        // Debug helpers (private)
        // -------------------------------------------------------------------------

        private void PrintNifTree()
        {
            NiAVObject? root = this.FindRoot();
            if (root == null) { Debug.WriteLine("No Root!"); return; }
            PrintNifNode(root, 0);
        }

        private void PrintNifNode(NiAVObject root, int depth)
        {
            string indent = new string('*', depth) + " ";
            Debug.WriteLine(indent + root.Name);

            if (root is NiNode node)
            {
                foreach (NiRef<NiAVObject> childRef in node.Children)
                {
                    if (childRef.IsValid() && childRef.Object != null)
                        PrintNifNode(childRef.Object, depth + 1);
                }
            }
        }
    }
}
