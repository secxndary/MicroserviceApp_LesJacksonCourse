## Запуск проекта:
1. Добавить в файл hosts (`C:\Windows\system32\drivers\etc`) строку:
```
127.0.0.1 acme.com
```
Поменять имя хоста можно в файле `ingress-srv.yaml` в секции `spec/rules/host`
2. Собрать образы и загрузить их в Docker Hub:
```
cd PlatformService
docker build -t {YOUR_DOCKER_ID}/platformservice .
docker push {YOUR_DOCKER_ID}/platformservice
cd CommandService
docker build -t {YOUR_DOCKER_ID}/commandservice .
docker push {YOUR_DOCKER_ID}/commandservice
```

2.1. (Опционально) Запустить контейнеры для проверки синхронной связи между сервисами через HTTP:
```
docker run -p 8080:80 {YOUR_DOCKER_ID}/platformservice
docker run -p 8080:80 {YOUR_DOCKER_ID}/commandservice
```

3. Для запуска в Kubernetes с Ingress Nginx:
```
cd k8s
kubectl apply -f platforms-depl.yaml
kubectl apply -f commands-depl.yaml
kubectl apply -f platforms-nodeport-srv.yaml
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0/deploy/static/provider/aws/deploy.yaml
kubectl apply -f ingress-srv.yaml 
kubectl rollout restart deployment platforms-depl
kubectl rollout restart deployment commands-depl
```
3.1. (Опционально) Для проверки внешней связи PlatformService через NodePort:
```
kubectl get services
```
В списке сервисов, в сервисе `platformnpservice-srv` в разделе PORT(S) скопировать сгенерированный порт формата 3хххх.
Запрос отправить на `http://localhost:{platformnpservice-srv_PORT}`.