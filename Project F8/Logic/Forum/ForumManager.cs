using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Project_F8.Logic.Forum
{
    public class ForumManager
    {

        private struct Folder
        {
            public String name, discription, id;
        }

        public enum FolderType
        {
            Room, 
            RoomFolder,
            SimpleFolder
        };

        private List<Folder> folders = new List<Folder>();
        private Folder folder = new Folder();
        private static ForumManager instance = null;

        public ForumManager() { }

        public static ForumManager getInstance()
        {
            if (instance == null) 
            {
                instance = new ForumManager();
            }
            return instance;
        }

        public void RememberFolders(MatchCollection foldersInfo)
        {
            folders.Clear();
            Int32 iteration = 4;

                while (iteration < foldersInfo.Count)
                {
                    folder.name = foldersInfo[iteration].Value;
                    folder.discription = foldersInfo[iteration + 1].Value;
                    folder.id = foldersInfo[iteration + 2].Value;
                    //Console.WriteLine(folder.name + ' ' + folder.discription + ' ' + folder.id + "\r\n");
                    iteration += 3;
                    folders.Add(folder);
                }
        }

        public List<String> GetNames()
        {
            List<String> retVal = new List<String>();
            foreach (Folder folder in folders)
            {
                retVal.Add(folder.name);
            }

            return retVal;
        }

        public Int32 SearchIDbyName(String name)
        {
            name = name.Substring(0, name.IndexOf("  ("));
            foreach (Folder folder in folders)
            {
                if (folder.name.Equals(name))
                {
                    //Console.WriteLine(folder.name +" " +folder.id);
                    return Int32.Parse(folder.id);
                }
            }
            return 0;
        }

        public String[] GetFolderInfo(String name)
        {
            name = name.Substring(0, name.IndexOf("  ("));
            String[] info = new String[4];
            foreach (Folder folder in folders)
            {
                if (folder.name.Equals(name))
                {
                    info[0] = folder.name;
                    info[1] = folder.id;
                    info[2] = folder.discription;
                    info[3] = ForumWindowHandler.CurrentFolderType.ToString();
                }
            }
            return info;
        }

    }
}
