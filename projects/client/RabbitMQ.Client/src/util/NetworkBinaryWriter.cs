// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (c) 2007-2016 Pivotal Software, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       https://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//  The contents of this file are subject to the Mozilla Public License
//  Version 1.1 (the "License"); you may not use this file except in
//  compliance with the License. You may obtain a copy of the License
//  at https://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS"
//  basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
//  the License for the specific language governing rights and
//  limitations under the License.
//
//  The Original Code is RabbitMQ.
//
//  The Initial Developer of the Original Code is Pivotal Software, Inc.
//  Copyright (c) 2007-2016 Pivotal Software, Inc.  All rights reserved.
//---------------------------------------------------------------------------

using System.IO;
using System.Text;

namespace RabbitMQ.Util
{
    /// <summary>
    /// Subclass of BinaryWriter that writes integers etc in correct network order.
    /// </summary>
    ///
    /// <remarks>
    /// <p>
    /// Kludge to compensate for .NET's broken little-endian-only BinaryWriter.
    /// </p><p>
    /// See also NetworkBinaryReader.
    /// </p>
    /// </remarks>
    public class NetworkBinaryWriter : BinaryWriter
    {
        private static readonly Encoding encoding = new UTF8Encoding(false, true);

        /// <summary>
        /// Construct a NetworkBinaryWriter over the given input stream.
        /// </summary>
        public NetworkBinaryWriter(Stream output) : base(output, encoding)
        {
        }

        /// <summary>
        /// Construct a NetworkBinaryWriter over the given input
        /// stream, reading strings using the given encoding.
        /// </summary>
        public NetworkBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
        {
        }

        ///<summary>Helper method for constructing a temporary
        ///BinaryWriter streaming into a fresh MemoryStream
        ///provisioned with the given initialSize.</summary>
        public static BinaryWriter TemporaryBinaryWriter(int initialSize)
        {
            return new BinaryWriter(new MemoryStream(initialSize));
        }

        ///<summary>Helper method for extracting the byte[] contents
        ///of a BinaryWriter over a MemoryStream, such as constructed
        ///by TemporaryBinaryWriter.</summary>
        public static byte[] TemporaryContents(BinaryWriter w)
        {
            return ((MemoryStream)w.BaseStream).ToArray();
        }

        /// <summary>
        /// Override BinaryWriter's method for network-order.
        /// </summary>
        public override void Write(short i)
        {
            Write((byte)((i & 0xFF00) >> 8));
            Write((byte)(i & 0x00FF));
        }

        /// <summary>
        /// Override BinaryWriter's method for network-order.
        /// </summary>
        public override void Write(ushort i)
        {
            Write((byte)((i & 0xFF00) >> 8));
            Write((byte)(i & 0x00FF));
        }

        /// <summary>
        /// Override BinaryWriter's method for network-order.
        /// </summary>
        public override void Write(int i)
        {
            Write((byte)((i & 0xFF000000) >> 24));
            Write((byte)((i & 0x00FF0000) >> 16));
            Write((byte)((i & 0x0000FF00) >> 8));
            Write((byte)(i & 0x000000FF));
        }

        /// <summary>
        /// Override BinaryWriter's method for network-order.
        /// </summary>
        public override void Write(uint i)
        {
            Write((byte)((i & 0xFF000000) >> 24));
            Write((byte)((i & 0x00FF0000) >> 16));
            Write((byte)((i & 0x0000FF00) >> 8));
            Write((byte)(i & 0x000000FF));
        }

        /// <summary>
        /// Override BinaryWriter's method for network-order.
        /// </summary>
        public override void Write(long i)
        {
            var i1 = (uint)(i >> 32);
            var i2 = (uint)i;
            Write(i1);
            Write(i2);
        }

        /// <summary>
        /// Override BinaryWriter's method for network-order.
        /// </summary>
        public override void Write(ulong i)
        {
            var i1 = (uint)(i >> 32);
            var i2 = (uint)i;
            Write(i1);
            Write(i2);
        }

        /// <summary>
        /// Override BinaryWriter's method for network-order.
        /// </summary>
        public override void Write(float f)
        {
            BinaryWriter w = TemporaryBinaryWriter(4);
            w.Write(f);
            byte[] wrongBytes = TemporaryContents(w);
            Write(wrongBytes[3]);
            Write(wrongBytes[2]);
            Write(wrongBytes[1]);
            Write(wrongBytes[0]);
        }

        /// <summary>
        /// Override BinaryWriter's method for network-order.
        /// </summary>
        public override void Write(double d)
        {
            BinaryWriter w = TemporaryBinaryWriter(8);
            w.Write(d);
            byte[] wrongBytes = TemporaryContents(w);
            Write(wrongBytes[7]);
            Write(wrongBytes[6]);
            Write(wrongBytes[5]);
            Write(wrongBytes[4]);
            Write(wrongBytes[3]);
            Write(wrongBytes[2]);
            Write(wrongBytes[1]);
            Write(wrongBytes[0]);
        }
    }
}
