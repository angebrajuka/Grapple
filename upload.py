import subprocess
import os
import shutil

path = 'C:\\Users\\braju\\AppData\\Roaming\\.Grapple'
output_path = 'C:\\Users\\braju\\AppData\\Roaming\\.Grapple.zip'

if os.path.exists(output_path):
    os.remove(output_path)

output_path = output_path[:len(output_path)-4]

if os.path.exists(path):
    shutil.make_archive(output_path, 'zip', path)
