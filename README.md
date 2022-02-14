# C#开发工具类

此项目主要记录平时常用的C#类。以文件夹形式存放，建立此仓库，以便记录翻找。
此项目主要包括以下常用工具源代码：
1. 常用数据库操作工具类（DBHelper）
2. 常用数据操作类（DataHelper）
3. 常用web对接类（WebHelper）
4. 常用加密类（Cryptography）
5. 日志记录类（LogHelper）
6. 文件上传类（UpdateFile）
7. 项目配置类（Config）

## 修改记录
- 2022年2月14日16:58:12
1. 丰富了DBHelper的支持，新增了sqlite、mysql、oracle数据库的支持
2. 实现了使用ssh连接mysql
- 2022年2月6日22:40:05
3. 优化jsonHelper
- 2021年9月30日19:01:25
4. 对DBHelpe、Cryptography、LogHelper 的项目文件进行了移除，保留核心代码
5. CryptoHelper新增MD5加密方式，对DES加密进行优化，对RSA加密进行优化
6. DBHelper：优化了sqlserver操作处理，对执行涉及修改的存储过程的添加事务机制，部分选择语句新增参数化实现，以保证其安全性
7. 新增项目配置类，具体见：https://www.zhihu.com/people/redant-58/columns
