# =============================================================================
# Script para build y test local del contenedor Docker
# AppCreditosBackEnd API
# =============================================================================

# Configuración
$IMAGE_NAME = "appcreditos-backend"
$IMAGE_TAG = "latest"
$CONTAINER_NAME = "appcreditos-backend-container"
$PORT = "8080"

Write-Host "🚀 Iniciando build de Docker para AppCreditosBackEnd API..." -ForegroundColor Green

# Limpiar contenedores anteriores si existen
Write-Host "🧹 Limpiando contenedores anteriores..." -ForegroundColor Yellow
docker stop $CONTAINER_NAME 2>$null
docker rm $CONTAINER_NAME 2>$null

# Build de la imagen
Write-Host "🔨 Construyendo imagen Docker..." -ForegroundColor Blue
docker build -t "${IMAGE_NAME}:${IMAGE_TAG}" . --no-cache

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error en el build de Docker" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Imagen construida exitosamente: ${IMAGE_NAME}:${IMAGE_TAG}" -ForegroundColor Green

# Mostrar información de la imagen
Write-Host "📊 Información de la imagen:" -ForegroundColor Cyan
docker images $IMAGE_NAME

# Ejecutar el contenedor
Write-Host "🏃 Ejecutando contenedor..." -ForegroundColor Blue
docker run -d `
    --name $CONTAINER_NAME `
    -p "${PORT}:8080" `
    -e ASPNETCORE_ENVIRONMENT=Development `
    -e "ConnectionStrings__DefaultConnection=Server=databasemain.cx20gk0oi9bt.us-east-2.rds.amazonaws.com,1433;Database=CreditsDatabase;User Id=dafesanc;Password=Dafesanc.1224+;Encrypt=true;" `
    -e "JwtSettings__Secret=TXlTZWNyZXRLZXlGb3JKV1RUb2tlbkNyZWRpdFBsYXRmb3JtQXBp" `
    -e "JwtSettings__Issuer=CreditPlatformAPI" `
    -e "JwtSettings__Audience=CreditPlatformUsers" `
    -e "JwtSettings__ExpiresInMinutes=30" `
    "${IMAGE_NAME}:${IMAGE_TAG}"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error ejecutando el contenedor" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Contenedor ejecutándose exitosamente" -ForegroundColor Green
Write-Host "🌐 API disponible en: http://localhost:${PORT}" -ForegroundColor Cyan
Write-Host "📋 Swagger UI: http://localhost:${PORT}/swagger" -ForegroundColor Cyan
Write-Host "❤️  Health Check: http://localhost:${PORT}/health" -ForegroundColor Cyan

# Mostrar logs del contenedor
Write-Host "`n📄 Logs del contenedor (presiona Ctrl+C para salir):" -ForegroundColor Yellow
docker logs -f $CONTAINER_NAME
