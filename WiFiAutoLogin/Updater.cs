using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiFiAutoLogin
{
    static class Updater
    {
        private static string UPDATE_PATH = "https://giovannivella.com/softwares/" + Application.ProductName;

        public static bool checkUpdates(NotifyIcon notifyIcon)
        {
            try
            {
                System.IO.File.WriteAllText("temp.config", webRequest(UPDATE_PATH + "/info"));
                string webVer = getSetting("version", "temp.config");
                if (webVer != Application.ProductVersion)
                {
                    string localFilePath = System.IO.Path.GetTempPath() + @"\" + Application.ProductName;
                    if (!System.IO.Directory.Exists(localFilePath))
                        System.IO.Directory.CreateDirectory(localFilePath);
                    notifyIcon.ShowBalloonTip(2000, "Downloading update", "Updating to version " + webVer, ToolTipIcon.Info);
                    webDownloadFile(UPDATE_PATH + "/setup.exe", localFilePath + @"\setup.exe");
                    System.IO.File.Delete("temp.config");
                    System.Diagnostics.Process.Start(localFilePath + @"\setup.exe");
                    Environment.Exit(309);
                    return true;
                }
                else
                    return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        private static string getSetting(string name, string filename = "settings.config")
        {
            string[] fileline = null;
            try
            {
                fileline = System.IO.File.ReadAllLines(filename);
                for (int i = 0; i < fileline.Length; i++)
                {
                    if (fileline[i].Contains(name + ": "))
                    {
                        return fileline[i].Replace(name + ": ", "");
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                System.IO.File.WriteAllText(filename, "");
            }
            catch
            {

            }
            return String.Empty;
        }
        private static void webDownloadFile(string webPath, string localPath)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile(webPath, localPath);
        }
        private static string webRequest(string url)
        {
            WebClient request = new WebClient();
            byte[] newFileData = request.DownloadData(url);
            return System.Text.Encoding.Default.GetString(newFileData);
        }
    }
}
