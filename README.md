#### visitas-mvp-cesar-hernandez-5190-18-7385
Repositorio de Cesar Hernandez con Carnet 5190-18-7385. 
Este repositorio contiene el codigo fuente REST del proyecto de visitas técnicas para el seminario de privados de la sede de antigua. 


COMO EJEUTAR EL PROYECTO REST (API + DOCKER)

Este proyecto es una API REST desarrollada en **.NET 8** que usa **MySQL 8** como base de datos y se ejecuta mediante **Docker Compose**.  
Aquí se explican los pasos para levantar todo en local.

#####REQUISITOS

Asegurarse de tener instalado:

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [.NET SDK 8.0](https://dotnet.microsoft.com/download) (opcional, solo si quieres ejecutar la API fuera de Docker)
- Git (si vas a clonar el repositorio desde GitHub)

---

####CLONAR EL REPOSITORIO

Abre una terminal (CMD, PowerShell o Git Bash) y ejecuta:

bash
git clone https://github.com/C3sar0192/visitas-mvp-cesar-hernandez-5190-18-7385.git 
-nota:Si no se quiere clonar solo descarga el proyecto

####EJECUTAR EL PROYECTO
Dirigirse a raiz del proyecto donde se descargó o se clonó, con comando "cd" desde una terminal.
Lo que se busca es estar dentro de la carpeta "Api" para ejecuta comandos
ejemplo cd E:\Proyectos\visitas-mvp-cesar-hernandez-5190-18-7385\Api

Ejecutar el comando "docker compose up -d --build" dentro de la carpeta "Api" en la terminal
Esto para correr el proyecto y se consumido por el WEB(El proyecto empezará a correr en Doker Desktop)

####Probar la API (Swagger) 
http://localhost:5000/swagger
