using Smart_Hold.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Hold.IO
{
    public static class Explorer
    {
        public static int TotalFiles(DirectoryInfo folder) => Directory.GetFiles(folder.FullName, "*", SearchOption.AllDirectories).Count();

        public static void Clone(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
                return;

            if (!Directory.Exists(target.FullName))
                Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fi in source.GetFiles())
            {
                SmartHold.CurrentFolderBackup.ProgressValue++;
                SmartHoldViewModel.Instance.CurrentFile = fi.FullName;
                try
                {
                    bool needSync = false;
                    string newPath = Path.Combine(target.ToString(), fi.Name);

                    if (!File.Exists(newPath))
                    {
                        fi.CopyTo(newPath);
                        needSync = true;
                    }
                    else
                    {
                        if (File.GetLastWriteTime(fi.FullName) > File.GetLastWriteTime(newPath))
                        {
                            fi.CopyTo(newPath, true);
                            needSync = true;
                        }
                    }

                    if (needSync) SyncMetadata(fi, new DirectoryInfo(newPath));
                }
                catch (Exception ex) { Logger.Logger.LogError(ex, fi); }
            }

            foreach (DirectoryInfo di in source.GetDirectories())
            {
                try
                {
                    string newPath = Path.Combine(target.ToString(), di.Name);

                    if (!Directory.Exists(newPath))
                        SyncMetadata(di, target.CreateSubdirectory(di.Name));

                    DirectoryInfo nextTarget = new DirectoryInfo(newPath);
                    Clone(di, nextTarget);
                }
                catch (Exception ex) { Logger.Logger.LogError(ex, di); }
            }

            SmartHoldViewModel.Instance.CurrentFile = "";
        }

        public static void CleanTarget(DirectoryInfo source, DirectoryInfo target)
        {

            if (source.FullName.ToLower() == target.FullName.ToLower())
                return;

            foreach (FileInfo fi in target.GetFiles())
            {
                try
                {
                    string path = Path.Combine(source.ToString(), fi.Name);
                    if (!File.Exists(path))
                    {
                        SmartHoldViewModel.Instance.CurrentFile = "Suppression de : " + fi.FullName;
                        Debug.WriteLine($"Deleting de {fi.FullName}...");
                        fi.Delete();
                        Debug.WriteLine("Done..");
                    }
                }
                catch(Exception ex) { Logger.Logger.LogError(ex, fi); }
            }

            foreach (DirectoryInfo di in target.GetDirectories())
            {
                try
                {
                    string path = Path.Combine(source.ToString(), di.Name);
                    if (!Directory.Exists(path))
                        di.Delete(true);
                    else
                        CleanTarget(new DirectoryInfo(path), di);
                }
                catch (Exception ex) { Logger.Logger.LogError(ex, di); }
            }
        }

        public static void SyncMetadata(FileSystemInfo source, FileSystemInfo target)
        {
            target.CreationTime = source.CreationTime;
            target.LastWriteTime = source.LastWriteTime;
            target.LastAccessTime = source.LastAccessTime;
        }
    }
}
