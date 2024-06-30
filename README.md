# 这是学习资料存储对应的服务器
# 环境
基于.NET8 的ASP.NET框架搭建，使用SQLite作为数据库
# 示例代码
`Test.py`既是一个测试程序，也是一段示例代码


# Web API

## 定义

文件指针（file_pointer）:为了能精确地表示每一个文件，每一个文件都有唯一对应的文件指针。

## POST

### /upload
用于上传文件

POST主体为表单
1. kcm:string 课程名
2. kch:string 课程号
3. details:string 详细信息
4. file_name:string 文件名
5. file:文件
6. uploader:string 上传者

### /rate?file_pointer=...&rating=...
用于打分

url参数有两个，file_pointer是文件指针（long），rating是打分（5分制）

## GET

### /search/[keyword]
返回格式：json
功能：用关键词搜索，返回一个列表，列表中的类型如下图所示
```json
[
    {
        "kcm":string,//课程名
        "kch":string,//课程号
        "details":string,//详情
        "file_name":string,//文件名
        "file_size":int,//文件大小（以byte为单位）
        "file_pointer":long,///文件指针
        "upload_time":long,//该文件的上传时间
        "rating":float,//打分，五分制；若为负数，说明该值无效
        "rating_number":int,//有多少人打分
        "uploader":string//上传者
    },
    ...
]
```

### /get_file/[file_pointer]
返回文件指针对应的文件


### /user/[uploader]/files
返回一个列表，包含了这个用户上传的文件对应的文件详情(返回类型与  /search  相同)

