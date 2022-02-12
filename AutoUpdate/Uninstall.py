import os
import shutil

APP_DATA = os.getenv('APPDATA')+'/'
game_files_directory_path = APP_DATA+'.Grapple/'

if(os.path.isdir(game_files_directory_path)):
    shutil.rmtree(game_files_directory_path)