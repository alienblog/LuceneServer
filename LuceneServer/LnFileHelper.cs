using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneServer
{
	public static class LnFileHelper
	{
		public const string IndexRootDir = "Indexs";

		private static readonly int _maxDirSize;

		static LnFileHelper()
		{
			var maxSize = ConfigurationManager.AppSettings["MaxIndexDirSize"];
			if (!int.TryParse(maxSize, out _maxDirSize))
			{
				_maxDirSize = 1024;
			}
		}

		public static long GetDirSize(DirectoryInfo d)
		{
			long Size = 0;
			// 所有文件大小.
			FileInfo[] fis = d.GetFiles();
			foreach (FileInfo fi in fis)
			{
				Size += fi.Length;
			}
			// 遍历出当前目录的所有文件夹.
			DirectoryInfo[] dis = d.GetDirectories();
			foreach (DirectoryInfo di in dis)
			{
				Size += GetDirSize(di);
			}
			return (Size);
		}

		/// <summary>
		/// 获取最新索引目录
		/// </summary>
		/// <param name="indexName">索引名称</param>
		/// <param name="needCompressionIndexDirPath">需要压缩的目录</param>
		/// <returns></returns>
		public static string GetLastIndexDir(string indexName, out string needCompressionIndexDirPath)
		{
			needCompressionIndexDirPath = string.Empty;
			var rootDir = new DirectoryInfo(Path.Combine(IndexRootDir, indexName));
			if (!rootDir.Exists)
			{
				rootDir.Create();
			}
			var subDirs = rootDir.GetDirectories();
			var index = Math.Max(subDirs.Length - 1, 0);
			var indexDirPath = Path.Combine(IndexRootDir, indexName, index.ToString());
			if (File.Exists(Path.Combine(indexDirPath, "segments.gen")))
			{
				var dirSize = (int)(GetDirSize(new DirectoryInfo(indexDirPath)) / 1024 / 1024);
				if (dirSize >= _maxDirSize)
				{
					needCompressionIndexDirPath = indexDirPath;
					indexDirPath = Path.Combine(IndexRootDir, indexName, (index + 1).ToString());
				}
			}
			return indexDirPath;
		}

		public static bool ExistsIndex(string indexName)
		{
			var rootDir = new DirectoryInfo(Path.Combine(IndexRootDir, indexName));
			if (!rootDir.Exists) return false;

			var subDirs = rootDir.GetDirectories();
			return subDirs.Any(subDir => File.Exists(Path.Combine(subDir.FullName, "segments.gen")));
		}

		public static bool LastDirExistsIndex(string indexName)
		{
			var rootDir = new DirectoryInfo(Path.Combine(IndexRootDir, indexName));
			if (!rootDir.Exists) return false;

			var subDirs = rootDir.GetDirectories();
			var index = Math.Max(subDirs.Length - 1, 0);
			var indexDirPath = Path.Combine(IndexRootDir, indexName, index.ToString());
			if (File.Exists(Path.Combine(indexDirPath, "segments.gen")))
			{
				var dirSize = (int)(GetDirSize(new DirectoryInfo(indexDirPath)) / 1024 / 1024);
				if (dirSize >= _maxDirSize)
				{
					indexDirPath = Path.Combine(IndexRootDir, indexName, (index + 1).ToString());
				}
			}
			
			return subDirs.Any(subDir => File.Exists(Path.Combine(indexDirPath, "segments.gen")));
		}

		public static string[] GetAllIndexDir(string indexName)
		{
			var rootDir = new DirectoryInfo(Path.Combine(IndexRootDir, indexName));

			var subDirs = rootDir.GetDirectories();
			var paths = new List<string>();

			foreach (var subDir in subDirs)
			{
				if (File.Exists(Path.Combine(subDir.FullName, "segments.gen")))
				{
					paths.Add(subDir.FullName);
				}
			}
			return paths.ToArray();
		}
	}
}
