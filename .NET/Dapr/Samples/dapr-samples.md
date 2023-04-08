# Dapr Samples

## 任何语言，任何平台

* [Dapr sidecar (daprd) overview (包含了重要的几个接口)](https://docs.dapr.io/concepts/dapr-services/sidecar/)

### DaprHttpBackend

* 无需引用任何dapr相关的东西，依赖或者组件SDK等，直接编译生成编译产物
* 直接dapr run xxxx dotnet启动sidecar和dotnet

### 接口的请求方式

> 假设dapr run的命令为：
```
dapr run --app-id webapi-http --app-port 5235 --dapr-http-port 6235 dotnet ./artifacts/daprhttpbackend/dapr-http-backend.dll --urls "http://*:5235"
```
> 假设开放了一个HttpGet接口：/foo

#### 1. 采用dotnet应用地址直接请求

> http://localhost:5235/foo

#### 2. 采用sidecar地址请求

**如果采用的dapr init --slim方式初始化dapr运行时的话，则无法通过下面接口请求sidecar，从而访问dotnet服务**
> http://localhost:6235/v1.0/invoke/webapi-http/method/user/list
