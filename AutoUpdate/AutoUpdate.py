import os
import sys
from google_drive_downloader import GoogleDriveDownloader as gdd
import zipfile
import shutil
import winshell
from win32com.client import Dispatch

APP_DATA = os.getenv('APPDATA')+'/'
game_files_directory_path = APP_DATA+'.Grapple/'
sys_version_file_path = game_files_directory_path+'version.v'
cld_version_file_id = '1fqSnlFYpbm-3NDSg21p3JmCdIMl4HuCP'
cld_game_files_id   = '1ImTlPnKqG4jlGJbAcUD2imZgdFqT4kHl'

cld_version_file_path = './temp.v'
gdd.download_file_from_google_drive(file_id=cld_version_file_id, dest_path=cld_version_file_path, unzip=False)
cld_version_file = open(cld_version_file_path, 'rb')
cld_version_major = int.from_bytes(cld_version_file.read(4), byteorder='big')
cld_version_minor = int.from_bytes(cld_version_file.read(4), byteorder='big')
cld_version_patch = int.from_bytes(cld_version_file.read(4), byteorder='big')
cld_version_file.close()
os.remove(cld_version_file_path)

if(os.path.isfile(sys_version_file_path)):
    sys_version_file = open(sys_version_file_path, 'rb')
    sys_version_major = int.from_bytes(sys_version_file.read(4), byteorder='big')
    sys_version_minor = int.from_bytes(sys_version_file.read(4), byteorder='big')
    sys_version_patch = int.from_bytes(sys_version_file.read(4), byteorder='big')
    sys_version_file.close()

    if(cld_version_major == sys_version_major and cld_version_minor == cld_version_minor and sys_version_patch == cld_version_patch):
        print('current version ({0}.{1}.{2}) already installed'.format(sys_version_major, sys_version_minor, sys_version_patch))
        sys.exit()

if(os.path.isdir(game_files_directory_path)):
    shutil.rmtree(game_files_directory_path)
zip_file_path = './temp.zip'
gdd.download_file_from_google_drive(file_id=cld_game_files_id, dest_path=zip_file_path, unzip=False)
with zipfile.ZipFile(zip_file_path, 'r') as zip_ref:
    zip_ref.extractall(APP_DATA)
os.remove(zip_file_path)

# sys_version_file = open(sys_version_file_path, 'wb')
# sys_version_file.write(cld_version_major.to_bytes(4, byteorder='big'))
# sys_version_file.write(cld_version_minor.to_bytes(4, byteorder='big'))
# sys_version_file.write(cld_version_patch.to_bytes(4, byteorder='big'))
# sys_version_file.close()

print('updated to newest version ({0}.{1}.{2})'.format(cld_version_major, cld_version_minor, cld_version_patch))

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