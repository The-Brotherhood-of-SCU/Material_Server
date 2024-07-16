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

### /comment
在POST的BODY中使用json传参
```json
{
    "account":string,//账号
    "file_pointer":long,//评论的这个文件对应的文件指针
    "text":string,//评论的内容
    "rating":float,//打分，五分制
    "file_name":string//对应的文件名
}
```
评论

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

### /file/[file_pointer]
返回文件指针对应的文件

### /delete/[file_pointer]
删除该文件指针对应的文件


### /user/[uploader]/files
返回一个列表，包含了这个用户上传的文件对应的文件详情(返回类型与  /search  相同)

### /user/[user]/comments
获取某个用户的全部评论，格式为json
```json
[
    {
        "account":string,
        "file_pointer":long,
        "timestamp":long,
        "text":string,
        "rating":float,
    },
    ...
]
```


### /comment/[file_pointer]
获取某个文件对应的全部评论，格式与`/user/[user]/comments`api相同

### /recommend?keyword=...&grades=...
获取智能推荐，

传入参数：keyword 关键词(string),grades 成绩（0-1之间的浮点数）

传出参数：与`/search`相同

### file_detail/{filePointer}
根据这个文件指针返回fileDetail

返回格式与/search中的`列表中的元素类型`相同

```json
{
        "account":string,
        "file_pointer":long,
        "timestamp":long,
        "text":string,
        "rating":float,
}
```