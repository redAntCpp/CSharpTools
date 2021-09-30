# C#开发工具类

此项目主要记录平时常用的C#类。以文件夹形式存放，建立此仓库，以便记录翻找。
此项目主要包括以下常用工具源代码：
1. 常用数据库操作工具类（DBHelper）
2. 常用数据操作类（DataHelper）
3. 常用web对接类（WebHelper）
4. 常用加密类（Cryptography）
5. 日志记录类（LogHelper）
6. 文件上传类（UpdateFile）

## 修改记录
### 2021年9月30日19:01:25
1. 对DBHelpe、Cryptography、LogHelper 的项目文件进行了移除，保留核心代码
2. CryptoHelper新增MS5加密方式，对DES加密进行优化，对RSA加密进行优化
3. DBHelpe：优化了sqlserver操作处理，对执行涉及修改的存储过程的添加事务机制，部分选择语句新增参数化实现，以保证其安全性
