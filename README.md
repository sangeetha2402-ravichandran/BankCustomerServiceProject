# Bank – Integrated Banking Customer Service Platform

An enterprise-style banking customer service solution built with **Microsoft Dynamics 365 Customer Service, Dataverse, Power Apps, Power Automate, C#, JavaScript, and the Dataverse Web API**.

The project demonstrates how a bank can manage customer information, banking transactions, fraud-related cases, service requests, SLA tracking, knowledge management, automated case processing, escalation, client-side customisation, server-side plug-ins, and a task-focused Canvas App using a shared Dataverse data model.

> **Project type:** Portfolio / hands-on enterprise implementation  
> **Domain:** Banking and Financial Services  
> **Platform:** Microsoft Dynamics 365 and Power Platform  
> **Data:** Sample/test data only — no real customer banking information is stored in this project.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Business Problem](#business-problem)
- [Solution Overview](#solution-overview)
- [Architecture](#architecture)
- [Core Banking Data Model](#core-banking-data-model)
- [Dynamics 365 Customer Service](#dynamics-365-customer-service)
- [Model-Driven Application](#model-driven-application)
- [Case Management](#case-management)
- [Banking Queues](#banking-queues)
- [Service Level Agreements](#service-level-agreements)
- [Knowledge Management](#knowledge-management)
- [Operational Views](#operational-views)
- [Security and Access](#security-and-access)
- [Entitlements and Agent Experience](#entitlements-and-agent-experience)
- [Power Automate](#power-automate)
- [Incoming Email Processing](#incoming-email-processing)
- [Existing Case Detection](#existing-case-detection)
- [High-Priority and Fraud Escalation](#high-priority-and-fraud-escalation)
- [Customer Notification](#customer-notification)
- [C# Dynamics 365 Plug-ins](#c-dynamics-365-plug-ins)
- [JavaScript Customisation](#javascript-customisation)
- [Business Rules](#business-rules)
- [Dataverse Web API](#dataverse-web-api)
- [Canvas App](#canvas-app)
- [Power Fx](#power-fx)
- [Application Design Decisions](#application-design-decisions)
- [Testing and Troubleshooting](#testing-and-troubleshooting)
- [Main Banking Scenario](#main-banking-scenario)
- [Technology Stack](#technology-stack)
- [Repository Structure](#repository-structure)
- [Key Skills Demonstrated](#key-skills-demonstrated)

---

## Project Overview

GBank is an integrated banking customer service platform designed around a realistic financial-services scenario.

Instead of building isolated Power Platform demonstrations, the project uses a **single banking data model** across Dynamics 365 Customer Service, Dataverse automation, custom development, and Power Apps.

The implementation focuses on the customer-service lifecycle:

```text
Customer
   |
   v
Account / Contact
   |
   v
Bank Transaction
   |
   v
Case / Service Request
   |
   +------------------------------+
   |                              |
   v                              v
Power Automate                C# Plug-in
   |                              |
   v                              v
Classification                  Validation
Escalation                      Business Logic
Notification                    Case Processing
   |                              |
   +---------------+--------------+
                   |
                   v
          Queue / SLA / Knowledge
                   |
                   v
          Customer Service Agent
```

The same Dataverse records are also exposed through a **Canvas App** for task-oriented banking operations.

---

## Business Problem

A banking customer-service team needs to handle several types of requests efficiently, including:

- unrecognised transactions;
- suspected fraud;
- card-related problems;
- payment failures;
- complaints;
- general customer-service enquiries;
- VIP customer cases;
- high-priority service requests.

Without an integrated platform, agents may need to search multiple systems, manually identify customers and transactions, create duplicate cases, select queues manually, track SLA deadlines separately, and repeatedly enter the same information.

GBank addresses these problems by bringing customer, transaction, case, automation, escalation, and service-management capabilities into one Dataverse-based solution.

---

## Solution Overview

The implemented solution includes:

- a **Bank Customer Service model-driven application**;
- Dataverse tables for banking and customer-service information;
- Account, Contact, Case and Bank Transaction relationships;
- banking-specific forms and views;
- advanced customer-service queues;
- fraud-queue processing;
- First Response and Resolution SLA tracking;
- Knowledge Articles linked to Cases;
- security-role and queue-membership configuration;
- entitlement concepts and Customer Service workspace practice;
- automated incoming-email processing;
- customer lookup using Dataverse;
- existing active Case detection;
- latest Case selection;
- Case update logic;
- Dataverse Note creation;
- high-priority / fraud escalation;
- customer notification automation;
- C# Dynamics 365 plug-ins;
- plug-in registration and unit testing;
- JavaScript form customisation;
- Xrm.WebApi usage;
- Business Rules;
- Dataverse Web API operations;
- a Dataverse-connected banking Canvas App;
- Power Fx formulas for querying and updating banking data.

---

## Architecture

The currently implemented architecture is intentionally centred on Dynamics 365 and Dataverse.

```text
+--------------------------------------------------+
|                 GBank Users                      |
|      Customer Service / Banking Staff            |
+-------------------------+------------------------+
                          |
                          v
+--------------------------------------------------+
|        Dynamics 365 Customer Service             |
|                                                  |
|  Bank Customer Service Model-Driven App          |
|  Cases | Queues | SLA | Knowledge | Views        |
+-------------------------+------------------------+
                          |
                          v
+--------------------------------------------------+
|                    Dataverse                     |
|                                                  |
| Account | Contact | Case | Bank Transaction      |
| Notes   | Relationships | Security               |
+-----------+----------------------+---------------+
            |                      |
            v                      v
+----------------------+   +-----------------------+
|   Power Automate     |   |  C# Dynamics Plug-in |
|                      |   |                       |
| Email Processing     |   | Case Processing       |
| Case Detection       |   | Priority Logic        |
| Escalation           |   | Validation            |
| Notifications        |   | Server-side Logic     |
+----------+-----------+   +-----------+-----------+
           |                           |
           +-------------+-------------+
                         |
                         v
+--------------------------------------------------+
|             JavaScript / Xrm.WebApi              |
| Form Behaviour | Validation | Dataverse Access   |
+--------------------------------------------------+

                         +

+--------------------------------------------------+
|              GBank Canvas App                    |
|                                                  |
| Screens | Galleries | Forms | Power Fx           |
| Customer / Case / Transaction Operations         |
+-------------------------+------------------------+
                          |
                          v
                      Dataverse
```

---

## Core Banking Data Model

The solution uses Dataverse as the central data platform.

### Account

Represents the banking customer or customer organisation at the account level.

The Account record acts as an important parent record for customer-service and transaction information.

### Contact

Represents an individual customer/contact.

Customer identification is important for email-driven Case processing because incoming messages need to be associated with the correct Dataverse Contact.

### Case

The Dynamics 365 **Case (`incident`)** table is used for banking customer-service issues.

Example Case scenarios include:

- fraud investigation;
- unrecognised transaction;
- card issue;
- payment problem;
- complaint;
- general service enquiry;
- VIP customer request;
- high-priority issue.

### Bank Transaction

A custom Dataverse table is used to represent banking transaction information.

The Bank Transaction table allows service agents and automation to associate a Case with the transaction being investigated.

### Relationships

The project validates the following important relationships:

```text
Account
   |
   +---- Contact
   |
   +---- Bank Transaction

Case
   |
   +---- Bank Transaction
```

These relationships allow an agent to move logically from:

```text
Customer
   -> Account
   -> Transaction
   -> Case
```

without treating each record as disconnected data.

---

## Dynamics 365 Customer Service

Dynamics 365 Customer Service provides the core service-management functionality for the project.

The implemented configuration covers:

- customer records;
- banking Cases;
- transaction-related Case information;
- advanced queues;
- SLA tracking;
- knowledge management;
- operational views;
- security;
- entitlement concepts;
- Customer Service agent experience.

This gives the project a realistic CRM/service-management foundation instead of using Dataverse only as a basic database.

---

## Model-Driven Application

A **Bank Customer Service** model-driven application was built inside the project solution.

The application uses the shared Dataverse data model and provides structured access to banking and service information.

Key model-driven concepts practised include:

- tables;
- columns;
- forms;
- views;
- relationships;
- navigation;
- business logic;
- security;
- Cases;
- queues;
- SLA information;
- Knowledge Articles.

### Why a Model-Driven App?

The model-driven app is appropriate for the main customer-service application because the business process is highly data-oriented.

Customer-service agents need structured access to related records such as:

```text
Account
Contact
Case
Bank Transaction
Knowledge Article
Queue
SLA
```

The platform automatically provides a strong Dataverse-based experience for forms, views, relationships, security and business process management.

---

## Case Management

Case management is a central part of GBank.

A Case represents a banking service issue that needs to be investigated and resolved.

A Case can contain information such as:

- customer;
- subject;
- description;
- priority;
- current status;
- transaction reference;
- escalation status;
- queue;
- SLA information;
- notes;
- knowledge information.

### Example

```text
Customer reports an unrecognised payment
                 |
                 v
Customer identified
                 |
                 v
Transaction identified
                 |
                 v
Fraud Case created or updated
                 |
                 v
Priority evaluated
                 |
                 v
Escalation applied when required
                 |
                 v
Case handled by appropriate support team
```

---

## Banking Queues

Advanced queues were created to organise banking work according to the type of service request.

Configured queues include:

| Queue | Purpose |
|---|---|
| General Support | Standard banking enquiries |
| Card Support | Debit/credit card-related issues |
| Payments | Payment and transaction-processing issues |
| Fraud | Fraud and suspicious transaction Cases |
| Complaints | Customer complaints |
| VIP Support | High-value / priority customer service |

### Fraud Queue Testing

Fraud-related Cases were used to practise queue-based customer-service processing.

A fraud Case can be identified, prioritised and handled by the team responsible for fraud investigation rather than remaining in a generic service queue.

---

## Service Level Agreements

The project includes independent SLA KPIs for:

### First Response

Measures how quickly the bank responds to the customer after the Case is received.

### Resolution

Measures how quickly the Case is resolved.

The project practises the distinction between:

```text
Case Created
   |
   +---- First Response SLA
   |
   +---- Resolution SLA
```

This is important because responding to a customer and completely resolving an issue are different service commitments.

---

## Knowledge Management

Knowledge management is included in the Customer Service implementation.

The project practises:

- creating Knowledge Articles;
- publishing Knowledge Articles;
- searching for relevant knowledge;
- making knowledge available for Case handling;
- linking useful knowledge to Cases.

This allows agents to reuse approved banking support information instead of writing every answer from scratch.

Example knowledge areas could include:

- card security;
- fraud reporting;
- transaction disputes;
- payment troubleshooting;
- banking support procedures.

---

## Operational Views

Operational views make important banking records easier for agents to find.

Views configured/practised in the project include:

- **Open Fraud Cases**
- **VIP Cases**
- **High Priority Cases**
- **Suspicious Fraud Transactions**

These views help customer-service teams focus on actionable records rather than searching the entire Dataverse dataset.

---

## Security and Access

The project includes Dynamics 365 / Dataverse security fundamentals.

Areas practised include:

- security roles;
- users;
- teams;
- Dataverse table privileges;
- record access concepts;
- queue membership.

### Security Principle

Users should receive the access required to perform their role without unnecessarily receiving access to all banking information.

The project therefore treats security as part of solution design rather than as a final UI configuration step.

---

## Entitlements and Agent Experience

The implementation includes practice with:

- entitlement concepts;
- customer-service support eligibility;
- Customer Service workspace;
- agent-oriented Case handling.

This provides experience with how Dynamics 365 supports structured customer-service operations beyond simple CRUD screens.

---

# Power Automate

Power Automate is used to automate repetitive banking customer-service work.

The implementation focuses on Dataverse-based Case processing rather than isolated demonstration flows.

---

## Incoming Email Processing

An incoming-email automation was developed to start Case-processing logic when a banking customer sends an email.

The high-level process is:

```text
New Email Received
       |
       v
Read Sender Information
       |
       v
Search Dataverse Contact
       |
       v
Contact Found?
   /         \
 Yes          No
 |             |
 v             v
Continue     Handle unmatched
Case Logic   customer scenario
```

The flow searches Dataverse to identify the sender before continuing with customer-service processing.

### Screenshot

![Incoming email and Contact lookup](docs/images/01-email-trigger-contact-lookup.png)

### Contact Match Condition

The automation checks whether a matching Dataverse Contact was returned.

![Contact-found condition](docs/images/02-email-flow-condition.png)

---

## Existing Case Detection

One important design goal is to reduce unnecessary duplicate Cases.

Before blindly creating another Case, the automation can search for active Cases associated with the customer.

```text
Customer identified
       |
       v
Search active Cases
       |
       v
Existing Case?
   /        \
 Yes         No
 |            |
 v            v
Select      Continue with
latest      new Case logic
Case
 |
 v
Update Case
```

The implementation practises:

- querying active Cases;
- filtering customer-related Case information;
- selecting the latest relevant record;
- updating the existing Case when appropriate.

![Existing Case lookup](docs/images/06-existing-case-lookup.png)

![Case update](docs/images/07-case-update.png)

---

## Dataverse Note Creation

The flow also practises adding Dataverse Notes during Case processing.

This is useful for preserving information from:

- customer communications;
- automation results;
- investigation details;
- processing comments.

Dynamic content and expressions were used when creating the Note.

![Note creation](docs/images/08-note-creation-expression.png)

---

## High-Priority and Fraud Escalation

High-priority Case logic evaluates business information before applying escalation behaviour.

The implemented escalation condition uses factors including:

- **Priority**
- **Status**
- **Is Escalated**

```text
Case
 |
 v
Evaluate Priority
 |
 v
Evaluate Status
 |
 v
Already Escalated?
 |
 v
Apply required escalation action
```

![Escalation condition](docs/images/03-escalation-condition.png)

This pattern is useful for fraud and other time-sensitive banking scenarios.

---

## Customer Notification

Customer notification is part of the Case automation design.

The purpose is to keep the customer informed when important service-processing events occur instead of depending completely on manual agent communication.

Automation and Case updates are designed to work together so that Dataverse remains the central source of service information.

---

# C# Dynamics 365 Plug-ins

Server-side custom business logic is implemented using C# Dynamics 365 plug-ins.

The project includes Case-processing and high-priority Case scenarios.

A plug-in step was registered against:

```text
Table   : Case
Logical : incident
Message : Create
```

![Registered plug-in step](docs/images/09-plugin-step-registration.png)

---

## Plug-in Concepts Practised

The implementation provides hands-on experience with:

- `IPlugin`;
- `IServiceProvider`;
- execution context;
- Target entity;
- message name;
- primary entity;
- pipeline stages;
- Pre Images;
- Post Images;
- synchronous execution;
- asynchronous execution concepts;
- tracing;
- exception handling;
- filtering concepts;
- execution depth;
- plug-in registration;
- debugging/troubleshooting;
- unit testing.

### Simplified Plug-in Flow

```text
Case Create
    |
    v
Dynamics Event Pipeline
    |
    v
Registered Plug-in Step
    |
    v
Read Target Case
    |
    v
Evaluate Banking Rule
    |
    v
Apply Case Processing
```

---

## Plug-in Testing

Unit testing was added for the plug-in implementation.

A successful high-priority Case test was produced after development and debugging iterations.

![Successful plug-in unit test](docs/images/10-plugin-unit-test-pass.png)

This demonstrates that the plug-in work was not limited to writing C# code; registration, execution and testing were also practised.

---

# JavaScript Customisation

JavaScript is used when behaviour needs to happen directly in the model-driven application form.

The project practises several banking-specific client-side scenarios.

---

## Transaction ID Behaviour

JavaScript is used to control Transaction ID behaviour based on Case information.

The exercise includes:

- showing/hiding a field;
- changing whether the field is required;
- responding to form data.

This demonstrates how form behaviour can adapt to the type of banking issue being handled.

---

## VIP Customer Handling

VIP-related client-side logic was practised to support priority and escalation behaviour.

The purpose is to demonstrate how information already available on the form can affect the agent experience.

---

## Fraud Warning

A fraud-related form notification can immediately alert the user when the Case represents a fraud scenario.

This is useful because the agent receives contextual information while working on the record.

---

## Resolution Validation

Client-side validation is used to prevent incorrect Case-processing steps where required information has not been entered.

This demonstrates the role of JavaScript in interactive form validation.

---

## Bank Transaction Lookup Logic

JavaScript is also used to work with the related Bank Transaction information.

This helps connect Case-processing behaviour to the actual banking transaction being investigated.

---

## Xrm.WebApi

The project practises accessing Dataverse from model-driven application JavaScript using `Xrm.WebApi`.

Important operations include:

```javascript
Xrm.WebApi.retrieveRecord()
Xrm.WebApi.retrieveMultipleRecords()
Xrm.WebApi.createRecord()
Xrm.WebApi.updateRecord()
Xrm.WebApi.deleteRecord()
```

Example pattern:

```javascript
Xrm.WebApi.retrieveMultipleRecords(
    "incident",
    "?$select=title,prioritycode,statecode"
).then(
    function success(result) {
        console.log(result.entities);
    },
    function error(error) {
        console.log(error.message);
    }
);
```

This provides a supported client-side approach to Dataverse data access from Dynamics 365.

---

# Business Rules

Business Rules were practised for simple Dataverse/model-driven form logic.

The project also compares when to use different Power Platform extension technologies.

| Requirement | Suitable Option |
|---|---|
| Simple field rule / visibility / requirement | Business Rule |
| Interactive form behaviour | JavaScript |
| Immediate server-side transactional logic | C# Plug-in |
| Workflow / integration / asynchronous automation | Power Automate |
| Custom task-oriented user interface | Canvas App |

The purpose is not to solve every requirement with code.

A strong Dynamics 365 design selects the lowest-complexity technology that correctly satisfies the business requirement.

---

# Dataverse Web API

The project practises Dataverse Web API / REST operations.

Important concepts include:

- REST;
- OData;
- entity sets;
- record identifiers;
- `GET`;
- `POST`;
- `PATCH`;
- query options;
- error troubleshooting.

### Retrieve Records

```http
GET /api/data/v9.2/accounts
```

### Select Specific Columns

```http
GET /api/data/v9.2/accounts?$select=name,accountnumber
```

### Filter Data

```http
GET /api/data/v9.2/incidents?$filter=prioritycode eq 1
```

### Create a Record

```http
POST /api/data/v9.2/accounts
Content-Type: application/json
```

Example body:

```json
{
  "name": "GBank Demo Customer"
}
```

### Update a Record

```http
PATCH /api/data/v9.2/accounts(<record-guid>)
Content-Type: application/json
```

Example body:

```json
{
  "name": "Updated GBank Customer"
}
```

The exercises focus not only on successful requests but also on understanding why API operations fail.

---

# Canvas App

A banking Canvas App was built against the same Dataverse data used by the main GBank solution.

The Canvas App is not a disconnected demonstration application.

It extends the same banking data model and provides a more task-oriented user experience.

The implementation practises:

- screens;
- galleries;
- forms;
- controls;
- navigation;
- Dataverse data sources;
- customer information;
- Case information;
- transaction information;
- record creation;
- record updates;
- validation;
- reusable UI concepts;
- role-oriented user-interface thinking.

---

## Canvas App Data Flow

```text
Canvas App
    |
    +---- Customer Screen
    |
    +---- Case Screen
    |
    +---- Transaction Screen
    |
    +---- Forms / Galleries
              |
              v
           Power Fx
              |
              v
           Dataverse
              |
     +--------+---------+
     |        |         |
  Account   Case   Bank Transaction
```

---

# Power Fx

Power Fx is used to control Canvas App behaviour and data operations.

The project practises important functions and concepts including:

- variables;
- collections;
- `Filter`;
- `LookUp`;
- `Patch`;
- `SubmitForm`;
- validation;
- navigation;
- Dataverse CRUD operations.

---

## Filter

`Filter` returns multiple records matching a condition.

Example:

```powerfx
Filter(
    'Bank Transactions',
    Status = "Suspicious"
)
```

Possible banking use:

```text
Show all suspicious banking transactions.
```

---

## LookUp

`LookUp` returns a specific record matching a condition.

Example pattern:

```powerfx
LookUp(
    Contacts,
    'Contact Id' = varCustomerId
)
```

Possible banking use:

```text
Find the selected customer record.
```

---

## Patch

`Patch` can create or update Dataverse records with more direct control over the fields being changed.

Example pattern:

```powerfx
Patch(
    Cases,
    Defaults(Cases),
    {
        Title: txtCaseTitle.Text
    }
)
```

Possible banking use:

```text
Create a new service Case from a custom Canvas App screen.
```

---

## SubmitForm

`SubmitForm` is useful when the Canvas App uses an Edit Form and standard data cards.

Example:

```powerfx
SubmitForm(frmCase)
```

The project compares `Patch` and `SubmitForm` instead of treating them as interchangeable.

```text
SubmitForm
    -> convenient for standard Edit Form submission

Patch
    -> more control for custom create/update logic
```

---

## Variables and Collections

Variables are used for application state and navigation context.

Collections provide temporary in-memory tabular data when required.

These concepts support task-oriented Canvas App experiences without creating a second banking data store.

---

# Application Design Decisions

The project includes practical comparison of the main Power Apps application types.

## Model-Driven App

Best suited to the main GBank customer-service application because it provides:

- Dataverse-first design;
- structured forms;
- views;
- relationships;
- security;
- Cases;
- SLA integration;
- queues;
- knowledge-management capabilities.

## Canvas App

Best suited when the banking user needs:

- a highly customised screen;
- a focused workflow;
- a task-oriented experience;
- more control over layout and interaction.

## Power Pages

Power Pages is understood as the external-facing option for customer self-service; it is intentionally outside the implemented scope represented by this README.

---

# Testing and Troubleshooting

Testing is a major part of the project.

The engineering approach is:

```text
Build
  |
  v
Test
  |
  v
Identify Failure
  |
  v
Investigate
  |
  v
Fix
  |
  v
Retest
```

The project intentionally keeps troubleshooting evidence because real enterprise development involves diagnosing failures, not only showing successful screenshots.

---

## Power Automate OData Failure

During Power Automate testing, a Dataverse **Add a new row** operation produced an OData path / record-reference error.

Instead of rebuilding the flow immediately, the run history was inspected to determine the failing action and incorrect record reference.

![OData failure investigation](docs/images/04-escalation-failure-troubleshooting.png)

The condition result and subsequent actions were also inspected from flow run history.

![Flow run result](docs/images/05-flow-run-condition-result.png)

This demonstrates practical troubleshooting of:

- Dataverse references;
- OData paths;
- dynamic content;
- flow conditions;
- run history;
- downstream action behaviour.

---

## Plug-in Troubleshooting

Plug-in development also included iterative testing and debugging.

Important troubleshooting areas include:

- correct message registration;
- correct Dataverse table;
- pipeline stage;
- Target availability;
- attribute existence;
- null handling;
- tracing;
- exception handling;
- test setup.

---

## API Troubleshooting

Dataverse Web API exercises include understanding problems such as:

- incorrect entity-set names;
- invalid GUIDs;
- incorrect JSON payloads;
- invalid lookup bindings;
- unsupported query syntax;
- missing permissions;
- incorrect endpoint construction.

---

# Main Banking Scenario

The main project scenario is based on an unrecognised or potentially fraudulent banking transaction.

```text
Customer reports unrecognised transaction
                 |
                 v
Identify customer/contact
                 |
                 v
Identify banking transaction
                 |
                 v
Search for existing active Case
            /          \
          Found       Not Found
            |             |
            v             v
      Update Case     Create Case
            \             /
             +-----+-----+
                   |
                   v
         Evaluate priority
                   |
                   v
          Apply escalation
                   |
                   v
             Fraud Queue
                   |
                   v
            SLA tracking
                   |
                   v
        Knowledge assistance
                   |
                   v
        Agent investigates Case
                   |
                   v
           Customer updated
```

This scenario connects the major features in the current implementation rather than demonstrating each technology in isolation.

---

# Technology Stack

| Area | Technology |
|---|---|
| CRM / Customer Service | Microsoft Dynamics 365 Customer Service |
| Data Platform | Microsoft Dataverse |
| Low-Code Application | Power Apps Model-Driven App |
| Custom UI | Power Apps Canvas App |
| Automation | Power Automate |
| Server-Side Development | C# Dynamics 365 Plug-ins |
| Client-Side Development | JavaScript |
| Client API | `Xrm.WebApi` |
| Integration API | Dataverse Web API / REST / OData |
| Formula Language | Power Fx |
| Development | Visual Studio / VS Code |
| Plug-in Deployment | Plug-in Registration Tool |
| Source Repository | GitHub |

---

# Repository Structure

A clean repository can organise the implemented artefacts as follows:

```text
GBank/
|
|-- README.md
|
|-- docs/
|   |
|   |-- images/
|       |-- 01-email-trigger-contact-lookup.png
|       |-- 02-email-flow-condition.png
|       |-- 03-escalation-condition.png
|       |-- 04-escalation-failure-troubleshooting.png
|       |-- 05-flow-run-condition-result.png
|       |-- 06-existing-case-lookup.png
|       |-- 07-case-update.png
|       |-- 08-note-creation-expression.png
|       |-- 09-plugin-step-registration.png
|       |-- 10-plugin-unit-test-pass.png
|
|-- plugins/
|   |-- README.md
|   `-- src/
|
|-- javascript/
|   |-- README.md
|   `-- src/
|
|-- power-automate/
|   `-- README.md
|
|-- canvas-app/
|   `-- README.md
|
|-- dataverse-webapi/
|   `-- README.md
|
`-- documentation/
    |-- data-model.md
    |-- automation.md
    |-- plugin-design.md
    `-- testing-and-troubleshooting.md
```

> Adjust the folder names to match the actual repository before committing if your current GitHub structure is different.

---

# Key Skills Demonstrated

This project demonstrates practical experience with:

### Dynamics 365

- Dynamics 365 Customer Service;
- Case management;
- advanced queues;
- SLA KPIs;
- Knowledge Articles;
- views;
- security roles;
- queue membership;
- entitlement concepts;
- Customer Service agent experience.

### Dataverse

- standard and custom tables;
- columns;
- relationships;
- record creation/update;
- Notes;
- lookup relationships;
- Dataverse querying.

### Power Automate

- email triggers;
- Dataverse actions;
- List Rows;
- conditions;
- expressions;
- dynamic content;
- record lookup;
- latest-record selection;
- Case updates;
- escalation;
- customer notification;
- run-history troubleshooting.

### C# Plug-ins

- `IPlugin`;
- execution context;
- Target;
- pipeline stages;
- Pre/Post Images;
- sync/async concepts;
- tracing;
- exception handling;
- registration;
- unit testing;
- troubleshooting.

### JavaScript

- model-driven form scripting;
- field visibility;
- required-level changes;
- notifications;
- validation;
- lookup logic;
- `Xrm.WebApi`.

### Dataverse Web API

- REST;
- OData;
- `GET`;
- `POST`;
- `PATCH`;
- filtering;
- selecting columns;
- troubleshooting API requests.

### Canvas Apps

- screens;
- galleries;
- forms;
- navigation;
- Dataverse connectivity;
- variables;
- collections;
- `Filter`;
- `LookUp`;
- `Patch`;
- `SubmitForm`;
- validation;
- record creation/update.

---

# Portfolio Summary

GBank demonstrates how Microsoft Dynamics 365 and Power Platform can be used together to build an integrated banking customer-service solution.

The project goes beyond basic CRUD functionality by combining:

```text
Banking Data
     +
Customer Service
     +
Case Management
     +
Queues
     +
SLA
     +
Knowledge
     +
Power Automate
     +
C# Plug-ins
     +
JavaScript
     +
Dataverse Web API
     +
Canvas Apps
```

The implementation is designed as a **hands-on enterprise portfolio project** and uses only sample/test banking information.

---

## Disclaimer

This project is created for **learning, interview preparation and portfolio demonstration**.

It is not a production banking system and is not connected to real banking customers, accounts or financial transactions.
