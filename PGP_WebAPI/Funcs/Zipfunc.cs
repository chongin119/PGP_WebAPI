using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksum;

namespace Zip
{
    /// <summary>
    /// Zip類
    /// </summary>
    public class ZZZ
    {
        
        /// <summary>
        /// Zip類的main方法
        /// </summary>
        /// <param name="DirectoryToZip"></param>
        /// <param name="ZipedPath"></param>
        /// <param name="ZipedFileName"></param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public void ZipDirectory(string DirectoryToZip, string ZipedPath, string ZipedFileName = "")
        {
            if (!System.IO.Directory.Exists(DirectoryToZip))
            {
                throw new System.IO.FileNotFoundException("指定的目录: " + DirectoryToZip + " 不存在!");
            }

            string ZipFileName = string.IsNullOrEmpty(ZipedFileName) ? ZipedPath + "\\" + new DirectoryInfo(DirectoryToZip).Name + ".zip" : ZipedPath + "\\" + ZipedFileName + ".zip";

            using (System.IO.FileStream ZipFile = System.IO.File.Create(ZipFileName))
            {
                using (ZipOutputStream s = new ZipOutputStream(ZipFile))
                {
                    ZipSetp(DirectoryToZip, s, "");
                }
            }
        }

        private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath)
        {
            if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
            {
                strDirectory += Path.DirectorySeparatorChar;
            }
            Crc32 crc = new Crc32();

            string[] filenames = Directory.GetFileSystemEntries(strDirectory);

            foreach (string file in filenames)
            {

                if (Directory.Exists(file))
                {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf("\\") + 1);
                    pPath += "\\";
                    ZipSetp(file, s, pPath);
                }

                else 
                {
                    
                    using (FileStream fs = File.OpenRead(file))
                    {

                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);

                        string fileName = parentPath + file.Substring(file.LastIndexOf("\\") + 1);
                        ZipEntry entry = new ZipEntry(fileName);

                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;

                        fs.Close();

                        crc.Reset();
                        crc.Update(buffer);

                        entry.Crc = crc.Value;
                        s.PutNextEntry(entry);

                        s.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }
    }
}