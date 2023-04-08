FROM hub.17usoft.com/dotnet/runtime:6.0

RUN rm -rf /etc/supervisor/* \
    && rm -f /opt/otel/config.yaml

ENV APPNAME=opentelemetry.collector.test
ENV ENVIRONEMNTTYPE=qa
ENV ENVIRONMENT=qa
ENV PORT0=12345

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    jq \
    supervisor \
    && rm -rf /var/lib/apt/lists/*

COPY Publish/ /usr/local/app/
COPY etc/ /etc/
COPY opt/ /opt/
COPY usr/ /usr/ 

WORKDIR /usr/local/

CMD [ "./start.sh" ]

