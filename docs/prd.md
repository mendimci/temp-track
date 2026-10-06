# Temporary Staffing Request System - TempTrack

# 1. Project Overview

TempTrack is a **Temporary Staffing Request System** developed for St John & St Elizabeth Hospital to streamline and control the process of requesting, approving, and tracking temporary staffing, including overtime, bank, and agency staff.

The system replaces informal, email-based workflows with a structured, digital process that provides **real-time visibility of staffing requests and associated costs before they are incurred**. Managers submit requests with automatically calculated costs, which are then routed through a configurable multi-step approval workflow involving appropriate stakeholders such as department leads and finance.

TempTrack improves governance and accountability by maintaining a complete audit trail of all decisions, while automated notifications ensure timely approvals without manual follow-up. Additionally, it supports reporting by exporting approved data into formats compatible with existing financial and operational reports.

Overall, TempTrack enables the hospital to move from reactive, retrospective reporting to a **proactive and controlled approach to managing temporary staffing spend**.

# 2. Project Objectives

- **Improve Cost Visibility**
Provide real-time insight into temporary staffing costs at the point of request, enabling informed decision-making before spend is incurred.
- **Enforce Structured Approval Processes**
Replace informal verbal and email-based approvals with a standardized, configurable multi-step workflow.
- **Enhance Financial Control**
Prevent overspending on temporary staffing by ensuring all requests are reviewed and approved in line with defined thresholds and policies.
- **Increase Accountability and Transparency**
Maintain a complete audit trail of all requests, approvals, and decisions, including timestamps and approver details.
- **Streamline Request Management**
Enable managers to easily submit, track, and manage requests for overtime, bank, and agency staff in a centralized system.
- **Automate Notifications and Reduce Delays**
Ensure timely approvals through automated email notifications, eliminating the need for manual follow-ups.
- **Support Data-Driven Reporting**
Provide structured data exports to integrate with existing reporting processes and improve analysis of staffing trends and costs.
- **Implement Secure Access Control**
Use SSO (Azure AD) to ensure secure access, with role-based permissions so users only see relevant data.
- **Improve Operational Efficiency**
Reduce administrative overhead and manual processes associated with managing temporary staffing requests.

# 3. Scope of Work

### 3.1 Request Management Module

**Description**

A centralized module for creating, managing, and tracking temporary staffing requests (Overtime, Bank, Agency) with automated cost calculation.

**Included Activities**

- Analysis of staffing request types and business rules
- Design of request data model and workflow states
- Implementation of request creation and editing functionality
- Automatic cost calculation based on configured pay rates
- Validation rules and mandatory fields enforcement
- Request tracking and status visibility for users
- Contribution on the UI/UX during development based on the existing design provided by the client.

**Deliverables**

- Functional Request Management module
- Configurable request forms
- Cost calculation logic implementation
- Web interface for request submission and tracking
- Technical documentation

---

### 3.2 Approval Workflow Engine

**Description**

Implementation of a configurable, rule-based multi-step approval workflow to manage staffing requests based on type, cost, and escalation criteria.

**Included Activities**

- Definition of approval rules and escalation logic
- Implementation of multi-step approval process (Manager → Directorate Lead → Finance)
- Role-based approval assignments
- Handling of approval, rejection, and re-submission scenarios
- Configuration of thresholds for escalation (e.g., agency or high-cost requests)
- Audit logging of all approval actions

**Deliverables**

- Workflow engine with configurable rules
- Approval process implementation
- Role-based access and approval configuration
- Audit trail for all workflow actions
- Workflow documentation

---

### 3.3 Notification System

**Description**

An automated notification system to inform users of required actions and status updates throughout the request lifecycle.

**Included Activities**

- Design of notification triggers for each workflow stage
- Integration with email services
- Implementation of automated email notifications (submission, approval, rejection, escalation)
- Template creation for notification messages
- Error handling and retry mechanisms

**Deliverables**

- Configured notification system
- Email templates for all workflow events
- Notification trigger logic
- Documentation of notification flows

---

### 3.4 Audit Trail & Activity Logging

**Description**

A comprehensive audit mechanism to track all actions and decisions within the system for transparency and compliance.

**Included Activities**

- Definition of audit data structure
- Logging of all request actions (creation, updates, approvals, rejections)
- Capture of timestamps, user identities, and comments
- Implementation of audit history view per request
- Secure storage of audit data

**Deliverables**

- Audit logging functionality
- Request-level audit history view
- Secure audit data storage
- Audit documentation

---

### 3.5 Reporting & Data Export

**Description**

The system will provide internal reporting capabilities through **Excel file generation**, enabling users to export structured data for downstream analysis and reporting.

**Included Activities**

- Definition of export data structure and Excel format based on client requirements
- Implementation of Excel file generation within the system
- Filtering capabilities (e.g., by period, department, staff type)
- Data validation to ensure accuracy and consistency of exported data
- Support for multiple export scenarios (e.g., monthly reporting, departmental views)

**Deliverables**

- Excel export functionality
- Configurable export filters
- Structured Excel templates aligned with client requirements
- Reporting and export documentation

---

### 3.6 Authentication & Access Control

**Description**

Secure user authentication and role-based access control using Azure Active Directory (SSO).

**Included Activities**

- Integration with Azure AD for Single Sign-On (SSO)
- Role and permission model definition (Manager, Approver, Finance)
- Implementation of role-based data access (department-level visibility)
- Security and access validation
- Session and identity management

**Deliverables**

- SSO integration with Azure AD
- Role-based access control implementation within the platform
- User access configuration
- Security documentation

---

### 3.7 System Configuration & Administration

**Description**

Administrative capabilities to configure system parameters such as pay rates, approval thresholds, and organizational structure.

**Included Activities**

- Configuration of pay rates for different staffing types
- Setup of approval thresholds and escalation rules
- Management of departments and organizational hierarchy
- Admin interface for configuration management
- Validation and change tracking for configurations

**Deliverables**

- Administration module
- Configuration management interface
- System configuration documentation

---

### 3.8 Deployment & Documentation

**Description**

Deployment of the TempTrack system and delivery of all necessary technical and user documentation.

**Included Activities**

- Environment setup (development, test, production)
- Deployment configuration and setup
- System testing and validation support
- Preparation of technical documentation
- Preparation of user guides

**Deliverables**

- Deployed TempTrack system
- Technical documentation
- User documentation
- Deployment and configuration guides

# 4. Project Timeline (4 Sprints)

<aside>
<img src="https://app.notion.com/icons/bullseye_gray.svg" alt="https://app.notion.com/icons/bullseye_gray.svg" width="40px" />

4 Sprints (1 Sprint = 10 working days)

</aside>

The TempTrack project will be delivered over **four two-week sprints**. Core modules such as request management, approval workflows, and reporting will be developed iteratively, with early validation and continuous integration throughout the project lifecycle.

### Sprint Overview

**Sprint 1 – Architecture & Foundation**

- Definition of overall system architecture
- Analysis of business requirements and workflow rules
- Data model design for requests, approvals, and audit logs
- Setup of development environments and repositories
- Analysis and recommendations about the Design and UX of the platform.
- Azure AD (SSO) integration setup (initial configuration)
- Implementation of Request Management module

---

**Sprint 2 – Core Functionality (Request & Workflow)**

- Cost calculation logic based on pay rates
- Development of Approval Workflow engine (basic flow)
- Role-based access control (Manager, Approver roles)
- Initial UI for request creation and approval actions
- Integration of audit logging for core actions
- Completion of multi-step approval workflow (including escalation rules)

---

**Sprint 3 – Integration & Extended Features**

- Implementation of Notification system (email triggers and templates)
- Reporting and data export functionality (Excel format)
- Enhancement of audit trail and history views
- Frontend–backend integration and feature refinement
- Initial end-to-end testing
- Performance optimization and validation

---

**Sprint 4 – Stabilization, UAT & Handover**

- Bug fixing and system stabilization
- Finalization of SSO and access control
- Configuration of production hosting environment
- Deployment to production environment
- User Acceptance Testing (UAT) support
- Documentation (technical and user guides)
- Knowledge transfer and project handover

# 5. Team Structure

| Role | Name | Description | Commitment |
| --- | --- | --- | --- |
| Senior Backend Developer | John Snow |   • Development of the REST API service
  • Integration of Entra ID for authentication
  • Designing Data Models
  • Integration of email service
  • Development of real time notifications | 100% |
| Senior Frontend Developer | John Smith |   • Development of the Web User Interfaces based on the suggested design by client
  • Integration with the REST API service
  • Configuring the real time notifications
  • Core views as specified | 100% |

# 6. Assumptions

- All detailed business requirements, approval rules, and staffing policies are provided and validated by the client at project start
- Required system access (e.g., Azure AD, email services, reporting tools) is available from Sprint 1
- Pay rates, cost calculation rules, and approval thresholds are defined and maintained by the client
- Organizational structure (departments, roles, approvers) is provided and kept up to date by the client
- Email infrastructure for sending notifications is available and configurable
- Reporting format requirements (e.g., Excel structure) are provided by the client
- Users will access the system via modern web browsers within the hospital environment
- No major changes to core business processes will be introduced during development
- Integration is limited to Azure AD (SSO) and standard email services unless otherwise agreed
- Data required for reporting and exports will be consumed by existing client reporting tools without additional transformation requirements

# 7. Out of Scope

The following items are explicitly excluded unless agreed via change request:

- Development of new business logic beyond the defined TempTrack requirements
- Functional enhancements outside the agreed scope (e.g., additional modules or workflows)
- Integration with external systems beyond Azure AD (SSO) and email services
- Changes to existing hospital financial or reporting systems
- Infrastructure provisioning (e.g., cloud resources, hosting environments) and associated operating costs
- Long-term system operation, monitoring, and maintenance after project delivery
- Data migration from legacy or external systems
- End-user training beyond provided user and technical documentation
- Security audits, penetration testing, or compliance certification activities
- Major UI/UX redesigns after initial approval
- Ongoing support, feature requests, or enhancements after project handover without a separate agreement

# 8. Client Responsibilities

### 8.1 Infrastructure & Environment

**The client is responsible for:**

- Providing access to required environment production
- Providing Azure AD configuration and access for SSO integration
- Providing email infrastructure for notification delivery
- Providing necessary network access and firewall rules
- Providing any required monitoring or logging infrastructure

---

### 8.2 Business Configuration & Data

**The client is responsible for:**

- Defining and maintaining pay rates for overtime, bank, and agency staff
- Defining approval workflows, thresholds, and escalation rules
- Providing organizational structure (departments, roles, approvers)
- Providing Excel templates and required data structure for exports
- Validating business rules, workflows, and exported data

---

### 8.3 Client Responsibilities (General)

- Providing complete and accurate business requirements at project start
- Ensuring availability of key stakeholders for clarification and validation
- Participating in sprint reviews and User Acceptance Testing (UAT)
- Providing timely feedback and approvals

---

### 8.4 Contractor Responsibilities

- Development of the TempTrack system
- Implementation of all in-scope modules (request management, workflow, reporting, etc.)
- Implementation of Excel export functionality within the system
- Integration with Azure AD (SSO) and email services
- Implementation of audit trail and notification system
- System testing and support during UAT
- Preparation of technical and user documentation
- Deployment support and knowledge transfer

---

### 8.5 Explicitly Excluded

- Management or maintenance of client infrastructure
- Ownership of business data accuracy or configuration correctness
- Long-term system support and operations beyond project delivery
- Any integrations beyond agreed scope
- Changes outside the agreed scope without formal change request

# 8. Acceptance Criteria

The project will be considered successfully completed when the following conditions are met:

- The TempTrack system is fully deployed and operational in the agreed environments (development, test, and production)
- All core modules (Request Management, Approval Workflow, Notifications, Audit Trail, and Reporting) are functioning as defined in the scope
- All staffing request types (Overtime, Bank, Agency) can be created, processed, approved, or rejected through the system
- Multi-step approval workflows are executed correctly based on configured rules and thresholds
- Excel export functionality generates accurate and structured files based on approved requests
- Azure AD (SSO) authentication is successfully integrated and role-based access control is enforced
- Email notifications are triggered correctly at each workflow stage without manual intervention
- Audit trail captures all relevant actions, including timestamps, user identity, and comments
- All system components are documented (technical and user documentation completed) and stored in version control
- End-to-end testing is completed and all critical defects are resolved
- User Acceptance Testing (UAT) is successfully completed and signed off by the client
- Final system walkthrough and acceptance meeting are completed with formal approval from the client

# 9. Change Management

Any changes to requirements, scope, or functionality must follow a formal change management process to ensure full transparency and control over project impact.

All change requests must include:

- A clear description of the requested change
- An impact analysis covering scope, timeline, and cost implications
- Assessment of technical feasibility and risks (if applicable)
- Written approval from both the client and the contractor prior to implementation

# 10. Governance & Communication

- The project will follow a **two-week sprint cadence** with structured delivery and incremental releases
- **Sprint planning and sprint review meetings** will be held at the start and end of each sprint to align scope, progress, and feedback
- Clear **technical and business points of contact** will be defined on both the client and contractor side to ensure efficient communication and decision-making
- **Regular status reporting** will be provided, covering progress, risks, blockers, and upcoming activities

# 11. Risks and Mitigation

### 11.1 Identified Risks

- **Risk 1 – Organizational Structure Changes**
    
    Frequent changes in departments, approvers, or roles may affect workflow configuration and system consistency.
    
- **Risk 2 – Email Notification Dependency**
    
    Delays or failures in email delivery infrastructure could impact approval turnaround times and user experience.
    

### 11.2 Mitigation Measures

- Clear definition and approval of Excel export templates before implementation
- Dependency checks and early integration testing for email services
- Role and department structure managed through configurable administration module rather than code changes

# Questions

| **Q** | **A** |
| --- | --- |
| Which cloud provider can be used? | Azure, but hosting can be in a dedicated environment internally.  |
| Services | .NET for the backend, React in the frontend |
| How many approval steps are in the approval flow? Are there always three and is the last approval role Finance? | 2 or 3 roles in the approval chain. CNO (Deputy CNO) → Head of nursing 
  1. Head of Nursing
  2. CNO
  3. 
For agency
  1. manual approval
  2. 
a final flow will be provided |
| Which service can be used to send out emails from the system to recipients?  |  |
- Investment
    
    
    | # roles | % |  |
    | --- | --- | --- |
    | Development x2 | 100% |  |
    |  |  | 44,000.00 EUR |

# Material

[Temp Track - Presentation April 26.pdf](Temp_Track_-_Presentation_April_26.pdf)

[TempTrack-Demo.html](TempTrack-Demo.html)