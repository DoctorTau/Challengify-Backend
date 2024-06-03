# Database Backup and Restore Instructions

This document provides the necessary commands to manually backup and restore your PostgreSQL database using the custom Docker setup.

## Backup

To manually trigger a backup of the PostgreSQL database, execute the following command from the root directory of your project where your `docker-compose.yml` is located:

```bash
docker-compose exec pg_backup /usr/local/bin/backup-script.sh
```

This command runs the backup-script.sh script inside the pg_backup service container. The script creates a backup of the PostgreSQL database and saves it in the /backups directory inside the container, which is mapped to a local directory.

## Restore

To restore the database, you have two options depending on your needs:

### Restoring the Latest Backup

To restore the database using the most recent backup file:

``` bash
docker-compose exec pg_backup /usr/local/bin/restore-script.sh --latest
```

This command will find the latest backup file in the /backups directory and use it to restore the database.

### Restoring a Backup from a Specific Date

If you need to restore a backup from a specific date, use the following command format, replacing YYYYMMDD with the date you have the backup for:

```bash
docker-compose exec pg_backup /usr/local/bin/restore-script.sh --date YYYYMMDD
```

This command will search for a backup file that includes the specified date in its name and use it to restore the database.

## Notes

Ensure that the Docker containers are running before executing these commands.
These operations can affect your current database data. Always make sure to proceed with caution, especially in a production environment.

Backups should be tested regularly to ensure they can be restored correctly.
For any issues or further customization needs, refer to the Docker and PostgreSQL documentation or consult your system administrator.
