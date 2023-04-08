# yq

> yq是一款轻量级的操作YAML的命令行工具，采用类jq语法，可以处理yaml和json文件，后者处理暂没有jq全面

> [yq官方文档](https://mikefarah.gitbook.io/yq/)

## 一、安装

不做赘述，[官方安装文档](https://github.com/mikefarah/yq/#install)

## 二、使用

考虑以下一个示范yaml（后缀可以yaml，yml）文件：foo.yaml
```yml
# yaml表示以上对象
person:
  userName: zhangsan
  boss: false
  birth: 2019/12/12 20:12:33
  age: 18
  pet: 
    name: tomcat
    weight: 23.4
  interests: [篮球,游泳]
  animal: 
    - jerry
    - mario
  score:
    english: 
      first: 30
      second: 40
      third: 50
    math: [131,140,148]
    chinese: {first: 128,second: 136}
  salarys: [3999,4999.98,5999.99]
  allPets:
    sick:
      - {name: tom}
      - {name: jerry,weight: 47}
    health: [{name: mario,weight: 47}]	
```

### 1. 创建（新建，转化）
```shell

```

### 2. 插入（值，节点，片段，整体，合并）

### 3. 更新

### 4. 删除

### 5. 获取（查询，过滤，美化）
```shell

```