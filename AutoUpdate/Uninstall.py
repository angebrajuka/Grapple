import os
import Paths as p
import shutil

def uninstall():
    if(os.path.isdir(p.game_files_directory_path)):
        shutil.rmtree(p.game_files_directory_path)

if(__name__ == "__main__"):
    uninstall()

# compile with pyinstaller:
# pyinstaller -F Uninstall.py