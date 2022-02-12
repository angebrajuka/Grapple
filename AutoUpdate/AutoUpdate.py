import os
from google_drive_downloader import GoogleDriveDownloader as gdd
import zipfile
import winshell
from win32com.client import Dispatch
import Paths as p
import Uninstall as u

u.uninstall()
zip_file_path = './temp.zip'
gdd.download_file_from_google_drive(file_id=p.game_files_id, dest_path=zip_file_path, unzip=False)
with zipfile.ZipFile(zip_file_path, 'r') as zip_ref:
    zip_ref.extractall(p.APP_DATA)
    zip_ref.close()
os.remove(zip_file_path)

print('updated to newest version')

desktop = winshell.desktop()
path = os.path.join(desktop, "Grapple.lnk")
target = p.game_files_directory_path+'Grapple.exe'
wDir = p.game_files_directory_path
icon = target
shell = Dispatch('WScript.Shell')
shortcut = shell.CreateShortCut(path)
shortcut.Targetpath = target
shortcut.WorkingDirectory = wDir
shortcut.IconLocation = icon
shortcut.save()

# compile with pyinstaller:
# pyinstaller -F AutoUpdate.py