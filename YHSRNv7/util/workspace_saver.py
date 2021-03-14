import os, glob, datetime, shutil

class WorkspaceSaver():
    def __init__(self, dirpath = '_results', move_exists = False):

        if move_exists and os.path.exists(dirpath):
            dt_now = datetime.datetime.now()
            os.rename(dirpath, dirpath + '_' + \
                str(dt_now.year) + str(dt_now.month).zfill(2) + \
                str(dt_now.day).zfill(2) + str(dt_now.hour).zfill(2) + \
                str(dt_now.minute).zfill(2) + str(dt_now.second).zfill(2))

        os.makedirs(dirpath, exist_ok=True)

        for subdirs in ['/src', '/log', '/snap', '/sample']:
            os.makedirs(dirpath + subdirs, exist_ok=True)

        ignores = lambda srcdirpath : srcdirpath == dirpath or srcdirpath[0] == '_'

        srcfiles = glob.glob('**/*.py', recursive=True)
        srcfiles = [srcfile for srcfile in srcfiles if not ignores(srcfile.replace('\\', '/').split('/')[0])]

        for srcfile in srcfiles:
            dstpath = dirpath + '/src/' + srcfile
            os.makedirs(os.path.dirname(dstpath), exist_ok=True)

            shutil.copy2(srcfile, dstpath)

        self.__dirpath = dirpath

    @property
    def source(self):
        return self.__dirpath + '/src/'

    @property
    def log(self):
        return self.__dirpath + '/log/'

    @property
    def snap(self):
        return self.__dirpath + '/snap/'

    @property
    def sample(self):
        return self.__dirpath + '/sample/'