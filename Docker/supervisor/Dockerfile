FROM alpine

RUN sed -i 's/dl-cdn.alpinelinux.org/mirrors.ustc.edu.cn/g' /etc/apk/repositories
RUN apk add --no-cache \
        ca-certificates \
        tzdata \
        curl \
        bash \
        supervisor
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime

COPY assets/supervisor/ /

WORKDIR /usr/local/

ENTRYPOINT ["sleep", "999h"]
