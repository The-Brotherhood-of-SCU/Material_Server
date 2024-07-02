import requests
import json
import time

base_url="http://127.0.0.1:5000"
#base_url="http://124.221.108.135:5000"

user="sb"

def line(s):
    print ("-------------------",s,"-------------------")

# test file upload
line("FILE UPLOADING")
# 设置要上传的文件路径和表单数据
file_path = "README.md"  # 请替换为你要上传的文件路径
file_name = "web api"  # 如果file_name在表单中是必需的
form_data = {
    "kcm": "value1",
    "kch": "value2",
    "details": "value3",
    "uploader":user,
    "file_name": file_name  # 这里的file_name应该与实际文件名一致
}
# 打开文件
with open(file_path, 'rb') as file:
    # 发送POST请求
    response = requests.post(base_url+"/upload", files={"file": (file_name, file)}, data=form_data)

# 检查响应
if response.status_code == 200:
    print("文件上传成功！")
    print("响应内容：", response.content)
else:
    print("文件上传失败！")
    print("状态码：", response.status_code)
    print("响应内容：", response.content)


time.sleep(1)
# Test search

line("SEARCHING")
reponse=requests.get(base_url+"/search/api")
l=json.loads(reponse.content)
entry=l[0]
print("Get the json",entry)

# Test rate
line("RATING")
requests.post(base_url+"/rate?file_pointer="+str(entry["file_pointer"])+"&rating=5")

# Test User
line("USER")
url=base_url+"/user/"+user+"/files"
r=requests.get(url)
print(r.content)
# Test Get
line("DOWNLOAD")

response=requests.get(base_url+"/get_file/"+str(entry["file_pointer"]))

#print(response.text)

line("comment")

data={
    "account":"123",
    "file_pointer":0,
    "text":"goood",
    "rating":3
}
reponse=requests.post(base_url+"/comment",data=data)

line("get comment by file ptr")
r=requests.get(base_url+"/comment/0")
print(r.content)


line("get comment by user name")
r=requests.get(base_url+"/user/123/comments")
print(r.content)

line("get recommendation")

r=requests.get(base_url+"/recommend?keyword=123&grades=0.5")
print(r.content)