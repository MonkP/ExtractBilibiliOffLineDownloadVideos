using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net;

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
            configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");
            //尝试加载配置文件内容
            if (File.Exists(configPath))
            {
                var configText = File.ReadAllText(configPath);
                try
                {
                    var configObj =JsonNode.Parse(configText);
                    txt_OutputDir.Text = configObj["dest"].GetValue<string>();
                    txt_SourceDir.Text = configObj["source"].GetValue<string>();
                    txt_ffmpegPath.Text = configObj["ffmpeg"].GetValue<string>();
                }
                catch (Exception ex)
                {
                    var log = $"读取 config.json 异常，文件内容: {configText}， 异常信息：{ex.Message}";
                }
                if (!string.IsNullOrEmpty(configText))
                {
                    if (!Directory.Exists(txt_OutputDir.Text))
                    {
                        logout("config.json中的输出目录无效。");
                    }
                    if (!Directory.Exists(txt_SourceDir.Text))
                    {
                        logout("config.json中的源目录无效。");
                    }
                    if (!File.Exists(txt_ffmpegPath.Text))
                    {
                        logout("config.json中的ffmpeg路径无效。");
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
                    dest = txt_OutputDir.Text,
                    ffmpeg = txt_ffmpegPath.Text
                };
                var text = JsonSerializer.Serialize(obj);
                return text;
            }
            else
            {
                return string.Empty;
            }
        }

        private void logout(string text)
        {
            outputBuilder.AppendLine(text);
            txt_Output.Text = outputBuilder.ToString();
            txt_Output.SelectionStart = txt_Output.Text.Length;
            txt_Output.ScrollToCaret();
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
            //按title分组记录各entry中的视频信息
            Dictionary<string, List<VideoEntry>> videoEntrysByTitle = new Dictionary<string, List<VideoEntry>>();
            FileInfo[] files = sourceDir.GetFiles("entry.json", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var videoEntry = ScanVideoFromEntryFile(files[i]);
                if (videoEntry != null)
                {
                    List<VideoEntry> list = videoEntrysByTitle.ContainsKey(videoEntry.title)? videoEntrysByTitle[videoEntry.title]: new List<VideoEntry> ();
                    list.Add(videoEntry);
                    videoEntrysByTitle[videoEntry.title] = list;
                }
            }
            List<VideoEntry> toScanVideos = new List<VideoEntry>();
            foreach(var kvp in videoEntrysByTitle)
            {
                var list = kvp.Value;
                var title = kvp.Key;
                foreach(var entry in list)
                {
                    entry.outDirPath = Path.Combine(outputDirPath, entry.title);
                    entry.outPath = Path.Combine(entry.outDirPath, $"{entry.partTitle}.mp4");
                    toScanVideos.Add(entry);
                }
            }

            MoveVideoFiles(toScanVideos);
        }

        VideoEntry ScanVideoFromEntryFile(FileInfo entryFileInfo)
        {
            logout("entryFile = " + entryFileInfo.FullName);
            DirectoryInfo entryFileDir = entryFileInfo.Directory;

            JsonNode json = JsonNode.Parse(File.ReadAllText(entryFileInfo.FullName));
            if (string.IsNullOrEmpty(json["title"].GetValue<string>()))
            {
                return null;
            }
            string title = GetWindowsCanUseName(json["title"].GetValue<string>());
            string typeTag = json["type_tag"].GetValue<string>();
            var partNode = json["page_data"];
            var partTitle = GetWindowsCanUseName(partNode["part"].GetValue<string>());
            var toReturn = new VideoEntry
            {
                title = title,
                partTitle = partTitle,
                coverUrl = json["cover"].GetValue<string>(),
            };
            var contentDirPath = Path.Combine(entryFileDir.FullName, typeTag);
            if(Directory.Exists(contentDirPath))
            {
                var videoPath = Path.Combine(contentDirPath, "video.m4s");
                var audioPath = Path.Combine(contentDirPath, "audio.m4s");
                if(File.Exists(videoPath) && File.Exists(audioPath))
                {
                    toReturn.contentPath = contentDirPath;
                    return toReturn;
                }
            }
            return null;
        }
        //逐个entry处理
        private void MoveVideoFiles(List<VideoEntry> videoEntryList)
        {
            foreach (var videoEntry in videoEntryList)
            {
                if (!Directory.Exists(videoEntry.outDirPath))
                {
                    Directory.CreateDirectory(videoEntry.outDirPath);
                }
                var videoDir = new DirectoryInfo(videoEntry.contentPath);
                //启动命令行

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = txt_ffmpegPath.Text;
                p.StartInfo.Arguments = "-i video.m4s -i audio.m4s -vcodec copy -acodec copy new.mp4";
                p.StartInfo.WorkingDirectory = videoEntry.contentPath;
                //p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;            //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;       //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = false;     //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = false;      //重定向标准错误输出
                p.StartInfo.CreateNoWindow = false;              //不显示程序窗口
                p.Start();

                //切换工作目录

                //p.StandardInput.WriteLine(videoDir.Root.ToString()[0] + ":");
                //p.StandardInput.WriteLine("cd " + videoEntry.contentPath);

                ////合成video

                //p.StandardInput.WriteLine("ffmpeg -i video.m4s -i audio.m4s -vcodec copy -acodec copy new.mp4");       //防止文件名无效


                p.StandardInput.AutoFlush = true;

                p.WaitForExit();//等待程序执行完退出进程
                p.Close();

                //生成的文件移动到新位置
                var newFilePath = Path.Combine(videoEntry.contentPath, "new.mp4");
                //下载cover
                if (!string.IsNullOrEmpty(videoEntry.coverUrl))
                {
                    var coverExt = Path.GetExtension(videoEntry.coverUrl);
                    var coverPath = Path.Combine(videoEntry.outDirPath, "cover" + coverExt);
                    using (var webClient = new WebClient())
                    {
                        try
                        {
                            webClient.DownloadFile(videoEntry.coverUrl, coverPath);
                        }
                        catch
                        {
                            logout($"下载封面失败：" + coverPath);
                        }
                    }
                }
                if (File.Exists(newFilePath))
                {
                    File.Move(newFilePath, videoEntry.outPath);
                    logout($"已合并视频：{videoEntry.outPath}");
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
            p.StartInfo.CreateNoWindow = false;              //不显示程序窗口
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

        private void btn_ChooseFfmpeg_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (File.Exists(txt_ffmpegPath.Text))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(txt_ffmpegPath.Text);
            }
            dlg.Title = "选择ffmpeg.exe";
            dlg.Filter = "ffmpeg.exe|ffmpeg.exe";
            dlg.Multiselect = false;
            dlg.CheckFileExists = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txt_ffmpegPath.Text = dlg.FileName;
            }
        }

        private void btn_Exec_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txt_OutputDir.Text))
            {
                logout("输出目录无效。");
                return;
            }
            if (!Directory.Exists(txt_SourceDir.Text))
            {
                logout("源目录无效。");
                return;
            }
            if (!File.Exists(txt_ffmpegPath.Text) || Path.GetExtension(txt_ffmpegPath.Text).ToLower() != ".exe")
            {
                logout("未正确设置设置ffmpeg路径。");
                return;
            }
            writeConfigJson();
            ScanTheDirectory(txt_SourceDir.Text, txt_OutputDir.Text);
            logout("处理完成。");
        }
    }

    internal class VideoEntry
	{
        internal string contentPath;
        internal string title, partTitle, coverUrl;
        /// <summary>
        /// 文件的最终输出路径
        /// </summary>
        internal string outPath;

        /// <summary>
        /// 文件的输出路径所在文件夹
        /// </summary>
        internal string outDirPath;
	}
}
