using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LibGit2Sharp;
using System.IO;

namespace Challenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TypeFileInfoController : ControllerBase
    {
        
        public List<TypeFileInfo> list = new List<TypeFileInfo>();


        [HttpGet]
        public IEnumerable<TypeFileInfo> Get(string url)
        {
            try
            {
               string dirPath = CreateDirectory(url);

                Repository.Clone(url, dirPath);

                GetFileInfo(dirPath);

                return GetCountExtensions().OrderBy(i => i.Extension).ToArray();

            }
            catch (Exception ex)
            {
                throw new Exception("Please replace your 'url' parameter with url=https://github.com/YOUR_GIT_REPOSITORY");
            }
           
        }

        //Create a local directory in your hd
        public string CreateDirectory(string url)
        {
            string[] root = url.Split('/');
            string dirPath = "C:\\" + root.Last();
            if (Directory.Exists(dirPath))
            {
                var random = new Random();
                dirPath += random.Next().ToString();
            }
            else
                Directory.CreateDirectory(dirPath);

            return dirPath;
        }

        //Recursive function to navigate into the folders
        public void GetFileInfo(string dirPath)
        {
            DirectoryInfo[] dirs = new DirectoryInfo(dirPath).GetDirectories();
            FileInfo[] files = new DirectoryInfo(dirPath).GetFiles();
            if (dirs != null)
            {
                foreach (DirectoryInfo d in dirs)
                {
                    GetFileInfo(d.FullName);
                }
            }
            foreach (FileInfo f in files)
            {
                TypeFileInfo info = new TypeFileInfo();
                info.Extension = f.Extension;
                info.Bytes = f.Length;
                list.Add(info);    
            }
        }

        //Returns a new List<TypeFileInfo> with grouped count extensions
        public List<TypeFileInfo> GetCountExtensions()
        {
            List<TypeFileInfo> newList = new List<TypeFileInfo>();

            foreach (TypeFileInfo item in list)
            {
                if (newList.Any(l => l.Extension.Equals(item.Extension)))
                    newList.FirstOrDefault(l => l.Extension.Equals(item.Extension)).Bytes += item.Bytes;
                else
                    newList.Add(item);
                
            }

            return newList;
        }

    }
}
