# 🐳 Docker Setup - AppCreditosBackEnd API

Esta guía te ayudará a construir y desplegar la API de AppCreditos usando Docker.

## 📋 Prerrequisitos

- Docker Desktop instalado
- .NET 8 SDK (solo para desarrollo local)
- Acceso a la base de datos SQL Server

## 🚀 Build y Ejecución Local

### Opción 1: Script automático (Recomendado)
```powershell
# Ejecutar el script de build
.\docker-build.ps1
```

### Opción 2: Comandos manuales
```bash
# Build de la imagen
docker build -t appcreditos-backend:latest .

# Ejecutar el contenedor
docker run -d \
  --name appcreditos-backend-container \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e "ConnectionStrings__DefaultConnection=TU_CONNECTION_STRING" \
  -e "JwtSettings__Secret=TU_JWT_SECRET" \
  appcreditos-backend:latest
```

## 🌐 URLs de Acceso

Una vez ejecutando el contenedor:

- **API Base**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger
- **Health Check**: http://localhost:8080/health

## ⚙️ Variables de Entorno

### Requeridas para Producción:
```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<tu-connection-string>
JwtSettings__Secret=<tu-jwt-secret>
JwtSettings__Issuer=CreditPlatformAPI-Production
JwtSettings__Audience=CreditPlatformUsers-Production
JwtSettings__ExpiresInMinutes=30
```

### Opcionales:
```bash
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft.AspNetCore=Warning
AllowedHosts=*
CorsSettings__AllowedOrigins=https://tu-frontend.com
```

## 🔄 Despliegue en Render.com

### 1. Preparar el repositorio
```bash
# Subir archivos a GitHub
git add .
git commit -m "Add Docker configuration"
git push origin main
```

### 2. Configurar en Render
1. Conecta tu repositorio GitHub
2. Selecciona "Web Service"
3. Usar el archivo `render.yaml` incluido
4. Configurar las variables de entorno secretas

### 3. Variables de entorno en Render
- `ConnectionStrings__DefaultConnection`: Tu string de conexión a BD
- `JwtSettings__Secret`: Generar un secret seguro (Render puede generarlo automáticamente)

## 🛠️ Comandos Útiles

### Gestión del contenedor:
```bash
# Ver logs
docker logs appcreditos-backend-container

# Acceder al contenedor
docker exec -it appcreditos-backend-container /bin/bash

# Parar el contenedor
docker stop appcreditos-backend-container

# Eliminar el contenedor
docker rm appcreditos-backend-container

# Eliminar la imagen
docker rmi appcreditos-backend:latest
```

### Debugging:
```bash
# Ver procesos de contenedores
docker ps

# Ver uso de recursos
docker stats

# Inspeccionar la imagen
docker inspect appcreditos-backend:latest
```

## 🔧 Troubleshooting

### Problema: Container no inicia
```bash
# Revisar logs detallados
docker logs appcreditos-backend-container --details

# Verificar variables de entorno
docker exec appcreditos-backend-container env
```

### Problema: No se puede conectar a la BD
- Verificar que el connection string esté correcto
- Verificar que la BD permita conexiones externas
- Revisar firewalls y reglas de seguridad

### Problema: Health check falla
- Verificar que el endpoint `/health` esté respondiendo
- Revisar logs de la aplicación
- Verificar conectividad a la base de datos

## 📊 Optimizaciones de Producción

El Dockerfile incluye:

- ✅ **Multi-stage build** para imágenes más pequeñas
- ✅ **Non-root user** para seguridad
- ✅ **Layer caching** optimizado
- ✅ **Health checks** configurados
- ✅ **Runtime optimizations** (.NET ReadyToRun)

## 🔒 Seguridad

- Nunca incluyas secretos en el Dockerfile
- Usa variables de entorno para configuración sensible
- La imagen ejecuta con usuario no-root
- Mantén las imágenes base actualizadas

## 📞 Soporte

Si tienes problemas:

1. Revisa los logs del contenedor
2. Verifica las variables de entorno
3. Confirma conectividad a la base de datos
4. Revisa la documentación de Render.com
