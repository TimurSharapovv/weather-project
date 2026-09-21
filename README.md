# 🌤️ Микросервисная система для сбора, обработки и хранения данных о погоде в г. Казани

## 🏗️ Архитектура

<img width="1473" height="408" alt="image" src="https://github.com/user-attachments/assets/f552bc94-8979-4696-82ac-ed08b3f50819" />

### Service A
- Каждую минуту опрашивает открытый API Open-Meteo.
- Получает актуальные данные: температура, влажность, описание погоды.
- Отправляет данные в Kafka topic `weather`.

### Service B
- Подписывается на топик `weather` в Kafka.
- Десериализует сообщения.
- Отправляет данные в Service C через gRPC-метод `SetWeather`.

### Service C
- Принимает данные через gRPC.
- Сохраняет данные в PostgreSQL с использованием Entity Framework Core.
- Предоставляет REST API для получения последних 10 записей.
- Автоматически удаляет устаревшие записи (старше 10 часов).
- Фоновая очистка выполняется при запуске и повторяется каждые 10 часов.

---

## 🛠️ Стек технологий

**C# 12** | **.NET 9** | **ASP.NET Core** | **gRPC** | **REST API** | **Apache Kafka** | **PostgreSQL** | **Entity Framework Core** | **Docker Compose** | **Swagger**

---

## 🚀 Инструкция по запуску

### Требования
- **IDE:** Я использую JetBrains Rider, но также подойдет и Visual Studio 2022+
- **SDK:** .NET 9
- **Контейнеры:** Docker Desktop
- **Система:** Windows / Linux / macOS

---

### ⚙️ Пошаговый запуск

#### Шаг 1: Открытие проекта
1. Скачайте или клонируйте репозиторий.
2. Откройте его через Rider (**File → Open**).
3. Дождитесь завершения индексации решения и загрузки NuGet-пакетов.

В **Solution Explorer** (панель слева) должна отобразиться следующая структура:

<img width="318" height="204" alt="image" src="https://github.com/user-attachments/assets/c7dff6df-f964-431e-97c0-43519ba71b85" />

#### Шаг 2: Запуск инфраструктуры (Docker)
1. Откройте встроенный терминал в Rider: **Tools → Terminal** (или `Alt+F12`).
2. Убедитесь, что вы находитесь в **корневой папке проекта** (там, где лежит файл `docker-compose.yml`).
3. Выполните команду:
   ```bash
   docker-compose up -d 
<img width="529" height="273" alt="image" src="https://github.com/user-attachments/assets/fc611b68-b3d9-4cf8-8ad8-532a082fe2dd" />

Эта команда запустит:
- **PostgreSQL** (порт 5432) — хранение данных.
- **Kafka** (порт 9092) — обмен сообщениями.
- **Zookeeper** (порт 2181) — координация Kafka.

Проверьте, что все контейнеры успешно запущены:
  ```bash
  docker-compose ps
```
Вы должны увидеть три контейнера в статусе `Up`:

<img width="473" height="92" alt="image" src="https://github.com/user-attachments/assets/86ade81f-35fe-46ba-9472-da28000c21db" />

> ⏱️ **Рекомендация:** подождите 15–20 секунд после запуска, чтобы PostgreSQL полностью инициализировался.

#### Шаг 3: Запуск сервисов
1. В **Solution Explorer** выделите все 3 проекта (ServiceA, ServiceB, ServiceC).
2. Нажмите на них **правой кнопкой мыши**.
3. В появившемся контекстном меню выберите **Run Multiple Projects** (Запуск нескольких проектов).

   <img width="699" height="830" alt="image" src="https://github.com/user-attachments/assets/b3abbc30-80b8-4375-891b-fabdc69de74f" />

4. В открывшемся окне настроек поставьте галочку **Run** напротив всех трех сервисов для их одновременного запуска.

   <img width="1034" height="752" alt="image" src="https://github.com/user-attachments/assets/54db5672-884d-42b3-a6e8-886653001355" />

5. Нажмите **OK** и дождитесь запуска всех сервисов (в нижней панели Run появятся логи инициализации).

#### Шаг 4: Проверка работы
Проект настроен для запуска «из коробки». Проверить его работу можно, открыв в браузере:
- Прямой запрос к API: `http://localhost:5291/api/Weather`
- Интерфейс Swagger: `http://localhost:5291/swagger`

> ⏱️ **Примечание:** Если при первом запросе вы получаете пустой массив `[]`, подождите 1–2 минуты. Service A опрашивает внешний API раз в минуту, поэтому данные появляются не мгновенно.

**Пример полученных записей:**

<img width="367" height="622" alt="image" src="https://github.com/user-attachments/assets/e2a5c2d3-607d-4c3c-9fb6-3271ac05014d" />

## 🤝 PS: спасибо брату за ревью моего проекта!
