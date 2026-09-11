# Payment POC

## Objective

Build a payment POC consisting of:

- Angular frontend
- ASP.NET Core Web API microservice
- Entity Framework Core
- SQL Server
- Code First database approach

---

# Frontend

Technology:

- Angular
- TypeScript
- Reactive Forms
- CSS

Payment page should contain:

- Cardholder name
- Card number
- Expiry
- CVV
- Billing information
- Amount
- Pay button
- Loading state
- Success state
- Error state

---

# Backend

Technology:

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Code First approach

Microservice:

PaymentService

---

# API

## Create Payment

POST

/api/payments

Request:

{
  "cardholderName": "Test User",
  "cardNumber": "4111111111111111",
  "expiryMonth": 12,
  "expiryYear": 2030,
  "amount": 1000
}

Response:

{
  "paymentId": "...",
  "status": "Success",
  "amount": 1000
}

---

# Database

Database:

PaymentDb

Entities:

Payment

PaymentTransaction

PaymentStatus

---

# Entity Framework

Use:

- DbContext
- Entity configurations
- Code First migrations
- Relationships where required

---

# Security

This is a POC.

Never persist:

- CVV
- Full card number

Use mock payment data.

Use masked card information where required.

---

# Acceptance Criteria

Frontend:

- Payment form works
- Validation works
- API integration works
- Loading state works
- Success state works
- Failure state works

Backend:

- API works
- Validation works
- EF Core works
- Database migration works
- Payment is persisted safely
- Errors are handled

Integration:

Angular
    ↓
Payment API
    ↓
EF Core
    ↓
SQL Server