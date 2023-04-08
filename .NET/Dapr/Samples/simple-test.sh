# dotnet publish -c Release ./DaprHttpBackend -o ./artifacts/daprhttpbackend/

dapr run --app-id webapi-http --app-port 5235 --dapr-http-port 6235 && \ 
        #  --app-protocol http --config /root/.dapr/config.yaml --components-path /root/.dapr/components && \
        dotnet ./artifacts/daprhttpbackend/dapr-http-backend.dll --urls "http://*:5235"