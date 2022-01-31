import os
import sys
from google_drive_downloader import GoogleDriveDownloader as gdd
import zipfile
import shutil

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

shutil.rmtree(game_files_directory_path)
zip_file_path = './temp.zip'
gdd.download_file_from_google_drive(file_id=cld_game_files_id, dest_path=zip_file_path, unzip=False)
with zipfile.ZipFile(zip_file_path, 'r') as zip_ref:
    zip_ref.extractall(APP_DATA)
os.remove(zip_file_path)

sys_version_file = open(sys_version_file_path, 'wb')
byte_array = bytearray([cld_version_major, cld_version_minor, cld_version_patch])
sys_version_file.write(byte_array)
sys_version_file.close()

print('updated to newest version ({0}.{1}.{2})'.format(cld_version_major, cld_version_minor, cld_version_patch))