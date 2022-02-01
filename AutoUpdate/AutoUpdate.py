import os
from google_drive_downloader import GoogleDriveDownloader as gdd
import zipfile
import shutil
import winshell
from win32com.client import Dispatch

APP_DATA = os.getenv('APPDATA')+'/'
game_files_directory_path = APP_DATA+'.Grapple/'
game_files_id = '1ImTlPnKqG4jlGJbAcUD2imZgdFqT4kHl'

if(os.path.isdir(game_files_directory_path)):
    shutil.rmtree(game_files_directory_path)
zip_file_path = './temp.zip'
gdd.download_file_from_google_drive(file_id=game_files_id, dest_path=zip_file_path, unzip=False)
with zipfile.ZipFile(zip_file_path, 'r') as zip_ref:
    zip_ref.extractall(APP_DATA)
    zip_ref.close()
os.remove(zip_file_path)

print('updated to newest version')

desktop = winshell.desktop()
path = os.path.join(desktop, "Grapple.lnk")
target = game_files_directory_path+'Grapple.exe'
wDir = game_files_directory_path
icon = target
shell = Dispatch('WScript.Shell')
shortcut = shell.CreateShortCut(path)
shortcut.Targetpath = target
shortcut.WorkingDirectory = wDir
shortcut.IconLocation = icon
shortcut.save()

# compile with pyinstaller:
# pyinstaller -F AutoUpdate.py