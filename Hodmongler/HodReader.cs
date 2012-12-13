using System;
using System.IO;
using System.Collections.Generic;


namespace Hodmongler {
	
	public class HodReadException : Exception {
		public HodReadException(string s) : base(s) {}
	}
	
	
	/// <summary>
	/// Provides a nice way to read a HOD file by chunks, and also functions to handily read individual elements like
	/// numbers and vectors, doing byte-swapping and such where necessary.
	/// </summary>
	public class HodReader { 
		BinaryReader r;
		
		// XXX: Where do we close the file?
		public HodReader(string filename) {
			FileStream stream = File.Open(filename, FileMode.Open);
			r = new BinaryReader(stream);
		}
		
		public HodReader(Stream stream) {
			r = new BinaryReader(stream);
		}
		public HodReader(BinaryReader reader) {
			r = reader;
		}
		
		public HodChunk ReadChunk() {
			if((r.BaseStream.Position+4) > r.BaseStream.Length) {
				throw new HodReadException("Tried to read a chunk but there's <4 bytes left in the stream!");
			}
			string id = ReadID();
			switch(id) {
			case "FORM":
				debug("FORM chunk");
				return new FormChunk(this);
			case "VERS":
				debug("VERS chunk");
				return new VersChunk(this);
			case "NRML":
				debug("NRML chunk");
				return new NrmlChunk(this);
			case "BGLT":
				debug("BGLT chunk");
				return new BgltChunk(this);
			case "BGMS":
				debug("BGMS chunk");
				return new BgmsChunk(this);
			case "BMSH":
				debug("BMSH chunk");
				return new BmshChunk(this);
			default:
				debug("Warning!  Unknown chunk.");
				return new HodChunk(this);
			}
		}
		
		public HodChunk[] ReadChunks() {
			List<HodChunk> chunks = new List<HodChunk>();
			try {
				while(r.BaseStream.Position < r.BaseStream.Length) {
					chunks.Add(ReadChunk());
				}
			} catch(HodReadException e) {
				Console.WriteLine("WARNING:");
				Console.WriteLine(e);
			}
			return chunks.ToArray();
		}
		
		static void debug(string s) {
			Console.WriteLine(s);
		}
		
		// XXX: Sign-extension shenanigans?
		public static Int32 BigToLittleEndian(Int32 i) {
			Int32 b1 = (i&0x000000ff) << 24;
			Int32 b2 = (i&0x0000ff00) << 8;
			Int32 b3 = (i&0x00ff0000) >> 8;
			Int32 b4 = (i>>24)&0xff;
			return b1 + b2 + b3 + b4;

		}
		public static Int16 BigToLittleEndian(Int16 i) {
			Int32 tmp = (Int32) i;
			Int32 b1 = (tmp<<8)&0xff00;
			Int32 b2 = (tmp>>8)&0x00ff;
			tmp = b1 + b2;
			return (Int16) tmp;
		}
		
		public string ReadID() {
			char[] id = new char[4];
			id[0] = (char) r.ReadByte();
			id[1] = (char) r.ReadByte();
			id[2] = (char) r.ReadByte();
			id[3] = (char) r.ReadByte();
			return new string(id);
		}
		
		public Int16 ReadInt16() {
			return BigToLittleEndian(r.ReadInt16());
		}
		
		public Int32 ReadInt32() {
			return BigToLittleEndian(r.ReadInt32());
		}
		
		// XXX: Hopefully no endian buggery happens here.
		public float ReadFloat32() {
			return r.ReadSingle();
		}
		public double ReadFloat64() {
			return r.ReadDouble();
		}
		public Vector2 ReadVector2() {
			float x = ReadFloat32();
			float y = ReadFloat32();
			return new Vector2(x, y);
		}
		public Vector3 ReadVector3() {
			float x = ReadFloat32();
			float y = ReadFloat32();
			float z = ReadFloat32();
			return new Vector3(x, y, z);
		}
		public Vector4 ReadVector4() {
			float w = ReadFloat32();
			float x = ReadFloat32();
			float y = ReadFloat32();
			float z = ReadFloat32();
			return new Vector4(w, x, y, z);
		}
		
		public string ReadString() {
			return r.ReadString();
		}
		
	}
}
