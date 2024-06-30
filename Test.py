import requests
import json
import time

base_url="http://127.0.0.1:5000"

# test file upload

# 设置要上传的文件路径和表单数据
file_path = "README.md"  # 请替换为你要上传的文件路径
file_name = "web api"  # 如果file_name在表单中是必需的
form_data = {
    "kcm": "value1",
    "kch": "value2",
    "details": "value3",
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

reponse=requests.get(base_url+"/search/api")
l=json.loads(reponse.content)
entry=l[0]
print("Get the json",entry)

# Test rate

requests.post(base_url+"/rate?file_pointer="+str(entry["file_pointer"])+"&rating=5")


# Test Get
response=requests.get(base_url+"/get_file/"+str(entry["file_pointer"]))

print(response.text)
