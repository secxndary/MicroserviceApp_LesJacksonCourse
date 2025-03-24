# Запуск проекта:

#### Необходимое установленное ПО:
- .NET 6
- dotnet ef (Command-line Tools 9.0.3)
- Docker Desktop (4.38.0)
- Kubernetes (Kubeadm 1.31.4)
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
cd ..\CommandService
docker build -t {YOUR_DOCKER_ID}/commandservice .
docker push {YOUR_DOCKER_ID}/commandservice
```


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
kubectl apply -f platforms-nodeport-srv.yaml
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0/deploy/static/provider/aws/deploy.yaml
kubectl apply -f ingress-srv.yaml
kubectl apply -f local-pvc.yaml
```
Найти команду `kubectl apply` с актуальной версией Ingress Nginx можно здесь:
https://kubernetes.github.io/ingress-nginx/deploy/#network-load-balancer-nlb


#### 3.1. (Опционально) Проверка внешней связи с PlatformService через NodePort:
```
kubectl get services
```
В списке сервисов, в сервисе `platformnpservice-srv` в разделе PORT(S) скопировать сгенерированный порт формата 3хххх.
Запрос отправить на `http://localhost:{platformnpservice-srv_PORT}`


### 4. Добавить Local PVC и контейнер MSSQL Server с ClusterIP, LoadBalancer и паролем, хранящемся в Secret:
```
kubectl apply -f local-pvc.yaml
kubectl create secret generic mssql --from-literal=MSSQL_SA_PASSWORD="pa55w0rd!"
kubectl apply -f mssql-plat-depl.yaml
```


### 5. Добавить RabbitMQ (с ClusterIP и LoadBalancer):
```
kubectl apply -f rabbitmq-depl.yaml
```


<hr />

### Обновление docker image и k8s deployment (например, PlatformService):
```
cd PlatformService
docker build -t {YOUR_DOCKER_ID}/platformservice .
docker push {YOUR_DOCKER_ID}/platformservice
kubectl rollout restart deployment platforms-depl
```
