using System;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiver;

namespace UnitTests
{
	[TestClass]
	public class HDZTests
	{
		private ulong Checksum(Stream stream)
		{
			UInt64 res = 0, len = (UInt64)stream.Length;
			using (var b = new BinaryReader(stream))
			{
				while (stream.Position + 7 < stream.Length)
					res ^= b.ReadUInt64();
			}
			return Math.Min(res + len, ulong.MaxValue);
		}

		[TestMethod]
		public void ChecksumCalcValidator()
		{
			Assert.AreEqual((UInt64)4050765991979987545, Checksum(new MemoryStream(Encoding.ASCII.GetBytes("1234567812345678123456781234567812345678"))));
			Assert.AreEqual((UInt64)4050765991979987546, Checksum(new MemoryStream(Encoding.ASCII.GetBytes("12345678123456781234567812345678123456781"))));
			Assert.AreEqual((UInt64)4050765991979987513, Checksum(new MemoryStream(Encoding.ASCII.GetBytes("12345678"))));
			Assert.AreEqual((UInt64)7, Checksum(new MemoryStream(Encoding.ASCII.GetBytes("1234567"))));
			Assert.AreEqual((UInt64)6, Checksum(new MemoryStream(Encoding.ASCII.GetBytes("123456"))));
			Assert.AreEqual((UInt64)5, Checksum(new MemoryStream(Encoding.ASCII.GetBytes("12345"))));  
			Assert.AreEqual((UInt64)4, Checksum(new MemoryStream(Encoding.ASCII.GetBytes("1234"))));
			Assert.AreEqual((UInt64)3, Checksum(new MemoryStream(Encoding.ASCII.GetBytes("123"))));
			Assert.AreEqual((UInt64)2, Checksum(new MemoryStream(Encoding.ASCII.GetBytes("12"))));  
			Assert.AreEqual((UInt64)1, Checksum(new MemoryStream(Encoding.ASCII.GetBytes("1"))));
			Assert.AreEqual((UInt64)0, Checksum(new MemoryStream(Encoding.ASCII.GetBytes(""))));  
			byte[] arr = new byte[4095];
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = (byte)(i%10);
			}
			byte[] arr2 = new byte[4096];
			byte[] arr3 = new byte[4097];
			arr.CopyTo(arr2, 0);
			arr.CopyTo(arr3, 0);
			arr2[4095] = 8;
			arr3[4095] = 124;
			arr3[4096] = 227;
			Assert.AreEqual((UInt64)506097522914234623, Checksum(new MemoryStream(arr)));
			Assert.AreEqual((UInt64)1081433483395602440, Checksum(new MemoryStream(arr2)));
			Assert.AreEqual((UInt64)8863653639491819529, Checksum(new MemoryStream(arr3)));
		}

		[TestMethod]
		public void SmallHdzValidator()
		{
			HdzValidator(600);
		}

		[TestMethod]
		public void MediumHdzValidator()
		{
			HdzValidator(600 * 1024);
		}

		[TestMethod]
		public void BigHdzValidator()
		{
			HdzValidator(600 * 1024 * 100);
		}

		private void HdzValidator(int byteNumber)
		{
			using (var writer = new FileStream(Environment.ExpandEnvironmentVariables("%TEMP%") + "\\HDZtest_i.txt", FileMode.Create))
			{
				byte[] arr = Encoding.Default.GetBytes("1234567890");
				for (int i = 0; i < (byteNumber / 10); i++)
					writer.Write(arr, 0, 10);
				writer.Flush();
				var arch = new HdzArchive();
				arch.FileName = writer.Name.Replace("HDZtest_i.txt", "HDZtest_a.hdz");
				arch.AddItem(writer.Name, new HdzHeaderItem("HDZtest_o.txt"));
				arch.Save(HdzArchive.Versions.V1);
				arch = new HdzArchive(arch.FileName);
				arch.ExtractItemsFromHdz(new System.Collections.Generic.List<string> { "HDZtest_o.txt" });
				using (var reader = new FileStream(writer.Name.Replace("HDZtest_i.txt", "HDZtest_o.txt"), FileMode.Open))
				{
					writer.Seek(0, SeekOrigin.Begin);
					var res1 = Checksum(writer);
					var res2 = Checksum(reader);
					Assert.AreEqual(res1, res2);
				}
			}
		}
	}
}
