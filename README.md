# ChatNet
ChatNet is an API for chat application that allows users to chat with each other in real time.

## Features
- User registration and authentication
- Creating and joining chats
- Sending and receiving messages
- Sending and receiving files
- Receiving notifications in real time

## Technologies and tools:
- Framework: ASP.NET Core 7.0
- Database: PostgreSQL
- MessageQueue: RabbitMQ
- Gateway: Ocelot
- Real-time communication: SignalR
- Logging: Serilog
- Containerization: Docker
- Reverse proxy: Nginx

## Installation
### On local machine
1. Clone the repository
2. Install requirements:
    - RabbitMQ ([link](https://www.rabbitmq.com/download.html))
    - PostgreSQL ([link](https://www.postgresql.org/download/))
3. Run services together:
    - ChatNet.Auth.API
    - ChatNet.Backend.API
    - ChatNet.Files.API
    - ChatNet.Gateway
    - ChatNet.Notification.API
4. (Optional) Run service to work with notifications:
    - ChatNet.Notifications.Client
### On server
1. Clone the repository
2. Configure Nginx:
    - Copy `nginx.conf` to `/etc/nginx/sites-available/chatnet`
    - Create a symbolic link to `/etc/nginx/sites-enabled/chatnet`
    - Restart Nginx `nginx -s reload`
3. Install Docker and Docker Compose ([link](https://docs.docker.com/engine/install/ubuntu/))
4. Create docker network `docker network create external-network`
5. Run `docker-compose up -d` to start the containers
- Logs are stored in the `/var/log/chat-net` folder

## API Documentation
Full API documentation is available at `http://hostname/api/swagger/index.html`