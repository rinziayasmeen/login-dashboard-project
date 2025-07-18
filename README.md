﻿# Login Dashboard Project

Fullstack app using Angular (frontend) and .NET 8 Web API (backend).
- JWT Authentication
- Protected Dashboard with Chart.js
- Rate-limiting on login

---

### Tech Stack

- **Frontend:** Angular 17, Chart.js
- **Backend:** ASP.NET Core 8 Web API
- **Authentication:** JSON Web Token (JWT)
- **Security:** Simple in-memory rate limiting

---
### Setup Instructions
#### Backend (.NET Core API)
Open the solution in Visual Studio

Run the project (make sure it runs on:
http://localhost:5156)

Test the API in Swagger:
http://localhost:5156/swagger

Key APIs:
POST /api/auth/login → Login (returns JWT)
GET /api/dashboard/data → Protected (requires token)

#### Frontend (Angular)
Open terminal inside the frontend folder:
cd frontend
npm install
Start Angular development server:
ng serve
Open browser at:
http://localhost:4200

#### Auth Flow
User logs in via /api/auth/login

JWT token is stored in browser's localStorage

Angular HTTP interceptor adds the token to all protected API requests

Auth guard blocks dashboard route access if not logged in

Dashboard Functionality
Uses ng2-charts to render a bar chart
Data is fetched from /api/dashboard/data (secured with JWT)
Example chart: Ticket status distribution (Open, In Progress, Closed)

Rate Limiting (Simple)
Login API uses a simple in-memory rate limiter

Blocks repeated failed login attempts from the same IP
Prevents basic brute force attacks

