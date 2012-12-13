using System;
using System.IO;


namespace Hodmongler {

	public struct Vector2 {
		public float x;
		public float y;
		
		public Vector2(float a, float b) {
			x = a;
			y = b;
		}
	}
	public struct Vector3 {
		public float x;
		public float y;
		public float z;
		
		public Vector3(float a, float b, float c) {
			x = a;
			y = b;
			z = c;
		}
	}
	public struct Vector4 {
		public float w;
		public float x;
		public float y;
		public float z;
		public Vector4(float a, float b, float c, float d) {
			w = a;
			x = b;
			y = c;
			z = d;
		}
	}
	
	public class HodChunk {
		public string Kind {get;set;}
		public Int32 Length {get;set;}
		
		public override string ToString() {
			return string.Format("(UNKNOWN {0} {1})", Kind, Length);
		}
		
		public HodChunk() {}
		public HodChunk(HodReader r) {}
	}
	
	public class FormChunk : HodChunk {
		public HodChunk Contents;
		
		public FormChunk(HodReader r) {
			Kind = "FORM";
			Length = r.ReadInt32();
			Contents = r.ReadChunk();
		}
		
		public override string ToString() {
			return string.Format("({0} {1})", Kind, Contents);
		}

	}
	
	public class VersChunk : HodChunk {
		public Int32 Version;
		public VersChunk(HodReader r) {
			Kind = "VERS";
			Version = r.ReadInt32();
		}
		
		public override string ToString() {
			return string.Format("({0} {1})", Kind, Version);
		}
	}
	
	public class NrmlChunk : HodChunk {
		public HodChunk Contents;
		public NrmlChunk(HodReader r) {
			Kind = "NRML";
			Length = r.ReadInt32();
			Contents = r.ReadChunk();
		}
		
	public override string ToString() {
			return string.Format("({0} {1})", Kind, Contents);
		}
	}
	
	public class BgltChunk : HodChunk {
		public BgltChunk(HodReader r) {
			Kind = "BGLT";
			Length = r.ReadInt32();
		}
		
		public override string ToString() {
			return string.Format("({0} {1})", Kind, Length);
		}
	}
	
	
	public class BgmsChunk : HodChunk {
		public HodChunk Contents;
		public BgmsChunk(HodReader r) {
			Kind = "BGMS";
			Length = 0;
			Contents = r.ReadChunk();
		}
		
		public override string ToString() {
			return string.Format("({0} {1})", Kind, Contents);
		}
	}
	
	public class FatVertex {
		public Vector4 Vertex;
		public Vector4 Normal;
		public Int32 Color;
		public Vector2 Texture;
		public Vector3 Tangent;
		public Vector3 Binormal;
		
		public FatVertex(HodReader r, Int16 vertexFlags) {
			if((vertexFlags & 1) != 0) // 1 = HW2VF_VertexBit
				Vertex = r.ReadVector4();
			if((vertexFlags & 4) != 0) // 4 = HW2VF_ColourBit
				Color = r.ReadInt32();
			else
				Color = -1;
			if((vertexFlags & 2) != 0) // 2 = HW2VF_NormalBit
				Normal = r.ReadVector4();
			if((vertexFlags & 8) != 0) // 8 = Texture0Bit
				Texture = r.ReadVector2();
			if((vertexFlags & 8192) != 0) // TangentBit
				Tangent = r.ReadVector3();
			if((vertexFlags & 16384) != 0) // BinormalBit
				Binormal = r.ReadVector3();
		}
	}
	
	// XXX: Write proper enums for flags!
	public class Prim {
		public Int16 FaceType; // XXX: Should be an enum
		public Int32 NumIndices;
		public Int16[] Indices;
		
		public Prim(HodReader r) {
			FaceType = r.ReadInt16();
			NumIndices = r.ReadInt32();
			for(int i = 0; i < NumIndices; i++) {
				Indices[i] = r.ReadInt16();
			}
		}
	}
	 // XXX: May be fraught with sign errors; some things may or may not be unsigned.
	public class BmshGroup {
		public Int32 MatIndex;
		public Int16 VertexFlags; // XXX: Should be an enum
		public Int32 NumVerts;
		public FatVertex[] Verts;
		public Int16 NumPrims;
		public Prim[] Prims;
		
		public BmshGroup(HodReader r) {
			MatIndex = r.ReadInt32();
			VertexFlags = r.ReadInt16(); // XXX: May be longer?
			NumVerts = r.ReadInt32();
			Verts = new FatVertex[NumVerts];
			for(int i = 0; i < NumVerts; i++) {
				Verts[i] = new FatVertex(r, VertexFlags);
			}
			NumPrims = r.ReadInt16();
			Prims = new Prim[NumPrims];
			for(int i = 0; i < NumPrims; i++) {
				Prims[i] = new Prim(r);
			}
		}
	}
	
	// XXX: Some of the lengths here are probably not correct, causing OutOfMemory when trying to create arrays.
	public class BmshChunk : HodChunk {
		public Int32 MeshLod;
		public Int32 NumMeshes;
		public BmshGroup[] Mesh;
		public BmshChunk(HodReader r) {
			Kind = "BMSH";
			Length = 0; // r.ReadInt32();
			MeshLod = r.ReadInt32();
			NumMeshes = r.ReadInt32();
			Mesh = new BmshGroup[NumMeshes];
			for(int i = 0; i < NumMeshes; i++) {
				Mesh[i] = new BmshGroup(r);
			}
		}
		
		public override string ToString() {
			return string.Format("({0} {1} {2} {3})", Kind, MeshLod, NumMeshes, Mesh);
		}
	}
	
	/* This is BAFFLINGLY incomplete in cfhoded.
	public class VaryWedge {
	}
	
	public class VaryFace {
	}
	
	public class VaryCollapseFixup {
		public Int32 Typ;
		public Int32 data0;
		public Int16 data1;
		public Int16 data1;
		
		public VaryCollapseFixup(Int32 t, Int32 d0, Int16 d1, Int16 d2) {
			Typ = t;
			data0 = d0;
			data1 = d1;
			data2 = d2;
		}
	}
	
	public class VaryCollapse {
		public Int32 NumFixUps;
		public 
	}
	
	public class VaryChunk : HodChunk {
		public string Name;
		public string ParentName;
		public Int32 NumVerts;
		public Int32 NumWedges;
		public Int32 NumFaces;
		public Int32 NumCollapses;
		public Int32 NumUVs;
		
		public Vector3[] Vertices;
		public VaryWedge[] Wedges;
		public VaryFace[] Faces;
		public VaryCollapse[] Collapses;
		
		public VaryChunk(HodReader r) {
			Kind = "VARY";
			Name = r.ReadString();
			ParentName = r.ReadString();
			NumVerts = r.ReadInt32();
			NumWedges = r.ReadInt32();
			NumFaces = r.ReadInt32();
			NumCollapses = r.ReadInt32();
			NumUVs = r.ReadInt32();
			
			Vertices = new Vector3[NumVerts];
			Wedges = new VaryWedge[NumWedges];
			Faces = new VaryFace[NumFaces];
			Collapses = new VaryCollapse[NumCollapses];
			
			for(int i = 0; i < NumCollapses; i++) {
				Collapses[i] = new VaryCollapse(r);
			}
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	*/
	
	public class SimpVert {
		public Vector4 Point;
		public Vector4 Normal;
		public Vector2 UV;  // Not sure this is the UV, but what else would it be?
		
		public SimpVert(Vector4 p, Vector4 n, Vector2 u) {
			Point = p;
			Normal = n;
			UV = u;
		}
	}
	
	public class SimpChunk : HodChunk {
		public string Name;
		public Int32 NumVerts;
		public Int32 NumFaces;
		public SimpVert[] Verts;
		public Int32[] Faces;
		public SimpChunk(HodReader r) {
			Kind = "SIMP";
			Length = 0;
			Name = r.ReadString();
			NumVerts = r.ReadInt32();
			Verts = new SimpVert[NumVerts];
			for(int i = 0; i < NumVerts; i++) {
				Vector4 v = r.ReadVector4();
				Vector4 n = r.ReadVector4();
				Vector2 u = r.ReadVector2();
				Verts[i] = new SimpVert(v, n, u);
			}
			
			Faces = new Int32[NumFaces];
			for(int i = 0; i < NumFaces; i++) {
				Faces[i] = r.ReadInt32();
			}
		}
		
		public override string ToString() {
			return string.Format("({0} {1} {2})", Kind, Verts, Faces);
		}
	}
	
	/*
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	
	public class Chunk : HodChunk {
		public Chunk(HodReader r) {
			
		}
		
		public override string ToString() {
			return string.Format("({0} )", Kind);
		}
	}
	*/
}
