﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WolvenKit.Interfaces;

namespace WolvenKit.Bundles
{
    public class Bundle : IWitcherArchiveType
    {
        private static readonly byte[] IDString =
        {
            (byte) 'P', (byte) 'O', (byte) 'T', (byte) 'A',
            (byte) 'T', (byte) 'O', (byte) '7', (byte) '0'
        };

        private static int HEADER_SIZE = 32;
        private static int ALIGNMENT_TARGET = 4096;
        private static string FOOTER_DATA = "AlignmentUnused"; //The bundle's final filesize should be an even multiple of 16; garbage data should be appended at the end if necessary to make this happen [appears to be unnecessary/optional, as far as the game cares]
        private static int TOCEntrySize = 100 + 16 + 4 + 4 + 4 + 4 + 8 + 16 + 4 + 4; //Size of a TOC Entry.

        private uint bundlesize;
        private uint dataoffset;
        private uint dummysize;

        public Bundle(string filename)
        {
            FileName = filename;

            ReadTOC();
        }

        public Bundle()
        {

        }

        public string TypeName { get { return "Bundle"; } }
        public string FileName { get; set; }
        public Dictionary<string, BundleItem> Items { get; set; }
        
        /// <summary>
        /// Reads the Table Of Contents of the bundle.
        /// </summary>
        private void ReadTOC()
        {
            Items = new Dictionary<string, BundleItem>();

            using (var reader = new BinaryReader(new FileStream(FileName, FileMode.Open, FileAccess.Read)))
            {
                var idstring = reader.ReadBytes(IDString.Length);

                if (!IDString.SequenceEqual(idstring))
                {
                    throw new InvalidBundleException("Bundle header mismatch.");
                }

                bundlesize = reader.ReadUInt32();
                dummysize = reader.ReadUInt32();
                dataoffset = reader.ReadUInt32();

                reader.BaseStream.Seek(0x20, SeekOrigin.Begin);

                while (reader.BaseStream.Position < dataoffset + 0x20)
                {
                    var item = new BundleItem
                    {
                        Bundle = this
                    };

                    var strname = Encoding.Default.GetString(reader.ReadBytes(0x100));

                    item.Name = strname.Substring(0, strname.IndexOf('\0'));
                    item.Hash = reader.ReadBytes(16);
                    item.Empty = reader.ReadUInt32();
                    item.Size = reader.ReadUInt32();
                    item.ZSize = reader.ReadUInt32();
                    item.Offset = reader.ReadUInt32();

                    var date = reader.ReadUInt32();
                    var y = date >> 20;
                    var m = date >> 15 & 0x1F;
                    var d = date >> 10 & 0x1F;

                    var time = reader.ReadUInt32();
                    var h = time >> 22;
                    var n = time >> 16 & 0x3F;
                    var s = time >> 10 & 0x3F;

                    item.DateString = string.Format(" {0}/{1}/{2} {3}:{4}:{5}", d, m, y, h, n, s);

                    item.Zero = reader.ReadBytes(16);    //00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 (always, in every archive)
                    item.CRC = reader.ReadUInt32();    //CRC32 for the uncompressed data
                    item.Compression = reader.ReadUInt32();

                    Items.Add(item.Name, item);
                }


                reader.Close();
            }
        }

        /// <summary>
        /// Packs files to a bundle.
        /// </summary>
        /// <param name="Outputpath">The path to save the bundle to with the packed files.</param>
        /// <param name="Files">The Files to pack</param>
        public static void Write(string Outputpath, string rootfolder)
        {
            using (var fs = new FileStream(Outputpath, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
            {
                var bundlesize = 8192; //TODO Calculate the resulting bundle's size.
                var dummysize = 0; //May not need to be recomputed. TODO: Investigate
                var dataoffset = 320;
                var Offset = 48;

                bw.Write(IDString);
                bw.Write(bundlesize);
                bw.Write(dummysize);
                bw.Write(dataoffset);
                bw.Write(new byte[] { 0x03, 0x00, 0x01, 0x00, 0x00, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13, 0x13 }); //TODO: Figure out what the hell is this.
                Offset += HEADER_SIZE;
                foreach (var f in Directory.EnumerateFiles(rootfolder,"*",SearchOption.AllDirectories))
                {
                    Offset += 164;
                    var name = Encoding.Default.GetBytes(Path.GetFileName(GetRelativePath(f,rootfolder))).ToArray();
                    if (name.Length > 0x100)
                        name = name.Take(0x100).ToArray();
                    if(name.Length < 0x100)
                        Array.Resize(ref name, 0x100);
                    bw.Write(name); //Filename trimmed to 100 characters.
                    bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }); //HASH
                    bw.Write((UInt32)0x00000000); //EMPTY
                    bw.Write((UInt32)new FileInfo(f).Length); //SIZE
                    bw.Write((UInt32)new FileInfo(f).Length); //ZSIZE
                    bw.Write((UInt32)Offset); //OFFSET
                    bw.Write((UInt32)0x00000000); //DATE
                    bw.Write((UInt32)0x00000000); //TIME
                    bw.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }); //PADDING
                    bw.Write((UInt32)Crc32C.Crc32CAlgorithm.Compute(File.ReadAllBytes(f))); //CRC32
                    bw.Write((UInt32)0x00000000); // Compression. We don't compress it so 0.
                    Offset += (int)new FileInfo(f).Length;
                    Debug.WriteLine("Pos: " + bw.BaseStream.Position);
                }
                foreach (var item in Directory.EnumerateFiles(rootfolder, "*", SearchOption.AllDirectories))
                {
                    bw.Write(File.ReadAllBytes(item));
                }
            }
            MessageBox.Show("Done writing file!");
        }

        /// <summary>
        /// Gets relative path from absolute path.
        /// </summary>
        /// <param name="filespec">A files path.</param>
        /// <param name="folder">The folder's path.</param>
        /// <returns></returns>
        public static string GetRelativePath(string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}