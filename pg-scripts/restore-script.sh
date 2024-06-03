#!/bin/bash

# Параметры для подключения к базе данных
PGHOST=db
PGPORT=5432
PGUSER=$POSTGRES_USER
PGPASSWORD=$POSTGRES_PASSWORD
PGDATABASE=$POSTGRES_DB

# Экспорт переменных среды
export PGHOST PGPORT PGUSER PGPASSWORD PGDATABASE

# Каталог с бэкапами
BACKUP_DIR="/backups"

# Функция для получения последнего файла бэкапа
get_latest_backup() {
    ls -t $BACKUP_DIR/*.sql | head -n 1
}

# Функция для поиска файла бэкапа по дате
find_backup_by_date() {
    local backup_date=$1
    ls $BACKUP_DIR/*$backup_date*.sql
}

# Проверка входных параметров
if [ "$1" == "--latest" ]; then
    BACKUP_PATH=$(get_latest_backup)
elif [ "$1" == "--date" ]; then
    if [ -z "$2" ]; then
        echo "Date not specified. Usage: $0 --date YYYYMMDD"
        exit 1
    fi
    BACKUP_PATH=$(find_backup_by_date $2)
    if [ -z "$BACKUP_PATH" ]; then
        echo "No backup found for the date $2."
        exit 1
    fi
else
    echo "Invalid option. Usage: $0 --latest or $0 --date YYYYMMDD"
    exit 1
fi

echo "Restoring database from $BACKUP_PATH"

# Остановка всех подключений к базе данных
psql -U $PGUSER -d $PGDATABASE -c "SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '$PGDATABASE' AND pid <> pg_backend_pid();"

# Удаление текущей базы данных и создание новой
psql -U $PGUSER -c "DROP DATABASE IF EXISTS $PGDATABASE;"
psql -U $PGUSER -c "CREATE DATABASE $PGDATABASE;"

# Восстановление базы данных из бэкапа
pg_restore -U $PGUSER -d $PGDATABASE $BACKUP_PATH

echo "Database restored successfully from $BACKUP_PATH."
