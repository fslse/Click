Protocol Buffers - Google's data interchange format

GitHub地址： https://github.com/protocolbuffers/protobuf

使用方法：

1. https://github.com/protocolbuffers/protobuf/tags 下载最新版本的protobuf和protoc（下面以28.2为例）
2. 解压protobuf-28.2.zip
3. 进入protobuf-28.2\csharp\src目录，将Google.Protobuf目录下内容构建为DLL，复制到Unity项目中即可
4. 解压protoc-28.2-win64.zip，使用bin目录下的proto.exe可以根据proto文件生成cs脚本文件