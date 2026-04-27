# Task Management System Requirements

## Project Overview

The Task Management System is a full-stack web application that allows registered users to manage personal tasks through a responsive Angular frontend, a .NET ASP.NET Web API for task management, and a separated Identity Provider (IDP) server for user handling.

The project exists to demonstrate clean backend architecture, test-driven development practices, RESTful API design, separated identity management, authentication and authorization, persistent data storage, and frontend integration. The backend services must follow Clean Architecture principles where applicable and must not use Entity Framework, Dapper, or Mediator.

## Informal User Story

As a registered user, I want to log in and manage my personal tasks so that I can organize my work, track progress, and keep my task data private.

## Product Scope

The application will support user registration and login through a dedicated IDP server, protected access to task management features through the Task Management API, and complete CRUD operations for task records. Each task belongs to a user identity issued by the IDP, and authenticated users can only manage their own tasks.

The main entities are:

- `User`: stored and managed by the IDP server with identity, login information, and audit data.
- `Task`: stored and managed by the Task Management API with task details, status, owner identity, and audit data.

## Technology Scope

- Frontend: Angular application.
- Backend: .NET C# ASP.NET Web API for task management.
- Identity: separated .NET IDP server for user registration, login, token issuance, and user profile endpoints.
- Architecture: Clean Architecture with Domain, Application, Infrastructure, and API layers.
- Data access: custom data access layers without Entity Framework, Dapper, or Mediator.
- Testing: TDD-oriented automated tests for backend components and API behavior.

## Functional Requirements

### Identity Provider, User Management, and Authentication

FR-001: The system shall include a separated IDP server responsible for user registration, user login, token issuance, and authenticated user profile information.

FR-002: The IDP server shall allow a new user to register with at least a name, email address, and password.

FR-003: The IDP server shall validate that a registered user's email address is unique.

FR-004: The IDP server shall store user information in persistent storage separate from task records.

FR-005: The IDP server shall store passwords securely using a password hash instead of storing plain text passwords.

FR-006: The IDP server shall allow a registered user to log in with valid credentials.

FR-007: The IDP server shall reject login attempts with invalid credentials.

FR-008: The IDP server shall issue a signed authentication token after successful login.

FR-009: The issued authentication token shall include the user identifier required by the Task Management API to enforce ownership rules.

FR-010: The IDP server shall expose an endpoint that returns the currently authenticated user's basic profile information.

FR-011: The IDP server shall include seeded demo credentials for presentation and reviewer testing.

### Authorization

FR-012: The system shall expose at least one public endpoint that does not require authentication.

FR-013: The Task Management API shall expose protected task management endpoints that require authentication.

FR-014: The Task Management API shall validate tokens issued by the IDP server before allowing protected operations.

FR-015: The Task Management API shall reject unauthenticated requests to protected endpoints.

FR-016: The Task Management API shall ensure users can only access, update, or delete tasks that belong to their own IDP-issued user identity.

### Task Management

FR-017: The system shall allow authenticated users to create a task.

FR-018: The system shall require each task to have a title.

FR-019: The system shall allow each task to include a description.

FR-020: The system shall allow each task to have a status, such as `Pending`, `InProgress`, or `Completed`.

FR-021: The system shall allow each task to include an optional due date.

FR-022: The system shall store a unique identifier for each task.

FR-023: The system shall associate each task with the IDP-issued user identifier of the user who created it.

FR-024: The system shall allow authenticated users to list their own tasks.

FR-025: The system shall allow authenticated users to view the details of one of their own tasks.

FR-026: The system shall allow authenticated users to update editable fields of one of their own tasks.

FR-027: The system shall allow authenticated users to update a task's status.

FR-028: The system shall allow authenticated users to delete one of their own tasks.

FR-029: The system shall return a not found response when a task does not exist or does not belong to the authenticated user.

### API Behavior

FR-030: The IDP server shall expose API endpoints for user registration, login, and authenticated user profile retrieval.

FR-031: The Task Management API shall expose RESTful API endpoints with appropriate HTTP verbs for task operations.

FR-032: The backend services shall return meaningful HTTP status codes for successful requests, validation failures, unauthorized access, forbidden access, and missing records.

FR-033: The backend services shall validate API request data before executing business use cases.

FR-034: The backend services shall return response models that do not expose password hashes or database implementation details.

FR-035: The backend services shall expose API documentation or discoverable endpoint lists suitable for local development and review.

### Data Storage

FR-036: The IDP server shall provide persistent storage for users.

FR-037: The Task Management API shall provide persistent storage for tasks.

FR-038: The user data store shall include a unique identifier and at least two additional fields.

FR-039: The task data store shall include a unique identifier and at least two additional fields.

FR-040: Each backend service shall have a data access layer that provides the operations required by its application use cases.

FR-041: The data access implementations shall not use Entity Framework, Dapper, or Mediator.

FR-042: The application shall include seeded task data for demo purposes.

### Frontend

FR-043: The Angular frontend shall provide a user-friendly login screen backed by the IDP server.

FR-044: The Angular frontend shall provide a user registration screen backed by the IDP server.

FR-045: The Angular frontend shall provide a task list screen for authenticated users.

FR-046: The Angular frontend shall provide a form for creating tasks.

FR-047: The Angular frontend shall provide a form or interaction for editing tasks.

FR-048: The Angular frontend shall provide an action for deleting tasks.

FR-049: The Angular frontend shall display validation and API error messages in a user-friendly way.

FR-050: The Angular frontend shall store and send the IDP-issued authentication token required to call protected Task Management API endpoints.

FR-051: The Angular frontend shall prevent unauthenticated users from accessing protected task management screens.

FR-052: The Angular frontend shall be organized into clear components, services, models, and routing concerns.

### Documentation and Demo

FR-053: The project shall include a README with local setup instructions.

FR-054: The README shall document how to run the IDP server, Task Management API, Angular frontend, database or data store setup, and tests.

FR-055: The README shall include seeded demo credentials.

FR-056: The project presentation shall include the informal user story and explain how it drove the implementation.

## Non-Functional Requirements

### Architecture and Maintainability

NFR-001: The Task Management API shall follow Clean Architecture with separate Domain, Application, Infrastructure, and API responsibilities.

NFR-002: The IDP server shall keep identity use cases, identity persistence, and API endpoints separated through clear architectural boundaries.

NFR-003: Backend dependencies shall point inward: API depends on Application, Infrastructure depends on Application, Application depends on Domain, and Domain has no external dependencies.

NFR-004: Domain models shall not depend on ASP.NET, persistence frameworks, SQL APIs, frontend contracts, IDP implementation details, or infrastructure concerns.

NFR-005: API request and response models shall be separate from domain entities and persistence models when concepts differ.

NFR-006: Controllers or endpoints shall remain thin and delegate business behavior to application use cases.

NFR-007: Business rules and validation shall be implemented in the Domain or Application layers, not in controllers or persistence code.

NFR-008: Repository contracts shall be defined at the Application boundary and implemented in Infrastructure.

NFR-009: The Task Management API shall depend on stable identity contracts such as token validation and user claims, not on direct access to IDP persistence.

NFR-010: The codebase shall use clear business-oriented names and avoid vague generic classes such as helpers or managers unless justified.

### Testability and TDD

NFR-011: Backend development shall follow TDD-oriented practices where tests define expected behavior before or alongside implementation.

NFR-012: Tests shall cover business rules and invariants for task and user behavior.

NFR-013: Tests shall cover IDP use cases such as registration, duplicate email validation, login success, login failure, and profile retrieval.

NFR-014: Tests shall cover Task Management API use cases such as task creation, task updates, task deletion, and ownership checks.

NFR-015: Tests shall cover data access behavior against the real persistence boundary where practical.

NFR-016: Tests shall cover API behavior for success, validation failure, unauthorized access, and not found scenarios across both backend services.

NFR-017: Test classes shall use readable specification-style naming and focus assertions on behavior that matters to each scenario.

### Security

NFR-018: Passwords shall never be stored or returned in plain text.

NFR-019: Protected API endpoints shall require a valid authentication token issued by the IDP server.

NFR-020: The Task Management API shall validate token signature, issuer, audience, expiration, and required user identity claims.

NFR-021: The system shall avoid exposing sensitive implementation details in API errors.

NFR-022: The system shall enforce task ownership checks on the backend, regardless of frontend behavior.

NFR-023: Authentication secrets, signing keys, issuer values, audience values, and configuration settings shall be managed through local configuration and not hard-coded into source files intended for version control.

### Usability and Accessibility

NFR-024: The Angular frontend shall be responsive across common desktop and mobile screen sizes.

NFR-025: Primary user flows shall be understandable without special training: register, log in, view tasks, create task, edit task, delete task, and log out.

NFR-026: Form validation messages shall clearly explain what the user needs to correct.

NFR-027: Destructive actions, such as deleting a task, shall require a clear user interaction that reduces accidental deletion.

NFR-028: The UI shall provide visible loading or disabled states while submitting login, registration, and task changes.

### Reliability and Data Integrity

NFR-029: The application shall preserve user and task data between application restarts when using the configured local data stores.

NFR-030: The system shall reject invalid task status values.

NFR-031: The system shall maintain created and updated timestamps for user and task records when supported by the implementation.

NFR-032: Seeded identity and task data shall be repeatable so reviewers can run the application locally and reach a known demo state.

### Performance

NFR-033: The backend APIs shall respond within an acceptable time for local interview demo usage under small data volumes.

NFR-034: Task list retrieval shall be scoped to the authenticated user to avoid unnecessary data exposure and excessive payload size.

NFR-035: The frontend shall avoid unnecessary full page reloads during normal task management operations.

### Portability and Setup

NFR-036: The project shall be runnable on a local development machine using documented commands.

NFR-037: Local setup shall not require paid external services.

NFR-038: Required environment variables, configuration files, service URLs, ports, and prerequisites shall be documented in the README.

NFR-039: The IDP server and Task Management API startup order shall be documented for local demo execution.

NFR-040: The data store setup shall be simple enough for reviewers to reproduce during an interview review.

### Observability and Supportability

NFR-041: The backend services shall include basic logging for startup and error scenarios.

NFR-042: Each backend service shall provide a public health or ping endpoint suitable for verifying that the service is running.

NFR-043: API errors shall use consistent response shapes where practical.

NFR-044: The README shall include troubleshooting notes for common setup or demo issues, including IDP-to-API token validation issues.

## Acceptance Summary

The requirement is satisfied when a reviewer can follow the README, start the IDP server, Task Management API, and Angular frontend, log in with seeded credentials or create a new user through the IDP, perform all task CRUD operations through the UI and API, verify that protected endpoints reject unauthenticated or invalid tokens, and inspect automated tests that document the expected behavior.
