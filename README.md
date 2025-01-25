# NakamaFeatureTests


to dockerize unity dedicated server linux build

first build the linux build. it can be done from commandline
then call this Dockerfile. 

FROM ubuntu:latest

        # Install necessary dependencies
        RUN apt-get update && \
        apt-get install -y mono-devel libgdiplus && \
        apt-get clean
        
        # Copy server files
        WORKDIR /app
        COPY . .
        
        
        # Expose the port your server listens on (adjust as needed)
        RUN chmod +x ./ddd.x86_64
        EXPOSE 7777
        
        # Run the server
        CMD ["./ddd.x86_64"]


build and run or call it from docker-compose.

using development build also can help. 

running in wsl is simpler to see logs from ubuntu. 

not its working great. 