docker build . -t pyroscope:dotnet -f Dockerfile
docker run -itd --net=host --cap-add=sys_ptrace --name pyroscopeclient pyroscope:dotnet 
