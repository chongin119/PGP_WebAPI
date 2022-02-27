using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CmdFuncs;
using Zip;
using System;
using System.IO;
using LitJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PGP_WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PGPController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly string OUTputfileextension = ".pgp";

        public PGPController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="companyname">輸入加密用戶</param>
        /// <param name="files">上傳文件</param>
        /// <returns></returns>
        [HttpPost("Encrpyt")]
        public IActionResult En(string companyname,List<IFormFile> files)
        {

            string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Files");
            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            FileInfo[] fileInfo = dir.GetFiles();

            string directoryPath2 = Path.Combine(_webHostEnvironment.ContentRootPath, "Results");
            DirectoryInfo dir2 = new DirectoryInfo(directoryPath2);
            FileInfo[] fileInfo2 = dir2.GetFiles();

            string directoryPath3 = Path.Combine(_webHostEnvironment.ContentRootPath, "ZipResult");
            DirectoryInfo dir3 = new DirectoryInfo(directoryPath3);
            FileInfo[] fileInfo3 = dir3.GetFiles();

            foreach (FileInfo file in fileInfo)
            {
                file.Delete();
            }
            foreach (FileInfo file in fileInfo2)
            {
                file.Delete();
            }
            foreach (FileInfo file in fileInfo3)
            {
                file.Delete();
            }

            if (files.Count == 0)
            {
                return BadRequest("you must upload file!!");
            }

            foreach(var file in files)
            {
                string filePath = Path.Combine(directoryPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            
            Cw cmdd = new Cw("");

            
            

            if (files.Count == 1)
            {   
                fileInfo = dir.GetFiles();
                string filepath = fileInfo[0].ToString();
                string[] filename = fileInfo[0].Name.Split('.');
                string outputfilename = filename[0] + "_" + companyname + "." + filename[1] + OUTputfileextension;
                string outputfilepath = Path.Combine(directoryPath2, outputfilename);

                cmdd.change(companyname, filepath, outputfilepath);
                string result = cmdd.work("e");
                if(result != "success!!" && result.IndexOf("No public key") != -1)
                {
                    return BadRequest("user not exisit!!");
                }
                fileInfo2 = dir2.GetFiles();
                return File(System.IO.File.ReadAllBytes(fileInfo2[0].ToString()), "applicatoin/octet-stream", fileInfo2[0].Name);
            }
            else
            {
                
                ZZZ zzz = new ZZZ();
                zzz.ZipDirectory(directoryPath,directoryPath3);

                fileInfo3 = dir3.GetFiles();
                

                string filepath = fileInfo3[0].ToString();
                string[] filename = fileInfo3[0].Name.Split('.');
                string outputfilename = filename[0] + "_" + companyname + "." + filename[1] + OUTputfileextension;
                string outputfilepath = Path.Combine(directoryPath2, outputfilename);

                cmdd.change(companyname, filepath, outputfilepath);
                string result = cmdd.work("e");
                if (result != "success!!" && result.IndexOf("No public key") != -1)
                {
                    return BadRequest("user not exisit!!");
                }
                fileInfo2 = dir2.GetFiles();
                return File(System.IO.File.ReadAllBytes(fileInfo2[0].ToString()), "applicatoin/octet-stream", fileInfo2[0].Name);
            }
            

            
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="company">輸入用戶</param>
        /// <param name="files">上傳解密文件</param>
        [HttpPost("Decrpyt")]
        public IActionResult De(string company, List<IFormFile> files)
        {

            //read json
            string password = company2password(company);


            string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Files");
            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            FileInfo[] fileInfo = dir.GetFiles();

            string directoryPath2 = Path.Combine(_webHostEnvironment.ContentRootPath, "Results");
            DirectoryInfo dir2 = new DirectoryInfo(directoryPath2);
            FileInfo[] fileInfo2 = dir2.GetFiles();

            string directoryPath3 = Path.Combine(_webHostEnvironment.ContentRootPath, "ZipResult");
            DirectoryInfo dir3 = new DirectoryInfo(directoryPath3);
            FileInfo[] fileInfo3 = dir3.GetFiles();

            foreach (FileInfo file in fileInfo)
            {
                file.Delete();
            }
            foreach (FileInfo file in fileInfo2)
            {
                file.Delete();
            }
            foreach (FileInfo file in fileInfo3)
            {
                file.Delete();
            }

            if (files.Count == 0)
            {
                return BadRequest("you must upload file!!");
            }

            foreach (var file in files)
            {
                string filePath = Path.Combine(directoryPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            Cw cmdd = new Cw(password);

            if (files.Count == 1)
            {
                fileInfo = dir.GetFiles();
                string filepath = fileInfo[0].ToString();
                string[] filename = fileInfo[0].Name.Split('.');
                string outputfilename = filename[0] + "_decrypted" + "." + filename[1];
                string outputfilepath = Path.Combine(directoryPath2, outputfilename);

                cmdd.change("", filepath, outputfilepath);
                string result = cmdd.work("d");
                if(result != "success!!" && result.IndexOf("Bad") != -1)
                {
                    return BadRequest("password wrong!!");
                }
                fileInfo2 = dir2.GetFiles();
                if(fileInfo2.Length == 0)
                {
                    return BadRequest("This file is not a .pgp file!!");
                }
                return File(System.IO.File.ReadAllBytes(fileInfo2[0].ToString()), "applicatoin/octet-stream", fileInfo2[0].Name);
            }
            else
            {
                fileInfo = dir.GetFiles();

                string[] message = new string[files.Count];
                int cnt = 0;
                foreach(FileInfo file in fileInfo)
                {                  
                    string filepath = file.ToString();
                    string[] filename = file.Name.Split('.');
                    string outputfilename = filename[0] + "_decrypted." + filename[1];
                    string outputfilepath = Path.Combine(directoryPath2, outputfilename);

                    cmdd.change("", filepath, outputfilepath);
                    string result = cmdd.work("d");
                    if (result != "success!!" && result.IndexOf("Bad") != -1)
                    {
                        message[cnt++] = "The password of " + outputfilename + " is wrong!!";
                    }else if(result != "success!!" && result.IndexOf("decrypt_message failed") != -1)
                    {
                        message[cnt++] = "The file " + outputfilename + " is not a .pgp file!";
                    }
                }

                if(cnt != 0)
                {       
                    FileStream fs = new FileStream(Path.Combine(directoryPath2,"addtional_Wrong_comment.txt"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs);
                    for (int i = 0; i < cnt; i++)
                    {
                        sw.WriteLine(message[i]);
                    }
                    sw.Close();
                }

                fileInfo2 = dir2.GetFiles();
                if (fileInfo2.Length == 1 && fileInfo2[0].Name == "addtional_Wrong_comment.txt")
                {
                    return File(System.IO.File.ReadAllBytes(fileInfo2[0].ToString()), "application/octet-stream", fileInfo2[0].Name);
                }
                
                ZZZ zzz = new ZZZ();
                zzz.ZipDirectory(directoryPath2, directoryPath3);

                fileInfo3 = dir3.GetFiles();

                return File(System.IO.File.ReadAllBytes(fileInfo3[0].ToString()), "applicatoin/octet-stream", fileInfo3[0].Name);
                
            }

        }

        public static string company2password(string company)
        {
            string jsonfile = "./company2password.json";

            using(StreamReader file = System.IO.File.OpenText(jsonfile))
            {
                using(JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject jobj = (JObject)JToken.ReadFrom(reader);
                    var value = jobj[company];
                    if (value == null)
                    {
                        value = "";
                    }
                    return value.ToString();
                }
            }

        }

    }
}




