using System;

namespace Hodmongler {
	class MainClass {
		static void PrintArray<T>(T[] arr) {
			foreach(T item in arr) {
				Console.WriteLine(item);
			}
		}
		public static void Main(string[] args) {
			Console.WriteLine("File 1:");
			//PrintArray(new HodReader("../../../tests/Higaara_Outskirts.HOD").ReadChunks());
			Console.WriteLine("File 2:");
			//PrintArray(new HodReader("../../../tests/Higaara_Outskirts_light.HOD").ReadChunks());
			Console.WriteLine("File 3:");
			PrintArray(new HodReader("../../../tests/HWAT_UNH_NCHarvester.hod").ReadChunks());
			Console.WriteLine("File 4:");
			//PrintArray(new HodReader("../../../tests/sensorsphere.hod").ReadChunks());
			
			if(args.Length < 1) {
				Usage();
				return;
			}
		}
		
		public static void Usage() {
			string usage = "\n" +
				"Usage: hodmongler file.hod\n" +
				"\n";
			Console.Write(usage);
		}
	}
}
