# Restaurant Management System API

API для системы управления рестораном, разработанное с использованием ASP.NET Core и PostgreSQL.

## Функциональность

- Управление заказами (создание, обновление статуса, отмена)
- Управление меню (категории, блюда)
- Управление столиками
- Система статусов для заказов и позиций
- Управление пользователями и ролями

## Технологии

- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL
- Swagger/OpenAPI
- Docker (опционально)

## Требования

- .NET 8.0 SDK
- PostgreSQL 15 или выше
- Git

## Установка и запуск

1. Клонируйте репозиторий:
```bash
git clone https://github.com/yourusername/RestaurantManagementSystem.git
cd RestaurantManagementSystem
```

2. Настройте строку подключения к базе данных в `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=restaurant_db;Username=your_username;Password=your_password"
  }
}
```

3. Примените миграции и создайте базу данных:
```bash
dotnet ef database update
```

4. Запустите приложение:
```bash
dotnet run
```

5. Откройте Swagger UI в браузере:
```
https://localhost:5001
```

## Тестовые данные

При первом запуске приложения автоматически создаются следующие тестовые данные:

### Роли
- Администратор
- Официант
- Повар

### Статусы заказов
- Новый
- В обработке
- Готовится
- Готов
- Подано
- Отменен

### Статусы позиций
- Новый
- Готовится
- Готов
- Подано
- Отменен

### Тестовый пользователь
- Логин: admin
- Пароль: admin
- Роль: Администратор

### Категории блюд
- Закуски
- Супы
- Горячие блюда
- Десерты
- Напитки

### Блюда
- Цезарь с курицей (450₽)
- Борщ (350₽)
- Стейк Рибай (1200₽)
- Тирамису (350₽)
- Латте (250₽)

### Столики
- 5 столиков с разной вместимостью (2, 2, 4, 4, 6 мест)

## API Endpoints

### Заказы
- `GET /api/orders/active` - Получить список активных заказов
- `GET /api/orders/{id}` - Получить информацию о заказе
- `POST /api/orders` - Создать новый заказ
- `PUT /api/orders/{id}/status` - Обновить статус заказа
- `PUT /api/orders/{orderId}/items/{itemId}/status` - Обновить статус позиции в заказе
- `DELETE /api/orders/{id}` - Отменить заказ

## Разработка

### Структура проекта
```
RestaurantManagementSystem/
├── Controllers/         # API контроллеры
├── Data/               # Контекст базы данных и миграции
├── Models/             # Модели данных
├── Services/           # Бизнес-логика
└── Program.cs          # Точка входа приложения
```

### Добавление новых миграций
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Лицензия

MIT 