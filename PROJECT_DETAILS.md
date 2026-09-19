# QIP Ward Tracker

## Overview
QIP Ward Tracker is a lightweight ASP.NET Core MVC web application for capturing basic patient details and tracking daily ward presence. It is optimized for quick data entry, responsive display on mobile and desktop, and simple deployment against a Microsoft SQL Server database.

## Key Features
- 4-digit login screen with fixed access codes: `1111`, `2222`, `3333`
- Responsive ward list for web and mobile screens
- Add, edit, and delete patient records
- Daily ward presence capture using simple tick boxes
- Date selector that defaults to today and shows a rolling view of the previous 5 days, today, and 2 future days
- Current day highlight for easy identification
- Automatic filtering to show only patients whose last ward day is within the last 5 days of the selected date
- AJAX-based updates for the list, presence ticks, edit, and delete actions
- SQL Server setup and seed scripts for quick provisioning

## Solution Structure
- `/Controllers` - login flow and patient management endpoints
- `/Data` - Entity Framework Core SQL Server context
- `/Infrastructure` - simple session-based authorization filter
- `/Models` - patient and ward presence entities
- `/ViewModels` - page and form models
- `/Views` - Razor views and AJAX partials
- `/wwwroot` - responsive CSS and lightweight JavaScript
- `/database` - SQL Server create and seed scripts

## Database Setup
1. Update the SQL Server connection string in `/home/runner/work/QIP/QIP/appsettings.json` and replace `<YOUR_SQL_PASSWORD>` with your SQL Server password.
2. Run `/home/runner/work/QIP/QIP/database/001-create-database.sql` in SQL Server Management Studio.
3. Optionally run `/home/runner/work/QIP/QIP/database/002-seed-sample-data.sql` to load demo records.
4. Start the web app with `dotnet run` from `/home/runner/work/QIP/QIP`.

## Functional Notes
- New patients are automatically marked present on the currently selected date so they appear in the active list immediately.
- Patient rows are sorted alphabetically by surname and initials.
- The final column shows the total number of days recorded in the ward for each patient.
- Minimal validation is applied to keep the workflow fast while preventing duplicate hospital folder numbers.

## Future Phase Ideas
- Reporting for custom start/end date ranges
- Real user authentication and audit trail
- Printable/exportable ward summaries
- Dashboard metrics and trend views
