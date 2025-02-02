#!/bin/bash

/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "YourPassword123" -Q "CREATE DATABASE MyDatabase;"
