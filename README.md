# Запуск проекта:

#### Необходимое установленное ПО:
- .NET 6
- dotnet ef (Command-line Tools 9.0.3)
- Docker Desktop (4.38.0)
- Kubernetes (Kubernetes 1.31.4, Kubeadm 1.31.4)
- (Опционально) Средство визуального просмотра БД в Docker (DBeaver, SSMS, etc.)


### 1. Добавить в файл hosts (`C:\Windows\system32\drivers\etc`) строку:
```
127.0.0.1 acme.com
```
Поменять имя хоста, используемое Ingress Nginx, можно в файле `ingress-srv.yaml` в секции `spec/rules/host`.


### 2. Собрать образы и загрузить их в Docker Hub:
```
cd PlatformService
docker build -t {YOUR_DOCKER_ID}/platformservice .
docker push {YOUR_DOCKER_ID}/platformservice
```
Выполнить вышеописанные команды для всех сервисов: `CommandService`, `AuthService`, `ApiGateway`


#### 2.1. (Опционально) Запустить контейнеры для проверки синхронной связи между сервисами через HTTP по ClusterIP:
```
docker run -p 8080:80 {YOUR_DOCKER_ID}/platformservice
docker run -p 8080:80 {YOUR_DOCKER_ID}/commandservice
```


### 3. Запустить сервисы в Kubernetes с Ingress Nginx:
```
cd k8s
kubectl apply -f platforms-depl.yaml
kubectl apply -f commands-depl.yaml
kubectl apply -f auth-depl.yaml
kubectl apply -f apigw-depl.yaml
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0/deploy/static/provider/aws/deploy.yaml
kubectl apply -f ingress-srv.yaml
```
Найти команду `kubectl apply` с актуальной версией Ingress Nginx можно здесь: <br />
https://kubernetes.github.io/ingress-nginx/deploy/#network-load-balancer-nlb


#### 3.1. (Опционально) Проверка внешней связи с `PlatformService` через NodePort:
```
kubectl get services
```
В списке сервисов, в сервисе `platformnpservice-srv` в разделе PORT(S) скопировать сгенерированный порт формата 3хххх. <br />
Запрос отправить на `http://localhost:{platformnpservice-srv_PORT}`


### 4. Добавить Local PVC и контейнер MSSQL Server с ClusterIP, LoadBalancer и паролем, хранящемся в Secret:
```
kubectl apply -f local-pvc.yaml
kubectl create secret generic mssql --from-literal=MSSQL_SA_PASSWORD="pa55w0rd!"
kubectl apply -f mssql-plat-depl.yaml
```


### 5. Добавить миграции в `PlatformService`:
###### Это обходной способ для создания файла миграции локально (без использования Production базы данных).
1. Поменять окружение на `Production` (в командной строке от имени администратора):
```
setx /M ASPNETCORE_ENVIRONMENT "Production"
```
2. Создать миграцию командой:
```
dotnet ef migrations add InitialMigration 
```
3. Запустить проект `PlatformService` локально.


### 6. Добавить RabbitMQ (с ClusterIP и LoadBalancer):
```
kubectl apply -f rabbitmq-depl.yaml
```
Проверить работоспособность RabbitMQ и отправку сообщений можно на `localhost:15672` (логин – `guest`, пароль – `guest`).
Асинхронное сообщение отправляется через RabbitMQ при создании нового объекта Platform в `PlatformService`.


### 7. Добавить gRPC:
gRPC конфигурируется в проекте `PlatformService` в следующих файлах:
- `platforms-depl.yaml` – в ClusterIP добавляется порт gRPC, равный 666 (можно выбрать любой другой);
- `PlatformService.csproj` и `CommandService.csproj` – в ItemGroup прописывается путь к папке с proto-файлами;
- `Protos/platforms.proto` – сам proto-файл; 
- `PlatformService/appsettings.Production.json` – указываются URL и протоколы для gRPC (порт 666, протокол HTTP/2) и Web API (порт 80, протокол HTTP/1);
- `CommandService/appsettings.Production.json` – указываются URL и порт (666) к gRPC Platform в Production Environment. <br />

#### Для генерации кода классов на основании proto-файлов необходимо собрать проект `PlatformService`:
```
cd PlatformService
dotnet build 
```


### 8. Создать, загрузить и добавить в проект NuGet-пакет с Shared-кодом:
1. Создать и выгрузить NuGet-пакет по [гайду с MSDN](https://learn.microsoft.com/ru-ru/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli)
2. После загрузки каждой новой версии пакета необходимо подождать ~20 минут, пока он проиндексируется и появится в списке доступных пакетов
3. Если нужно изменить код пакета и загрузить новую версию, то нужно выполнить следующие команды:
```
cd Shared
dotnet build --configuration Release
dotnet pack --configuration Release --output ./nupkg
cd .\nupkg\
dotnet nuget push Secxndary.MicroserviceApp.Shared.1.0.1.nupkg  --api-key {YOUR_API_KEY} --source https://api.nuget.org/v3/index.json
```

<hr />

### Обновление docker image и k8s deployment (на примере PlatformService):
```
cd PlatformService
docker build -t {YOUR_DOCKER_ID}/platformservice .
docker push {YOUR_DOCKER_ID}/platformservice
kubectl rollout restart deployment platforms-depl
```


### Если не работает HTTPS в gRPC в k8s:
```
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```
