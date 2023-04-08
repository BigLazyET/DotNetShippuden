#!/bin/bash

export APPDLL=Learn.Demo.WebApp.dll
export PORT0=12345
export APPNAME=opentelemetry.collector.test
export APPID=1234
export ENVIRONMENTTYPE=qa
export ENVIRONMENT=qa
export ASPNETCORE_URLS=http://0.0.0.0:${PORT0}/
#echo $(date "+%Y-%m-%d %H:%M:%S") [INFO] ASPNETCORE_URLS=${ASPNETCORE_URLS}
#echo $(date "+%Y-%m-%d %H:%M:%S") [INFO] starting up the project "\""${APPNAME}"\""

# mkdir for log
mkdir -p /data/logs/console-${APPNAME}/

# project run by supervisord，and set ProcDump trigger
/usr/bin/supervisord -c /etc/supervisor/supervisord.conf

# supervisord watch dotnet process stdout(not the supervisord itself process stdout) as docker console stdout
supervisorctl tail -f dotnet


