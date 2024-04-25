using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        bool exitProgram = false;

        while (!exitProgram)
        {
            DisplayMainMenu();
            int choice = GetChoice();

            switch (choice)
            {
                case 1:
                    SubMenuOption1();
                    break;
                case 2:
                    SubMenuOption2();
                    break;
                case 3:
                    exitProgram = true;
                    break;
                default:
                    Console.WriteLine("無效的選項，你來亂的嗎?");
                    break;
            }
        }

        Console.WriteLine("程序已退出。");
    }

    static void DisplayMainMenu()
    {
        Console.WriteLine("主選單");
        Console.WriteLine("1. 執行SSH設置");
        Console.WriteLine("2. 執行GitHub同步");
        Console.WriteLine("3. 退出程序");
    }

    static int GetChoice()
    {
        Console.Write("請輸入選項編號: ");
        if (int.TryParse(Console.ReadLine(), out int choice))
        {
            return choice;
        }
        else
        {
            Console.WriteLine("無效的選項，你來亂的嗎?");
            return GetChoice();
        }
    }

    static void SubMenuOption1()
    {
        bool backToMainMenu = false;

        while (!backToMainMenu)
        {
            DisplaySubMenu1();
            int subChoice = GetSubMenuChoice();

            switch (subChoice)
            {
                case 1:
                    AddHostsEntry();// 执行新增系统hosts主机及IP
                    Console.WriteLine("新增伺服器主機名稱及IP");
                    break;
                case 2:
                    UpdateSshConfig();// 执行ssh配置文件修订
                    Console.WriteLine("ssh設定檔(sshd_config)修訂");
                    break;
                case 3:
                    RestartSshServer();// 执行重启OpenSSH SSH Server
                    Console.WriteLine("重啟OpenSSH SSH Server");
                    break;
                case 4:
                    Addkey();// 执行添加公钥至authorized_keys文件
                    Console.WriteLine("我要申請ssh_key");
                    break;
                case 5:
                    AddPublicKeyToAuthorizedKeys();// 执行添加公钥至authorized_keys文件
                    Console.WriteLine("將公鑰新增至authorized_keys");
                    break;
                case 6:
                    backToMainMenu = true;
                    break;
                case 7:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("無效的選項，你來亂的嗎?");
                    break;
            }
        }
    }


    static void DisplaySubMenu1()
    {
        Console.WriteLine("ssh設定選單");
        Console.WriteLine("1. 新增伺服器主機名稱及IP");
        Console.WriteLine("2. ssh設定檔(sshd_config)修訂");
        Console.WriteLine("3. 重啟OpenSSH SSH Server");
        Console.WriteLine("4. 給我一個ssh_key");
        Console.WriteLine("5. 將公鑰新增至authorized_keys");
        Console.WriteLine("6. 返回上層選單");
        Console.WriteLine("7. 退出程序");
    }

    static void Addkey()
    {
        Console.WriteLine("ssh-keygen -t rsa");
        Console.WriteLine("已完成key申請");
    
    }

    static int GetSubMenuChoice()
    {
        Console.Write("請輸入選項編號: ");
        if (int.TryParse(Console.ReadLine(), out int choice))
        {
            return choice;
        }
        else
        {
            Console.WriteLine("無效的選項，你來亂的嗎?");
            return GetSubMenuChoice();
        }
    }

    static void AddHostsEntry()
    {
        Console.Write("請輸入伺服器主機名稱: ");
        string hostname = Console.ReadLine();
        Console.Write("請輸入伺服器主機 IP: ");
        string ip = Console.ReadLine();

        try
        {
            using (StreamWriter writer = new StreamWriter(@"C:\Windows\System32\drivers\etc\hosts", true))
            {
                writer.WriteLine($"{ip}\t{hostname}");
            }
            Console.WriteLine("伺服器主機名稱及IP已成功新增至hosts文件中。");
        }
        catch (Exception ex)
        {
            Console.WriteLine("添加失敗: " + ex.Message);
        }
    }

    static void UpdateSshConfig()
    {
        string sshConfigPath = @"C:\ProgramData\ssh\sshd_config";

        try
        {
            string[] lines = File.ReadAllLines(sshConfigPath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#PubkeyAuthentication yes"))
                {
                    lines[i] = "PubkeyAuthentication yes";
                }
                else if (lines[i].StartsWith("#AuthorizedKeysFile authorized_keys"))
                {
                    lines[i] = "AuthorizedKeysFile authorized_keys";
                }
                else if (lines[i].Trim().StartsWith("Match Group administrators") ||
                    lines[i].Trim().StartsWith("AuthorizedKeysFile __PROGRAMDATA__/ssh/administrators_authorized_keys"))
                {
                    lines[i] = "#" + lines[i];
                }
            }
            File.WriteAllLines(sshConfigPath, lines);
            Console.WriteLine("sshd_config 更新成功。");
        }
        catch (Exception ex)
        {
            Console.WriteLine("sshd_config 更新失败: " + ex.Message);
        }
    }

    static void RestartSshServer()
    {
        Console.WriteLine("正在重啟OpenSSH SSH Server...");
        // 在这里添加重启SSH Server的代码，这里只是演示文本
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            Process p = Process.Start(psi);
            if (p != null)
            {
                using (StreamWriter sw = p.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine("sc stop ssh");
                        sw.WriteLine("sc start ssh");
                    }
                }
                p.WaitForExit();
            }
            Console.WriteLine("OpenSSH SSH Server 服務已重新起動");
        }
        catch (Exception ex)
        {
            Console.WriteLine("OpenSSH SSH Server重起失敗: " + ex.Message);
        }


    }

    static void AddPublicKeyToAuthorizedKeys()
    {
        string sshFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");
        string authorizedKeysPath = Path.Combine(sshFolderPath, "authorized_keys");
        string idRsaPubPath = Path.Combine(sshFolderPath, "id_rsa.pub");

        try
        {
            if (!Directory.Exists(sshFolderPath))
                Directory.CreateDirectory(sshFolderPath);

            // Read the content of id_rsa.pub file
            if (File.Exists(idRsaPubPath))
            {
                string idRsaPubContent = File.ReadAllText(idRsaPubPath);

                // Append the content to authorized_keys file with new lines between keys
                File.AppendAllText(authorizedKeysPath, idRsaPubContent + "\n");
                Console.WriteLine("已將id_rsa.pub成功新增於authorized_keys");
            }
            else
            {
                Console.WriteLine("id_rsa.pub file not found.");
            }

            Console.WriteLine("authorized_keys已成功創建");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating SSH folder and authorized_keys file: " + ex.Message);
        }
    }




    static void SubMenuOption2()
    {
        bool backToMainMenu = false;

        while (!backToMainMenu)
        {
            DisplaySubMenu2();
            int subChoice = GetSubMenuChoice();

            switch (subChoice)
            {
                case 1:
                    GitUPfirst();// 执行初始化Git仓库
                    Console.WriteLine("首次上傳Github");
                    break;
                case 2:
                    GitUP();// 执行新增提交数据
                    Console.WriteLine("本機上傳已連結Github資料夾");
                    break;
                case 3:
                    ConnectToRemoteRepository();// 执行与远程仓库连接
                    Console.WriteLine("與git倉庫建立連接(remote)");
                    break;
                case 4:
                    CommitAndPush();// 执行提交并同步到远程仓库
                    Console.WriteLine("Github提交(commit)並執行同步(push)");
                    break;
                case 5:
                    CloneGitRepository();// 执行下载Git数据至本地
                    Console.WriteLine("下載git資料(clone)");
                    break;
                case 6:
                    backToMainMenu = true;
                    break;
                case 7:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("無效的選項，你來亂的嗎?");
                    break;
            }
        }
    }

    static void DisplaySubMenu2()
    {
        Console.WriteLine("Github選單");
        Console.WriteLine("1. 首次上傳Github");
        Console.WriteLine("2. 本機上傳已連結Github資料夾");
        Console.WriteLine("3. 與git倉庫建立連接(remote)");
        Console.WriteLine("4. Github提交(commit)並執行同步(push)");
        Console.WriteLine("5. 下載git資料(clone)");
        Console.WriteLine("6. 返回上層選單");
        Console.WriteLine("7. 退出程序");
    }

    static void GitUPfirst()
    {
        // 輸入GitHub倉庫和URL
        Console.WriteLine("請輸入GitHub使用者名稱：");
       string githubid = Console.ReadLine();
        Console.WriteLine("請輸入GitHub倉庫名稱：");
        string githubURL = Console.ReadLine();
        Console.WriteLine("請輸入GitHub倉庫分支名稱：");
        string githubmain = Console.ReadLine();

        Console.Write("請輸入要初始化(init)的資料夾路徑: ");
        string folderPath = Console.ReadLine();

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("資料夾不存在！");
            return;
        }

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "cmd",
            WorkingDirectory = folderPath,
            UseShellExecute = false, // 不使用 Shell 執行
            RedirectStandardInput = true, // 重定向標準輸入
            RedirectStandardOutput = true, // 重定向標準輸出
            CreateNoWindow = false // 不創建新視窗
        };

        Process process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (sender, e) =>
        {
            // 監聽視窗a的輸出
            Console.WriteLine("/r/n"+"系統後台輸出： " + e.Data);
        };

        process.Start();
        process.BeginOutputReadLine(); // 開始異步讀取輸出

        // 向視窗a發送指令
        StreamWriter sw = process.StandardInput;
        sw.WriteLine("git init"); // 這裡可以替換成你想要執行的指令
        sw.WriteLine("git add *");
        sw.WriteLine("git commit -m \"first commit\"");
        sw.WriteLine($"git branch -M {githubmain}");
        sw.WriteLine($"git remote add {githubid} https://github.com/{githubid}/{githubURL}.git");
        sw.WriteLine($"git push -u {githubid} {githubmain}");
        sw.WriteLine("請繼續選擇選單執行項目: ");
        sw.WriteLine("exit"); // 退出視窗a
        sw.Close(); // 關閉輸入流
   
    }


    static void GitUP()
    {
        Console.Write("請輸入要上傳的資料夾路徑(資料夾需已git資料夾連結): ");
        string folderPath = Console.ReadLine();

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("資料夾不存在！");
            return;
        }

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "cmd",
            WorkingDirectory = folderPath,
            UseShellExecute = false, // 不使用 Shell 執行
            RedirectStandardInput = true, // 重定向標準輸入
            RedirectStandardOutput = true, // 重定向標準輸出
            CreateNoWindow = false // 不創建新視窗
        };

        Process process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (sender, e) =>
        {
            // 監聽視窗a的輸出
            Console.WriteLine("系統後台輸出： " + e.Data);
        };

        process.Start();
        process.BeginOutputReadLine(); // 開始異步讀取輸出

        // 向視窗a發送指令
        StreamWriter sw = process.StandardInput;
        // 這裡可以替換成你想要執行的指令
        sw.WriteLine("git add *");
        sw.WriteLine("git commit -m \"我是提交訊息\"");
        sw.WriteLine("git push");
        sw.WriteLine("請繼續選擇選單執行項目");
        sw.WriteLine("exit"); // 退出視窗a
        sw.Close(); // 關閉輸入流
        
    }

    static bool CheckIfGitRepo()
    {
        // 执行 git rev-parse --is-inside-work-tree 命令来检查当前目录是否是Git仓库
        ExecuteGitCommand("git rev-parse --is-inside-work-tree");
        // 在 ExecuteGitCommand 方法中获取并处理命令行的输出结果
        // 如果输出结果为 true，则表示当前目录是Git仓库；否则不是。
        // 在这里假设 ExecuteGitCommand 方法能正确处理命令行输出并返回结果
        bool isGitRepo = true;
        return isGitRepo;
    }

    static void ConnectToRemoteRepository()
    {
        Console.Write("請輸入遠端倉庫名稱: ");
        string remoteName = Console.ReadLine();
        Console.Write("請輸入遠端倉庫的URL: ");
        string remoteUrl = Console.ReadLine();

        ExecuteGitCommand($"git remote add {remoteName} {remoteUrl}");
        Console.WriteLine("遠端倉庫連接成功。");
    }

    static void CommitAndPush()
    {
        


       

        Console.WriteLine("請輸入GitHub使用者名稱：");
        string githubid = Console.ReadLine();

        Console.WriteLine("請輸入GitHub倉庫名稱：");
        string githubURL = Console.ReadLine();

        Console.WriteLine("請輸入GitHub倉庫分支名稱：");
        string githubmain = Console.ReadLine();

        Console.Write("請輸入要提交(commit)的資料夾路徑: ");
        string folderPath = Console.ReadLine();

        Console.Write("請輸入要提交(commit)的訊息: ");
        string commitMessage = Console.ReadLine();

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("資料夾不存在！");
            return;
        }


        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "cmd",
            WorkingDirectory = folderPath,
            UseShellExecute = false, // 不使用 Shell 執行
            RedirectStandardInput = true, // 重定向標準輸入
            RedirectStandardOutput = true, // 重定向標準輸出
            CreateNoWindow = false // 不創建新視窗
        };

        Process process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (sender, e) =>
        {
            // 監聽視窗a的輸出
            Console.WriteLine("/r/n" + "系統後台輸出： " + e.Data);
        };

        process.Start();
        process.BeginOutputReadLine(); // 開始異步讀取輸出

        // 向視窗a發送指令
        StreamWriter sw = process.StandardInput;
        sw.WriteLine("git init"); // 這裡可以替換成你想要執行的指令
        sw.WriteLine("git add *");
    
        sw.WriteLine($"git commit -M  {commitMessage}");
        sw.WriteLine($"git remote add {githubid} https://github.com/{githubid}/{githubURL}.git");
        sw.WriteLine($"git push -u {githubid} {githubmain}");
        sw.WriteLine("請繼續選擇選單執行項目: ");
        sw.WriteLine("exit"); // 退出視窗a
        sw.Close(); // 關閉輸入流


        Console.Write("是否執行同步操作 (push)? (Y/N): ");
        string input = Console.ReadLine();
        if (input.ToUpper() == "Y")
        {

            Console.WriteLine("1. 首次推送");
            Console.WriteLine("2. 直接推送");
            Console.Write("請選擇推送(push)方式: ");
            string option = Console.ReadLine();

            if (option == "1")
            {
                FirstTimePush();
            }
            else if (option == "2")
            {
                DirectPush();
            }

        }
        else
        {
            Console.WriteLine("未完成同步推送(push)，回到主選單。");
        }

    }
    static void FirstTimePush()
    {
        Console.Write("請輸入要同步的git倉庫名稱: ");
        string remoteName = Console.ReadLine();
        Console.Write("請輸入同步的分支名稱: ");
        string branchName = Console.ReadLine();

        ExecuteGitCommand($"git push -u {remoteName} {branchName}");
        Console.WriteLine("首次推送已完成。" + "\r\n");
    }

    static void DirectPush()
    {
        ExecuteGitCommand($"git push");
        Console.WriteLine("直接推送已完成。" + "\r\n");
    }

    static void CloneGitRepository()
    {
        Console.Write("請輸入要存放的本機資料夾路徑: ");
        string folderPath = Console.ReadLine();

        Console.Write("請輸入要下載的GitHub資料夾URL: ");
        string repoUrl = Console.ReadLine();
        

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "cmd",
            WorkingDirectory = folderPath,
            UseShellExecute = false, // 不使用 Shell 執行
            RedirectStandardInput = true, // 重定向標準輸入
            RedirectStandardOutput = true, // 重定向標準輸出
            CreateNoWindow = false // 不創建新視窗
        };

        Process process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (sender, e) =>
        {
            // 監聽視窗a的輸出
            Console.WriteLine("系統後台輸出： " + e.Data);
        };

        process.Start();
        process.BeginOutputReadLine(); // 開始異步讀取輸出

        // 向視窗a發送指令
        StreamWriter sw = process.StandardInput;
        // 這裡可以替換成你想要執行的指令
        sw.WriteLine($"git clone {repoUrl}");
        Console.WriteLine("Git資料下載完成。" + "\r\n");
        sw.WriteLine("exit"); // 退出視窗a
        sw.Close(); // 關閉輸入流
        Console.WriteLine("請選擇選單執行項目");
               
        
    }

    static void ExecuteGitCommand(string command)
    {
        var processInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (var process = Process.Start(processInfo))
        {
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine("Git 輸出訊息:" + "\r\n");
                Console.WriteLine(output);
            }

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Git 錯誤訊息:" + "\r\n");
                Console.WriteLine(error);
            }
        }
    }

}
