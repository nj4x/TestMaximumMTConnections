# TestMaximumMTConnections
Maximum TS terminals probing application

MS Visual Studio .Net Framework 4.8 project.
An application that can be used to get an idea on how many MT4/5 terminals can your NJ4X Terminal Server hardware handle.

![](https://i.imgur.com/rxDkmiG.png)


```bash
set VS="C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe"

git clone https://github.com/nj4x/TestMaximumMTConnections.git

cd TestMaximumMTConnections
nuget restore
%VS% TestMaximumMTConnections.sln /build Debug /out build.log && sleep 15

TestMaximumMTConnections\bin\Debug\TestMaximumMTConnections.exe
```
