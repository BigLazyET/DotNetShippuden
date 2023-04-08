#!/bin/bash

export DAOKEAPPUK=foo.appuk
export DAOKEID=foo.id
export FOO=987654321

echo $(date "+%Y-%m-%d %H:%M:%S") [INFO] the FOO is $FOO

# mkdir for log
mkdir -p /data/logs/console-foo/
mkdir -p /data/logs/console-foo.appuk-foo.id/

# project run by supervisord，and set ProcDump trigger
/usr/bin/supervisord -c /etc/supervisord.conf

# supervisord watch dotnet process stdout(not the supervisord itself process stdout) as docker console stdout
supervisorctl tail -f foo