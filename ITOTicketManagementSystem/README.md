# ITO Ticket Management System

This project is a simple IT Ticket Management System developed as an intern assignment. The system allows employees to report IT-related issues, which are then tracked and resolved by the Help Desk and Engineering teams.

---

## Features Implemented

-   **Role-Based Access Control:** The application supports four distinct user roles: Admin, Help Desk Team, Engineering Team, and general Employees.
-   **User Authentication:** Secure user login and registration.
-   **Dashboard:** A summary view of all tickets categorized by their current status.
-   **Ticket Creation:** Employees can create new tickets with a title, description, and an optional file attachment.
-   **Ticket Lifecycle Management:** Help Desk and Engineering users can update a ticket's status (New, InProgress, Resolved, Closed).
-   **Ticket Assignment:** Help Desk users can reassign tickets to the Engineering Team.
-   **Comments System:** Authorized users can add comments to tickets.
-   **History Timeline:** All status changes and reassignments are logged and displayed in a timeline on the ticket details page.
-   **Data Export:** Help Desk users can export the current ticket list to a CSV file.

---

## Technologies Used

-   **Backend:** ASP.NET Core MVC (.NET 8)
-   **Frontend:** HTML, CSS, JavaScript, Bootstrap
-   **Database:** Microsoft SQL Server with Entity Framework Core (Code-First approach)
-   **Authentication:** ASP.NET Core Identity for role-based authentication

---

## Setup Instructions

1.  **Prerequisites:**
    -   [.NET 8 SDK](httpshttps://dotnet.microsoft.com/en-us/download/dotnet/8.0)
    -   [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or other SQL Server instance.
    -   [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) with the ASP.NET and web development workload.

2.  **Configuration:**
    -   Clone the repository.
    -   Open the `appsettings.json` file.
    -   Modify the `DefaultConnection` connection string to point to your local SQL Server instance.

3.  **Database Migration:**
    -   Open the Package Manager Console in Visual Studio (`Tools > NuGet Package Manager > Package Manager Console`).
    -   Run the following command to create the database and apply all migrations:
        ```powershell
        Update-Database
        ```
    -   The application will automatically seed the database with the required roles and dummy user accounts on first run.

---

## Usage / Login Details

You can log in with any of the following pre-configured dummy accounts. The password for all accounts is **`Password123!`**.

| Role               | Email Address          |
| ------------------ | ---------------------- |
| Admin              | `admin@test.com`       |
| Help Desk          | `helpdesk1@test.com`   |
| Help Desk          | `helpdesk2@test.com`   |
| Engineering        | `engineer1@test.com`   |
| Engineering        | `engineer2@test.com`   |
| Engineering        | `engineer3@test.com`   |
| Employee (General) | `employee1@test.com`   |
| Employee (General) | `employee2@test.com`   |
| Employee (General) | `employee3@test.com`   |
| Employee (General) | `employee4@test.com`   |