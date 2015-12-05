@echo off
echo Turning on ETW inlining trace
logman start clrevents -p {e13c0d23-ccbc-4e12-931b-d9cc2eee27e4} 0x1000 5 -ets
pause
echo Turning off ETW inlining trace
logman stop clrevents -ets