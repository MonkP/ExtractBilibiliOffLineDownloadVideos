using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ExtractBilibiliOffLineDownloadVideos
{
    public partial class Form1 : Form
    {
        StringBuilder outputBuilder;
        string configPath;

        public Form1()
        {
            InitializeComponent();
            outputBuilder = new StringBuilder();
            //固定的配置文件路径
            configPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "config.json");
            //尝试加载配置文件内容
            if (File.Exists(configPath))
            {
                var configText = File.ReadAllText(configPath);
                try
                {
                    var configObj =JsonNode.Parse(configText);
                    txt_OutputDir.Text = configObj["dest"].ToJsonString();
                    txt_SourceDir.Text = configObj["source"].ToJsonString();
                }
                catch (Exception ex)
                {
                    var log = $"读取 config.json 异常，文件内容: {configText}， 异常信息：{ex.Message}";
                }
                if (!string.IsNullOrEmpty(configText))
                {
                    if (!Directory.Exists(txt_OutputDir.Text))
                    {
                        writeOutputlog("config.json中的输出目录无效。");
                    }
                    if (!Directory.Exists(txt_SourceDir.Text))
                    {
                        writeOutputlog("config.json中的源目录无效。");
                    }
                }
            }
        }


        /// <summary>
        /// 根据当前输入框的值，生成配置文件内容
        /// </summary>
        /// <returns></returns>
        private string buildConfigJson()
        {
            if(Directory.Exists(txt_OutputDir.Text) && Directory.Exists(txt_SourceDir.Text))
            {
                var obj = new
                {
                    source = txt_SourceDir.Text,
                    dest = txt_OutputDir.Text
                };
                var text = JsonSerializer.Serialize(obj);
                return text;
            }
            else
            {
                return string.Empty;
            }
        }

        private void writeOutputlog(string text)
        {
            outputBuilder.AppendLine(text);
            txt_Output.Text = outputBuilder.ToString();
        }

        /// <summary>
        /// 如当前目录配置有效，则将目录配置写到配置文件
        /// </summary>
        private void writeConfigJson()
        {
            var text = buildConfigJson();
            if (!string.IsNullOrEmpty(text))
            {
                File.WriteAllText(configPath, text);
            }
        }

        public void ScanTheDirectory(string sourceDirPath, string outputDirPath)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(sourceDirPath);
            if (!sourceDir.Exists)
            {
                return;
            }

            Dictionary<int, List<VideoEntry>> videoEntryByAvidList = new Dictionary<int, List<VideoEntry>>();
            FileInfo[] files = sourceDir.GetFiles("entry.json", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                List<VideoEntry> videoEntrys = ScanVideoFromEntryFile(files[i]);
                if (videoEntrys != null && videoEntrys.Count > 0)
                {
                    if (!videoEntryByAvidList.ContainsKey(videoEntrys[0].avid))
                    {
                        videoEntryByAvidList.Add(videoEntrys[0].avid, new List<VideoEntry>());
                    }
                    videoEntryByAvidList[videoEntrys[0].avid].AddRange(videoEntrys);
                }
            }

            MoveVideoFiles(videoEntryByAvidList, outputDirPath);
        }

        List<VideoEntry> ScanVideoFromEntryFile(FileInfo entryFileInfo)
        {
            Console.WriteLine("entryFile = " + entryFileInfo.FullName);
            DirectoryInfo entryFileDir = entryFileInfo.Directory;
            FileInfo[] files = entryFileDir.GetFiles("*", SearchOption.AllDirectories);
            List<FileInfo> videoFiles = new List<FileInfo>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".blv") || files[i].Name.EndsWith(".flv") || files[i].Name.EndsWith(".mp4"))
                {
                    videoFiles.Add(files[i]);
                }
            }

            JsonNode json = MiniJSON.jsonDecode(File.ReadAllText(entryFileInfo.FullName)) as Hashtable;
            if (string.IsNullOrEmpty(json["title"].ToString()))
            {
                return null;
            }
            int avid = (int)((double)json["avid"]);
            string title = GetWindowsCanUseName(json["title"].ToString());
            string part = GetWindowsCanUseName((json["page_data"] as Hashtable)["part"].ToString());
            List<VideoEntry> videoEntrys = new List<VideoEntry>();
            for (int i = 0; i < videoFiles.Count; i++)
            {
                //Console.WriteLine("videoFileAddresss = " + videoFiles[i].FullName);
                VideoEntry videoEntry = new VideoEntry(avid);
                videoEntry.fileInfo = videoFiles[i];
                videoEntry.title = title;
                videoEntry.part = part;
                videoEntrys.Add(videoEntry);
            }
            return videoEntrys;
        }

        private void MoveVideoFiles(Dictionary<int, List<VideoEntry>> videoEntryByAvidList, string outPutDir)
        {
            List<int> videoIdList = new List<int>(videoEntryByAvidList.Keys);
            for (int i = 0; i < videoIdList.Count; i++)
            {
                List<VideoEntry> videoEntrys = new List<VideoEntry>(videoEntryByAvidList[videoIdList[i]]);
                if (videoEntrys.Count == 1)
                {
                    string newPath = outPutDir + Path.DirectorySeparatorChar + videoEntrys[0].title + videoEntrys[0].fileInfo.Extension;
                    File.Move(videoEntrys[0].fileInfo.FullName, newPath);
                }
                else if (videoEntrys.Count > 1)
                {
                    string newDirectoryPath = outPutDir + Path.DirectorySeparatorChar + videoEntrys[0].title;
                    Directory.CreateDirectory(newDirectoryPath);
                    Dictionary<string, List<VideoEntry>> videoEntryByPartList = new Dictionary<string, List<VideoEntry>>();
                    for (int j = 0; j < videoEntrys.Count; j++)
                    {
                        if (!videoEntryByPartList.ContainsKey(videoEntrys[j].part))
                        {
                            videoEntryByPartList.Add(videoEntrys[j].part, new List<VideoEntry>());
                        }
                        videoEntryByPartList[videoEntrys[j].part].Add(videoEntrys[j]);
                    }
                    List<string> partList = new List<string>(videoEntryByPartList.Keys);
                    if (partList.Count == 1)
                    {
                        for (int j = 0; j < videoEntrys.Count; j++)
                        {
                            string newFilePart1 = newDirectoryPath + Path.DirectorySeparatorChar + videoEntrys[j].title;
                            File.Move(videoEntrys[j].fileInfo.FullName, newFilePart1 + videoEntrys[j].fileInfo.Name);
                        }
                        //生成自动批处理合成配置文件 最后并执行
                        GenertMergeVideoPlayerList(newDirectoryPath);
                    }
                    else if (partList.Count > 1)
                    {
                        for (int j = 0; j < partList.Count; j++)
                        {
                            newDirectoryPath = outPutDir + Path.DirectorySeparatorChar + videoEntrys[0].title;
                            if (videoEntryByPartList[partList[j]].Count == 1)
                            {
                                string newFilePart1 = newDirectoryPath + Path.DirectorySeparatorChar + videoEntrys[j].part;
                                File.Move(videoEntrys[j].fileInfo.FullName, newFilePart1 + videoEntrys[j].fileInfo.Extension);
                            }
                            else if (videoEntryByPartList[partList[j]].Count > 1)
                            {
                                newDirectoryPath = outPutDir + Path.DirectorySeparatorChar + videoEntrys[0].title
                                    + Path.DirectorySeparatorChar + videoEntrys[j].part;
                                for (int k = 0; k < videoEntryByPartList[partList[j]].Count; k++)
                                {
                                    string newFilePart1 = newDirectoryPath + Path.DirectorySeparatorChar + videoEntryByPartList[partList[j]][k].title;
                                    File.Move(videoEntryByPartList[partList[j]][k].fileInfo.FullName, newFilePart1 + videoEntryByPartList[partList[j]][k].fileInfo.Name);
                                }
                                //生成自动批处理合成配置文件 最后并执行
                                GenertMergeVideoPlayerList(newDirectoryPath);
                            }
                        }
                    }
                }
            }
        }

        string GetWindowsCanUseName(string orgName)
        {
            return orgName.Replace("<", "_").Replace(">", "_").Replace("/", "_").Replace('\\', '_').Replace(":", "_")
                .Replace("|", " ").Replace("*", "_").Replace("?", "_").Replace("\"", "_");
        }

        bool GenertMergeVideoPlayerList(string dirPath)
        {
            DirectoryInfo videosDir = new DirectoryInfo(dirPath);
            if (!videosDir.Exists)
            {
                return false;
            }

            string playerListStr = "";
            FileInfo[] files = videosDir.GetFiles("*");
            for (int i = 0; i < files.Length; i++)
            {
                playerListStr += "file '" + files[i].FullName + ((i < files.Length - 1) ? "'\n" : "");
            }
            string playerlistFilePath = dirPath + Path.DirectorySeparatorChar + "playlist";
            File.WriteAllText(playerlistFilePath, playerListStr);


            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;            //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;       //接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = false;     //由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = false;      //重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;              //不显示程序窗口
            p.Start();                                      //启动程序

            //此处需要安装ffmpeg https://www.ffmpeg.org/download.html
            p.StandardInput.WriteLine(videosDir.Root.ToString()[0] + ":");
            p.StandardInput.WriteLine("cd " + videosDir);
            p.StandardInput.WriteLine("ffmpeg -f concat -safe 0 -i playlist -c copy outVideo.mp4 &exit");       //防止文件名无效
            p.StandardInput.AutoFlush = true;

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            Console.WriteLine("marge finish");

            File.Move(dirPath + Path.DirectorySeparatorChar + "outVideo.mp4", dirPath + ".mp4");
            if (File.Exists(dirPath + ".mp4"))
            {
                videosDir.Delete(true);
            }

            return true;
        }

        /// <summary>
        /// 选择来源路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ChooseSourceDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "选择源路径";
            if (Directory.Exists(txt_SourceDir.Text))
            {
                dlg.SelectedPath = txt_SourceDir.Text;
            }
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                txt_SourceDir.Text = dlg.SelectedPath;
            }
        }
        /// <summary>
        /// 选择输出路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ChooseOutputDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "选择输出路径";
            if (Directory.Exists(txt_OutputDir.Text))
            {
                dlg.SelectedPath = txt_OutputDir.Text;
            }
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txt_OutputDir.Text = dlg.SelectedPath;
            }
        }

        private void btn_Exec_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txt_OutputDir.Text))
            {
                writeOutputlog("输出目录无效。");
                return;
            }
            if (!Directory.Exists(txt_SourceDir.Text))
            {
                writeOutputlog("源目录无效。");
                return;
            }
            writeConfigJson();
            ScanTheDirectory(txt_SourceDir.Text, txt_OutputDir.Text);
        }
    }

    internal class VideoEntry
	{
		internal int avid;
		internal FileInfo videoFileInfo;
		internal string title, part;
        /// <summary>
        /// 文件的最终输出路径
        /// </summary>
        internal string outPath;

        /// <summary>
        /// 此文件的输出路径
        /// </summary>
        internal string outDirPath;



		internal VideoEntry(int avid)
		{
			this.avid = avid;
		}
	}
}
