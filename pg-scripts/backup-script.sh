#!/bin/bash
# Параметры для подключения к базе данных
PGHOST=db
PGPORT=5432
PGUSER=$POSTGRES_USER
PGPASSWORD=$POSTGRES_PASSWORD
PGDATABASE=$POSTGRES_DB

# Экспорт переменных среды
export PGHOST PGPORT PGUSER PGPASSWORD PGDATABASE

# Путь для сохранения бэкапа
BACKUP_PATH="/backups/backup_$(date +%Y%m%d%H%M%S).sql"

# Выполнение бэкапа
pg_dump -Fc > $BACKUP_PATH
