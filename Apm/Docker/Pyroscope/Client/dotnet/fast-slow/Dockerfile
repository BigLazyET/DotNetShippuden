FROM mcr.microsoft.com/dotnet/sdk:5.0

WORKDIR /dotnet

COPY --from=pyroscope/pyroscope:latest /usr/bin/pyroscope /usr/bin/pyroscope
ADD example .

RUN dotnet publish -o . -r $(dotnet --info | grep RID | cut -b 6- | tr -d ' ')

ENV PYROSCOPE_APPLICATION_NAME=fast-slow.dotnet.app
ENV PYROSCOPE_SERVER_ADDRESS=http://localhost:4040/
ENV PYROSCOPE_LOG_LEVEL=debug

RUN adduser --disabled-password --gecos --quiet pyroscope
USER pyroscope

CMD ["pyroscope", "exec", "-spy-name", "dotnetspy", "/dotnet/example"]
