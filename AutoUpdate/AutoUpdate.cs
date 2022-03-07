using System.Net;
using System.IO.Compression;
using IWshRuntimeLibrary;

const string NAME = "ProceduralShooter";
const string GAME_FILES_LINK = "https://angebrajuka.github.io/gamefiles/"+NAME+".zip";
const string TEMP_ZIP = "./temp.zip";

string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
string install_folder = Path.Combine(roaming, "."+NAME);
string install_exe = Path.Combine(install_folder, NAME+".exe");
string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
string shortcut_path = Path.Combine(desktop, NAME+".lnk");

if(Directory.Exists(install_folder)) Directory.Delete(install_folder, true);

#pragma warning disable SYSLIB0014
using(var client = new WebClient()) {
    await client.DownloadFileTaskAsync(new Uri(GAME_FILES_LINK), TEMP_ZIP);
}

ZipFile.ExtractToDirectory(TEMP_ZIP, roaming);

if(System.IO.File.Exists(TEMP_ZIP)) System.IO.File.Delete(TEMP_ZIP);


object shDesktop = (object)"Desktop";
WshShell shell = new WshShell();
string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\ProceduralShooter.lnk";
IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
shortcut.Description = "pew pew";
shortcut.TargetPath = install_exe;
shortcut.Save();